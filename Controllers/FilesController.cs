using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using FilesShareApi.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FilesShareApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("files")]
    public class FilesController : ControllerBase
    {
        private readonly IFileService fileServices;
        private readonly IAmazonS3 s3Client;
        private readonly UserManager<ApplicationUser> userManager;

        /// <summary>
        /// Name of my bucket
        /// </summary>
        private readonly string bucketName = "secretsharingbucket";

        public FilesController(IFileService fileServices, IAmazonS3 s3Client, UserManager<ApplicationUser> userManager)
        {
            this.fileServices = fileServices;
            this.s3Client = s3Client;
            this.userManager = userManager;
        }

        /// <summary>
        /// Get files of user
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public IActionResult GetFiles()
        {
            var filesList = fileServices.GetFiles(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
            var filesListDto = new List<FileResponseEntity>();
            foreach (var file in filesList)
            {
                filesListDto.Add(new FileResponseEntity
                {
                    Id = file.Id,
                    Name = file.Name,
                    Url = file.Url,
                    CreatedTime = file.CreatedTime
                });
            }
            return Ok(filesListDto);
        }

        /// <summary>
        /// upload new file
        /// </summary>
        /// <param name="file">file instance</param>
        /// <param name="deleteOnceDownload">defines whether the file should be deleted after download</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UploadFile(IFormFile file, bool deleteOnceDownload = false)
        {
            try
            {
                await using var newMemoryStream = new MemoryStream();
                await file.CopyToAsync(newMemoryStream);

                var fileExtension = Path.GetExtension(file.FileName);
                var documentId = Guid.NewGuid().ToString();
                var documentNameS3 = documentId + fileExtension;

                var result = $"https://secretsharingbucket.s3.amazonaws.com/{documentNameS3}";

                var uploadRequest = new TransferUtilityUploadRequest
                {
                    InputStream = newMemoryStream,
                    Key = documentNameS3,
                    BucketName = bucketName,
                    CannedACL = S3CannedACL.PublicRead,
                };

                var fileInst = new FileEntity
                {
                    Id = documentId,
                    Name = file.FileName,
                    Url = this.Url.ActionLink() + $"/download?id={documentId}",
                    CreatedTime = DateTime.Now,
                    DeleteAfterDownload = deleteOnceDownload,
                    CreatorId = this.User.FindFirstValue(ClaimTypes.NameIdentifier),
                    DocumentType = file.ContentType,
                    S3Name = documentNameS3,
                    ToDelete = false
                };
                var fileName = fileServices.AddFile(fileInst);

                var fileTransferUtility = new TransferUtility(s3Client);
                await fileTransferUtility.UploadAsync(uploadRequest);
            }

            catch (AmazonS3Exception amazonS3Exception)
            {
                if (amazonS3Exception.ErrorCode != null
                    && (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId") || amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                {
                    throw new Exception("Check the provided AWS Credentials.");
                }
                else
                {
                    throw new Exception("Error occurred:" + amazonS3Exception.Message);
                }
            }
            return Ok($"File {file.FileName} was successfully uploaded");

        }

        /// <summary>
        /// Delete file by it's identifier
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> DeleteFile(string id)
        {
            var fileToDelete = fileServices.DeleteFile(id, this.User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (fileToDelete == null)
            {
                return NotFound();
            }
            try
            {
                var fileTransferUtility = new TransferUtility(s3Client);
                await fileTransferUtility.S3Client.DeleteObjectAsync(new DeleteObjectRequest()
                {
                    BucketName = bucketName,
                    Key = fileToDelete.S3Name
                });
            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                if (amazonS3Exception.ErrorCode != null
                    && (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId") || amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                {
                    throw new Exception("Check the provided AWS Credentials.");
                }
                else
                {
                    throw new Exception("Error occurred: " + amazonS3Exception.Message);
                }
            }
            return Ok($"file {fileToDelete.Name} was successfully deleted");
        }

        [HttpDelete("all")]
        [Authorize]
        public async Task<IActionResult> deleteAllFiles()
        {
            var filesToDelete = fileServices.GetFiles(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
            foreach (var file in filesToDelete)
            {
                await DeleteFile(file.Id);
            }
            return Ok("All your files have been deleted");
        }

        [HttpGet("download")] 
        [AllowAnonymous]
        public async Task<IActionResult> DownloadFile(string id)
        { 
            try
            {
                var file = fileServices.GetFileById(id);
                var fileTransferUtitlity = new TransferUtility(s3Client);

                var objectResponse = await fileTransferUtitlity.S3Client.GetObjectAsync(new GetObjectRequest()
                {
                    BucketName = bucketName,
                    Key = file.S3Name
                });

                if (objectResponse.ResponseStream == null)
                {
                    return NotFound();
                }

                if (file.DeleteAfterDownload)
                {
                    fileServices.SetFileToDelete(new FileEntity
                    {
                        Id = file.Id,
                        Name = file.Name,
                        Url = null,
                        DeleteAfterDownload = true,
                        CreatorId = null,
                        DocumentType = null,
                        S3Name = file.S3Name,
                        ToDelete = true
                    });
                }
                return File(objectResponse.ResponseStream, objectResponse.Headers.ContentType, file.Name);
            }
            catch(AmazonS3Exception amazonS3Exception)
            {
                if (amazonS3Exception.ErrorCode != null
                    && (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId") || amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                {
                    throw new Exception("Check the provided AWS Credentials.");
                }
                else
                {
                    throw new Exception("Error occurred: " + amazonS3Exception.Message);
                }
            }
        }
    }
}

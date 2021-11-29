using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Core;
using Core.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Security.Claims;
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

        [HttpGet("getFiles")]
        [Authorize]
        public IActionResult GetFiles()
        {
            return Ok(fileServices.GetFiles(this.User.FindFirstValue(ClaimTypes.NameIdentifier)));
        }

        [HttpPost("uploadFile")]
        [Authorize]
        public async Task<IActionResult> UploadFile(IFormFile file)
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
                    CreatedTime = DateTime.Now,
                    S3Name = documentNameS3,
                    DocumentType = file.ContentType,
                    CreatorId = this.User.FindFirstValue(ClaimTypes.NameIdentifier)
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

        [HttpDelete]
        [Authorize]
        [Route("deleteFile")]
        public IActionResult DeleteFile(string id)
        {
            var fileName = fileServices.DeleteFile(id);
            return Ok($"file {fileName} was successfully deleted");
        }

        [HttpGet("download")] 
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

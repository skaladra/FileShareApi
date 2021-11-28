using Amazon.S3;
using Amazon.S3.Transfer;
using Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;

namespace FilesShareApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class FilesController : ControllerBase
    {
        private readonly IFileService fileServices;
        private readonly IAmazonS3 s3Client;

        public FilesController(IFileService fileServices, IAmazonS3 s3Client)
        {
            this.fileServices = fileServices;
            this.s3Client = s3Client;
        }

        [HttpGet]
        [Authorize]
        public IActionResult GetFiles(string id)
        {
            return Ok(fileServices.GetFiles(id));
        }

        [HttpPost]
        [Authorize]
        [Route("addFile")]
        public IActionResult AddFile(IFormFile file)
        {
            var id = Guid.NewGuid().ToString();
            var userId = Guid.NewGuid().ToString();
            var url = "hello";
            var fileInst = new FileEntity { Id = id, CreatedTime = DateTime.Now, Url = url, CreatorId = userId };
            var fileName = fileServices.AddFile(fileInst);
            return Ok($"file {fileName} was successfully uploaded");
        }

        [HttpPost("UploadFile")]
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
                    BucketName = "secretsharingbucket",
                    CannedACL = S3CannedACL.PublicRead,
                };

                var fileInst = new FileEntity
                {
                    Id = documentId,
                    Name = file.FileName,
                    CreatedTime = DateTime.Now,
                    DocumentType = file.ContentType,
                    CreatorId = "1"
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
            return Ok("GetFiles");

        }

        [HttpDelete]
        [Authorize]
        [Route("deleteFile")]
        public IActionResult DeleteFile(string id)
        {
            var fileName = fileServices.DeleteFile(id);
            return Ok($"file {fileName} was successfully deleted");
        }

        [HttpGet("auth")]
        public IActionResult Authenticate()
        {
            return Ok("no way");
        }
    }
}

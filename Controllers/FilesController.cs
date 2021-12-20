using FilesShareApi.Entity;
using FilesShareApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
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
        private readonly IS3Service s3Service;

        public FilesController(IFileService fileServices, IS3Service s3Service)
        {
            this.fileServices = fileServices;
            this.s3Service = s3Service;
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
        public IActionResult UploadFile(IFormFile file, bool deleteOnceDownload = false)
        {
            var fileId = Guid.NewGuid().ToString();
            var fileNameS3 = fileId + Path.GetExtension(file.FileName);

            try
            {
                s3Service.UploadFileToS3(file, fileNameS3);
                var downloadUrl = this.Url.ActionLink() + $"/download?id={fileId}";
                var createdTime = DateTime.Now;

                var fileInst = new FileEntity
                {
                    Id = fileId,
                    Name = file.FileName,
                    Url = downloadUrl,
                    CreatedTime = createdTime,
                    DeleteAfterDownload = deleteOnceDownload,
                    CreatorId = this.User.FindFirstValue(ClaimTypes.NameIdentifier),
                    DocumentType = file.ContentType,
                    S3Name = fileNameS3,
                    ToDelete = false
                };
                var fileName = fileServices.AddFile(fileInst);

                var fileResponse = new FileResponseEntity
                {
                    Id = fileId,
                    Name = fileName,
                    Url = downloadUrl,
                    CreatedTime = createdTime
                };
                return Ok(fileResponse);
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        /// <summary>
        /// Delete file by it's identifier
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Authorize]
        public IActionResult DeleteFile(string id)
        {
            var fileToDelete = fileServices.DeleteFile(id, this.User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (fileToDelete == null)
            {
                return NotFound();
            }
            try
            {
                s3Service.DeleteFileFromS3(fileToDelete.S3Name);
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
            return Ok($"file {fileToDelete.Name} was successfully deleted");
        }

        [HttpDelete("all")]
        [Authorize]
        public IActionResult deleteAllFiles()
        {
            var filesToDelete = fileServices.GetFiles(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
            foreach (var file in filesToDelete)
            {
                DeleteFile(file.Id);
            }
            return Ok($"All your files({filesToDelete.Count}) have been deleted");
        }

        /// <summary>
        /// Download file by it's identifier
        /// Sets useless files to delete by FilesCleaner
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("download")] 
        [AllowAnonymous]
        public async Task<IActionResult> DownloadFile(string id)
        {
            var file = fileServices.GetFileById(id);
            if (file == null)
                return NotFound();
            try
            {
                var objectResponse = await s3Service.DownloadFileFromS3(file.S3Name);

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
            catch(Exception)
            {
                return StatusCode(500);
            }
        }
    }
}

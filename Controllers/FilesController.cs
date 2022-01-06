﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FilesShareApi
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
        public async Task<IActionResult> GetFiles()
        {
            var user = this.User;

            if (user == null)
            {
                return StatusCode(404);
            } 

            var filesList = await fileServices.GetFiles(user.FindFirstValue(ClaimTypes.NameIdentifier));

            return Ok(FilesMapper.CreateListDto(filesList));
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
            byte[] byteFile;

            await using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                byteFile = memoryStream.ToArray();
            }

            try
            {
                var fileInst = new FileEntity
                    (
                        this.User.FindFirstValue(ClaimTypes.NameIdentifier),
                        file.ContentType,
                        file.FileName,
                        this.Url.ActionLink(),
                        deleteOnceDownload
                    );

                s3Service.UploadFileToS3(byteFile, fileInst.S3Name);

                var fileName = fileServices.AddFile(fileInst);

                return Ok(FilesMapper.CreateDto(fileInst));
            }

            catch (Exception exception)
            {
                return StatusCode(500, exception.InnerException); 
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

            catch (Exception exception)
            {
                return StatusCode(500, exception.InnerException);
            }
            return Ok(FilesMapper.CreateDto(fileToDelete));
        }

        [HttpDelete("all")]
        [Authorize]
        public async Task<IActionResult> DeleteAllFiles()
        {
            var filesToDelete = await fileServices.GetFiles(this.User.FindFirstValue(ClaimTypes.NameIdentifier));

            foreach (var file in filesToDelete)
            {
                DeleteFile(file.Id);
            }

            return Ok(FilesMapper.CreateListDto(filesToDelete));
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
            {
                return NotFound();
            }

            try
            {
                var objectResponse = await s3Service.DownloadFileFromS3(file.S3Name);

                if (file.DeleteAfterDownload)
                {
                    fileServices.SetFileToDelete(file);
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

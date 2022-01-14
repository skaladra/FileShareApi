using Microsoft.AspNetCore.Authorization;
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
    [Route("/files")]
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

            var filesList = await fileServices.GetAll(user.FindFirstValue(ClaimTypes.NameIdentifier));

            return Ok(FileMapper.CreateListDto(filesList));
        }

        [HttpGet("/files/all")]
        [Authorize(Roles =("Admin"))]
        public IActionResult GetAllFiles()
        {
            var filesList = fileServices.GetAllByAdmin();

            return Ok(FileMapper.CreateListDto(filesList));
        }

        [HttpDelete("/files/admin/1")]
        [Authorize(Roles = ("Admin"))]
        public async Task<IActionResult> DeleteFileByAdmin([FromQuery] string id)
        {
            var fileToDelete = await fileServices.DeleteOneByAdmin(id);

            if (fileToDelete == null)
            {
                return NotFound();
            }

            try
            {
                await s3Service.DeleteFileFromS3(fileToDelete.S3Name);
            }

            catch (Exception exception)
            {
                return StatusCode(500, exception.InnerException);
            }
            return Ok(FileMapper.CreateDto(fileToDelete));
        }

        /// <summary>
        /// upload new file
        /// </summary>
        /// <param name="file">file instance</param>
        /// <param name="deleteOnceDownload">defines whether the file should be deleted after download</param>
        /// <returns></returns>
        [HttpPost("/files/1")]
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

                await s3Service.UploadFileToS3(byteFile, fileInst.S3Name);

                var fileResponse = await fileServices.AddOne(fileInst);

                return Ok(FileMapper.CreateDto(fileResponse));
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
        [HttpDelete("/files/1")]
        [Authorize]
        public async Task<IActionResult> DeleteFile([FromQuery] string id)
        {
            var fileToDelete = await fileServices.DeleteOne(id, this.User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (fileToDelete == null)
            {
                return NotFound();
            }

            try
            {
                await s3Service.DeleteFileFromS3(fileToDelete.S3Name);
            }

            catch (Exception exception)
            {
                return StatusCode(500, exception.InnerException);
            }
            return Ok(FileMapper.CreateDto(fileToDelete));
        }

        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> DeleteAllFiles()
        {
            var filesToDelete = await fileServices.GetAll(this.User.FindFirstValue(ClaimTypes.NameIdentifier));

            foreach (var file in filesToDelete)
            {
                await DeleteFile(file.Id);
            }

            return Ok(FileMapper.CreateListDto(filesToDelete));
        }

        /// <summary>
        /// Download file by it's identifier
        /// Sets useless files to delete by FilesCleaner
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/files/1")] 
        [AllowAnonymous]
        public async Task<IActionResult> DownloadFile([FromQuery] string id)
        {
            FileEntity file;
            try
            {
                file = await fileServices.GetOneById(id);
            }

            catch( InvalidOperationException )
            {
                return NotFound();
            }

            try
            {
                var objectResponse = await s3Service.DownloadFileFromS3(file.S3Name);

                if (file.DeleteAfterDownload)
                {
                    await fileServices.SetToDelete(file);
                }

                return File(objectResponse.ResponseStream, objectResponse.Headers.ContentType, file.Name);
            }

            catch(Exception exception)
            {
                return StatusCode(500, "{ Error Occured " + $"{exception.InnerException.Message}" + " }");
            }
        }
    }
}

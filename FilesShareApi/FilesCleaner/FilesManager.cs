using System.Threading;

namespace FilesShareApi.FilesCleaner
{

    /// <summary>
    /// This file manager is capable of deleting files, that were set to be deleted after download
    /// </summary>
    public class FilesManager
    {
        private readonly IFileService fileService;
        private readonly IS3Service s3Service;        

        public FilesManager(IFileService fileService, IS3Service s3Service)
        {
            this.fileService = fileService;
            this.s3Service = s3Service;
        }

        public void DeleteUselessFiles(CancellationToken cancellationToken)
        {
            var filesToDelete = fileService.GetFilesToDelete();

            foreach (var fileToDelete in filesToDelete)
            {
                fileService.DeleteFile(fileToDelete.Id, null);
                s3Service.DeleteFileFromS3(fileToDelete.S3Name);
            }
        }
    }
}

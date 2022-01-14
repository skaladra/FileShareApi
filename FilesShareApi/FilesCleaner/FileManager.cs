using System.Threading;
using System.Threading.Tasks;

namespace FilesShareApi.FilesCleaner
{

    /// <summary>
    /// This file manager is capable of deleting files, that were set to be deleted after download
    /// </summary>
    public class FileManager
    {
        private readonly IFileService fileService;
        private readonly IS3Service s3Service;        

        public FileManager(IFileService fileService, IS3Service s3Service)
        {
            this.fileService = fileService;
            this.s3Service = s3Service;
        }

        public async Task DeleteUselessFiles(CancellationToken cancellationToken)
        {
            var filesToDelete = await fileService.GetToDelete();

            foreach (var fileToDelete in filesToDelete)
            {
                await fileService .DeleteOne(fileToDelete.Id, null);
                await s3Service.DeleteFileFromS3(fileToDelete.S3Name);
            }
        }
    }
}

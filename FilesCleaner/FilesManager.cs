using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FilesShareApi.FilesCleaner
{

    /// <summary>
    /// This file manager is capable of deleting files, that were set to be deleted after download
    /// </summary>
    public class FilesManager
    {
        private readonly IFileService fileService;
        private readonly IAmazonS3 amazonS3;

        private readonly string bucketName = "secretsharingbucket";
        

        public FilesManager(IFileService fileService, IAmazonS3 amazonS3)
        {
            this.fileService = fileService;
            this.amazonS3 = amazonS3;
        }

        public async Task DeleteUselessFiles(CancellationToken cancellationToken)
        {

            var filesToDelete = fileService.GetFilesToDelete();

            foreach (var fileToDelete in filesToDelete)
            {
                try
                {
                    fileService.DeleteFile(fileToDelete.Id, null);
                    var fileTransferUtility = new TransferUtility(amazonS3);
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
            }


        }
    }
}

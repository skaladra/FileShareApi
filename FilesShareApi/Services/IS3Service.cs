using Amazon.S3.Model;
using System.Threading.Tasks;

namespace FilesShareApi
{
    public interface IS3Service
    {
        Task DeleteFileFromS3(string key);

        Task UploadFileToS3(byte[] file, string key);

        Task<GetObjectResponse> DownloadFileFromS3(string key);
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;

namespace FilesShareApi
{
    public interface IFileService
    {
        Task<List<FileEntity>> GetFiles(string id);
        FileEntity DeleteFile(string id, string creatorId);
        string AddFile(FileEntity file);

        FileEntity GetFileById(string id);

        List<FileEntity> GetFilesToDelete();

        void SetFileToDelete(FileEntity file);
    }
}

using System.Collections.Generic;

namespace FilesShareApi
{
    public interface IFileService
    {
        List<FileEntity> GetFiles(string id);
        FileEntity DeleteFile(string id, string creatorId);
        string AddFile(FileEntity file);

        FileEntity GetFileById(string id);

        List<FileEntity> GetFilesToDelete();

        void SetFileToDelete(FileEntity file);
    }
}

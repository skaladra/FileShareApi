using System.Collections.Generic;
using System.Threading.Tasks;

namespace FilesShareApi
{
    public interface IFileService
    {
        Task<List<FileEntity>> GetAll(string id);

        IEnumerable<FileEntity> GetAllByAdmin();

        FileEntity DeleteOneByAdmin(string id);

        FileEntity DeleteOne(string id, string creatorId);
        string AddOne(FileEntity file);

        FileEntity GetOneById(string id);

        List<FileEntity> GetToDelete();

        void SetToDelete(FileEntity file);
    }
}

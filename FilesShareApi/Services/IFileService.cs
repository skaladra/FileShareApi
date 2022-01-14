using System.Collections.Generic;
using System.Threading.Tasks;

namespace FilesShareApi
{
    public interface IFileService
    {
        Task<List<FileEntity>> GetAll(string id);

        IEnumerable<FileEntity> GetAllByAdmin();

        Task<FileEntity> DeleteOneByAdmin(string id);

        Task<FileEntity> DeleteOne(string id, string creatorId);
        Task<FileEntity> AddOne(FileEntity file);

        Task<FileEntity> GetOneById(string id);

        Task<List<FileEntity>> GetToDelete();

        Task SetToDelete(FileEntity file);
    }
}

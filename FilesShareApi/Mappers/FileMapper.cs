using System.Collections.Generic;

namespace FilesShareApi
{
    public static class FileMapper
    {
        public static FileDto CreateDto(FileEntity file)
        {
            return new FileDto()
            {
                CreatedTime = file.CreatedTimeUtc,
                Id = file.Id,
                Name = file.Name,
                Url = file.Url
            };
        }

        public static List<FileDto> CreateListDto(IEnumerable<FileEntity> files)
        {
            var fileList = new List<FileDto>();
            foreach (var file in files)
            {
                fileList.Add(CreateDto(file));
            }
            return fileList;
        }
    }
}

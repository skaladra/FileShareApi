using System.Collections.Generic;

namespace FilesShareApi
{
    public static class FileMapper
    {
        public static FileResponseDto CreateDto(FileEntity file)
        {
            return new FileResponseDto()
            {
                CreatedTime = file.CreatedTimeUtc,
                Id = file.Id,
                Name = file.Name,
                Url = file.Url
            };
        }

        public static List<FileResponseDto> CreateListDto(IEnumerable<FileEntity> files)
        {
            var fileList = new List<FileResponseDto>();
            foreach (var file in files)
            {
                fileList.Add(CreateDto(file));
            }
            return fileList;
        }
    }
}

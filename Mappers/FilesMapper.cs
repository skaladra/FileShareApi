﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FilesShareApi
{
    public static class FilesMapper
    {
        public static FileDto CreateDto(FileEntity file)
        {
            return new FileDto()
            {
                CreatedTime = file.CreatedTime,
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

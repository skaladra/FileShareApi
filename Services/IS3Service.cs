﻿using Amazon.S3.Model;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace FilesShareApi.Services
{
    public interface IS3Service
    {
        public void DeleteFileFromS3(string key);

        public void UploadFileToS3(IFormFile file, string key);

        public Task<GetObjectResponse> DownloadFileFromS3(string key);
    }
}
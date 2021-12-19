﻿using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace FilesShareApi.Services
{
    public class S3Service : IS3Service
    {
        private readonly TransferUtility fileTransferUtility;
        private readonly IAmazonS3 s3Client;
        private readonly string bucketName = "secretsharingbucket";

        public S3Service(IAmazonS3 s3Client)
        {
            this.s3Client = s3Client;
            fileTransferUtility = new TransferUtility(this.s3Client);
        }

        public async void DeleteFileFromS3(string key)
        {
            try
            {
                await fileTransferUtility.S3Client.DeleteObjectAsync(new DeleteObjectRequest()
                {
                    BucketName = bucketName,
                    Key = key
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

        public async Task<GetObjectResponse> DownloadFileFromS3(string key)
        {
            try
            {
                var objectResponse = await fileTransferUtility.S3Client.GetObjectAsync(new GetObjectRequest()
                {
                    BucketName = bucketName,
                    Key = key
                });

                return objectResponse;
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

        public async void UploadFileToS3(IFormFile file, string key)
        {   
            await using var newMemoryStream = new MemoryStream();
            await file.CopyToAsync(newMemoryStream);
            try
            {
                await fileTransferUtility.UploadAsync(new TransferUtilityUploadRequest
                {
                    InputStream = newMemoryStream,
                    Key = key,
                    BucketName = bucketName,
                    CannedACL = S3CannedACL.PublicRead,
                });
            }
            catch(AmazonS3Exception amazonS3Exception)
            {
                if (amazonS3Exception.ErrorCode != null
                    && (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId") || amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                {
                    throw new Exception("Check the provided AWS Credentials.");
                }
                else
                {
                    throw new Exception("Error occurred:" + amazonS3Exception.Message);
                }
            }

            
        }
    }
}

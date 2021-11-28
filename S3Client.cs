using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace SecretSharing
{
    public class S3Client : ControllerBase
    {
        private readonly IAmazonS3 amazonS3;

        public S3Client(IAmazonS3 amazonS3)
        {
            this.amazonS3 = amazonS3;
        }

        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            try
            {
                PutObjectRequest putRequest = new PutObjectRequest
                {
                    BucketName = "secretsharingbucket",
                    Key = file.FileName,
                    InputStream = file.OpenReadStream(),
                    ContentType = file.ContentType,
                };

                GetPreSignedUrlRequest request = new GetPreSignedUrlRequest
                {
                    BucketName = "secretsharingbucket",
                    Key = file.FileName,
                    Verb = HttpVerb.GET,
                    Expires = DateTime.Now.AddDays(7)
                };

                var url = amazonS3.GetPreSignedURL(request);

                PutObjectResponse response = await amazonS3.PutObjectAsync(putRequest);
                return Ok(url);
            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                if (amazonS3Exception.ErrorCode != null &&
                    (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId")
                    ||
                    amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                {
                    throw new Exception("Check the provided AWS Credentials.");
                }
                else
                {
                    throw new Exception("Error occurred: " + amazonS3Exception.Message);
                }
            }
        }
    }
}


using System;

namespace FilesShareApi.Entity
{
    /// <summary>
    /// A class to response not all file fields
    /// </summary>
    public class FileDto
    {

        public string Id { get; set; }

        public string Name { get; set; }

        public string Url { get; set; }

        public DateTime CreatedTime { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FilesShareApi
{
    public class MessageDto
    {
        public string Id { get; set; }
        public DateTime SentTime { get; set; }
        public string Text { get; set; }
        public string SentByName { get; set; }
        public string SentToName { get; set; }
    }
}

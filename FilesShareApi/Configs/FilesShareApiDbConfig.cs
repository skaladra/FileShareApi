namespace FilesShareApi
{
    public class FilesShareApiDbConfig
    {
        public string Database_Name { get; set; }

        public string Files_Collection_Name { get; set; }

        public string Users_Collection_Name { get; set; }

        public string Roles_Collections_Name { get; set; }

        public string Messages_Collection_Name { get; set; }

        public string Chats_Collection_Name { get; set; }

        /// <summary>
        /// string to connect to database
        /// </summary>
        public string Connection_String { get; set; }
    }
}

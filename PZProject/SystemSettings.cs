﻿namespace PZProject
{
    public class SystemSettings
    {
        public string DbConnectionString { get; set; }
        public string JwtSecurityKey { get; set; }
        public string StorageAccountConnectionString { get; set; }
        public string StorageAccountBlobContainerName { get; set; }
    }
}
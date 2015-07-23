using System;
using System.IO;

namespace PSXDH.Model
{
    public class LocalFile
    {
        public LocalFile(string filepath)
        {
            if (!File.Exists(filepath)) return;
            Filepath = filepath;
            FileStream = File.OpenRead(filepath);
            LastModified = File.GetLastWriteTime(filepath);
            Filesize = FileStream.Length;
        }

        public string Filepath { get; private set; }

        public long Filesize { get; private set; }

        public FileStream FileStream { get; private set; }

        public DateTime LastModified { get; private set; }
    }
}
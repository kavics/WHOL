using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Whol.Logic
{
    public interface IDisk
    {
        bool IsDirectoryExists(string path);
        void CreateDirectory(string path);
        bool IsFileExists(string path);
        Stream GetStreamForWrite(string path);
        Stream GetStreamForRead(string path);
    }
    public class FsDisk : IDisk
    {
        public bool IsDirectoryExists(string path)
        {
            return Directory.Exists(path);
        }
        public void CreateDirectory(string path)
        {
            Directory.CreateDirectory(path);
        }
        public bool IsFileExists(string path)
        {
            return File.Exists(path);
        }
        public Stream GetStreamForWrite(string path)
        {
            return new FileStream(path, FileMode.OpenOrCreate);
        }
        public Stream GetStreamForRead(string path)
        {
            return new FileStream(path, FileMode.Open);
        }
    }
}

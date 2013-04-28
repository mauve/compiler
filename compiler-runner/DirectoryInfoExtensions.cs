using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CompilerRunner
{
    public static class DirectoryInfoExtensions
    {
        public static Task<FileInfo[]> GetFilesAsync(this DirectoryInfo dir)
        {
            return Task<FileInfo[]>.Run(() =>
            {
                return dir.GetFiles();
            });
        }

        public static Task<FileInfo[]> GetFilesAsync(this DirectoryInfo dir, string path)
        {
            return Task<FileInfo[]>.Run(() =>
            {
                return dir.GetFiles(path);
            });
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace FileWatcher
{
    class Tools
    {
        public string LogFilePath { get; set; }
        public string SourcePath { get; set; }
        public bool NeedToEncrypt { get; set; }
        public string TargetPath { get; set; }
        public ArchiveTools Options { get; set; }
        public Tools() { }
        public Tools(string sourcePath, string targetPath, string logFilePath, ArchiveTools archiveOptions, bool needToEncrypt)
        {
            SourcePath = sourcePath;
            TargetPath = targetPath;
            LogFilePath = logFilePath;
            Options = archiveOptions;
            NeedToEncrypt = needToEncrypt;
        }
    }
}

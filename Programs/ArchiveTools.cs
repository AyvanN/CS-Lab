using System;
using System.Text;
using System.IO.Compression;

namespace FileWatcher
{
    class ArchiveTools
    {
        public ArchiveTools() { }

        public bool Compress { get; set; }
        public bool Archive { get; set; }
        public CompressionLevel Level { get; set; }

    }
}

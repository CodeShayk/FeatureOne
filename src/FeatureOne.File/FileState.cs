using System;

namespace FeatureOne.File
{
    internal class FileState
    {
        public FileRecord[] Records { get; set; } = Array.Empty<FileRecord>();
        public string Hash { get; set; }
    }
}
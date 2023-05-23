using System.Collections.Generic;
using System.Linq;

namespace FeatureOne.File
{
    public class FileRecord
    {
        public FileRecord(string name, string toggle)
        {
            Name = name;
            Toggle = toggle;
        }

        public string Name { get; set; }
        public string Toggle { get; set; }
    }
}
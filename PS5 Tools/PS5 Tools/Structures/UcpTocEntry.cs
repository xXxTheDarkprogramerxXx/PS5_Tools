using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PS5_Tools.Structures
{
    public class UcpTocEntry
    {
        public string FileName { get; set; }
        public long Offset { get; set; }
        public long Size { get; set; }
        public bool IsPng => Signature?.StartsWith("89504E47") == true;
        public bool IsJson => Signature?.StartsWith("{") == true || Signature?.StartsWith("[") == true;
        public string Signature { get; set; }
    }
}

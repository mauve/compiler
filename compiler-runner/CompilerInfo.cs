using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompilerRunner
{
    public class CompilerInfo
    {
        public string Name { get; set; }
        public string Vendor { get; set; }
        public string FullVersion { get; set; }
        public int Version { get; set; }
        public string Variant { get; set; }

        public override string ToString()
        {
            return Name + " " + FullVersion;
        }
    }
}

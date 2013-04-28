using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompilerRunner
{
    public class CompileResult
    {
        public CompilerInfo Compiler { get; set; }
        public bool Successful { get; set; }
        public string StdOut { get; set; }
        public string StdErr { get; set; }
        public int ExitCode { get; set; }
        public TimeSpan CompileTime { get; set; }

        public List<Message> Messages { get; set; }
    }
}

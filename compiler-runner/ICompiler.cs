using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompilerRunner
{
    public interface ICompiler
    {
        CompilerInfo Info { get; }

        Task<CompileResult> Compile(CompileRequest request);
    }
}

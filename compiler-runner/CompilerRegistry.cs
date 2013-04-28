using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompilerRunner
{
    class CompilerRegistry
    {
        private List<ICompiler> Compilers = new List<ICompiler>();

        public void Add(ICompiler compiler)
        {
            Compilers.Add(compiler);
        }

        public async Task<List<CompileResult>> Compile(CompileRequest snippet)
        {
            if (Compilers.Count == 0)
                throw new InvalidOperationException("No compilers registered");

            List<CompileResult> results = new List<CompileResult>();
            
            foreach(ICompiler compiler in Compilers)
            {
                results.Add(await compiler.Compile(snippet));
            }

            return results;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace CompilerRunner
{
    public class ConfiguredCompiler : ICompiler
    {
        private string ExecutablePath;
        private string Arguments;
        private IOutputParser OutputParser;

        public ConfiguredCompiler(string path, string arguments, CompilerInfo info,
                                    IOutputParser parser)
        {
            ExecutablePath = path;
            Arguments = arguments;
            Info = info;
            SourceEncoding = Encoding.ASCII;
            OutputParser = parser;
        }

        public Encoding SourceEncoding { get; set; }
        public CompilerInfo Info { get; private set; }

        public async Task<CompileResult> Compile(CompileRequest snippet)
        {
            // write code to temp file
            string path = await WriteCode(snippet);

            ProcessRunner runner = new ProcessRunner(ExecutablePath, String.Format(Arguments, "output.exe", path));
            ProcessResult process_result = await runner.Start();

            CompileResult result = new CompileResult();
            result.Successful = process_result.ExitCode == 0;

            var inputFiles = new Dictionary<string, string>();
            inputFiles.Add(path, "input001.cpp");

            result.StdOut = OutputParser.preprocess(inputFiles, process_result.StdOut);
            result.StdErr = OutputParser.preprocess(inputFiles, process_result.StdErr);
            result.ExitCode = process_result.ExitCode;
            result.CompileTime = process_result.Runtime;
            result.Messages = OutputParser.parse(result.StdErr);
            return result;
        }

        private async Task<string> WriteCode(CompileRequest snippet)
        {
            byte[] code = SourceEncoding.GetBytes(snippet.Code);
            string path = Path.GetTempPath() + Guid.NewGuid().ToString() + ".cpp";

            using (FileStream sourceStream = new FileStream(path,
                    FileMode.Append, FileAccess.Write, FileShare.None,
                    bufferSize: 4096, useAsync: true))
            {
                await sourceStream.WriteAsync(code, 0, code.Length);
            };

            return path;
        }
    }
}

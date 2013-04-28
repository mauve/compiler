using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompilerRunner
{
    [Serializable]
    class ProcessException : Exception
    {
        public ProcessException(string message, int code)
            : base(message)
        {
            ExitCode = code;
        }

        public ProcessException(string message, int code, Exception inner)
            : base(message, inner)
        {
            ExitCode = code;
        }

        public int ExitCode { get; private set; }
    }

    class ProcessResult
    {
        public TimeSpan Runtime { get; set; }
        public String StdOut { get; set; }
        public String StdErr { get; set; }
        public int ExitCode { get; set; }
    }

    class ProcessRunner
    {
        public TimeSpan Timeout { get; set; }
        public string Command { get; set; }
        public string Arguments { get; set; }
        public string WorkingDirectory { get; set; }

        public ProcessRunner(string command, string arguments)
        {
            Timeout = new TimeSpan(0, 0, 10);
            Command = command;
            Arguments = arguments;
            WorkingDirectory = Path.GetDirectoryName(command);
        }

        public async Task<ProcessResult> Start()
        {
            return await Start(Arguments);
        }

        public async Task<ProcessResult> Start(string arguments)
        {
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.CreateNoWindow = true;
            psi.UseShellExecute = false;
            psi.RedirectStandardInput = true;
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;
            psi.WorkingDirectory = WorkingDirectory;
            psi.LoadUserProfile = false;
            psi.FileName = Command;
            psi.Arguments = arguments;

            Stopwatch watch = new Stopwatch();
            watch.Start();

            Process process = new Process();
            process.StartInfo = psi;
            process.Start();

            Task<string> stdout = process.StandardOutput.ReadToEndAsync();
            Task<string> stderr = process.StandardError.ReadToEndAsync();

            Task kill = Task.Run(() =>
                {
                    if (!process.WaitForExit(10000))
                    {
                        process.Kill();
                    }
                });

            await Task.WhenAll(new Task[] { stdout, stderr, kill });
            watch.Stop();

            ProcessResult result = new ProcessResult();
            result.Runtime = watch.Elapsed;
            result.StdOut = stdout.Result;
            result.StdErr = stderr.Result;
            result.ExitCode = process.ExitCode;

            return result;
        }

        public static Task<ProcessResult> Run(string command, string args)
        {
            ProcessRunner runner = new ProcessRunner(command, args);
            return runner.Start();
        }
    }
}

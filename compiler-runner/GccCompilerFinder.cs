using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace CompilerRunner
{
    public class GccCompilerFinder
    {
        public static async Task<ICompiler> DetectAtPath(string Path)
        {
            ProcessResult result = await ProcessRunner.Run(Path, "--version");
            if (result.ExitCode != 0)
                throw new ProcessException("Non-successful return code when extracting version", result.ExitCode);

            Match extractVersion = Regex.Match(result.StdOut.FirstLine(),
                @"^[^ ]+ \(([^)]+)\) ([0-9]+)\.([0-9]+)\.([0-9]+)$");
            if (!extractVersion.Success)
                throw new Exception("Cannot extract version");

            CompilerInfo info = new CompilerInfo();
            info.FullVersion = result.StdOut;
            info.Name = "g++";
            info.Vendor = "gcc";

            info.Variant = extractVersion.Groups[1].ToString();
            int Major = int.Parse(extractVersion.Groups[2].ToString());
            int Minor = int.Parse(extractVersion.Groups[3].ToString());
            int Micro = int.Parse(extractVersion.Groups[4].ToString());
            info.Version = Micro + Minor * 1000 + Major * 1000 * 1000;

            RegexOutputParser parser = new RegexOutputParser();
            parser.Add(new RegexOutputParserConfig
            {
                Type = Message.MsgType.Error,
                ParseRegex = new Regex(@"(?<file>.*):(?<line>\d+):(?<column>\d+): error: (?<text>.*)")
            });
            parser.Add(new RegexOutputParserConfig
            {
                Type = Message.MsgType.Warning,
                ParseRegex = new Regex(@"(?<file>.*):(?<line>\d+):(?<column>\d+): warning: (?<text>.*)")
            });
            parser.Add(new RegexOutputParserConfig
            {
                Type = Message.MsgType.Notice,
                ParseRegex = new Regex(@"(?<file>.*):(?<line>\d+):(?<column>\d+): info: (?<text>.*)")
            });

            ConfiguredCompiler compiler = new ConfiguredCompiler(
                Path,
                "-o \"{0}\" -std=c++11 \"{1}\" -Wall -static-libgcc",
                info,
                parser);
            return compiler;
        }
    }
}

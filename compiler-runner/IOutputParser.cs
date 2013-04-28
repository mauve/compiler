using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompilerRunner
{
    public interface IOutputParser
    {
        string preprocess(Dictionary<string, string> InputFiles, string Output);
        List<Message> parse(string Output);
    }
}

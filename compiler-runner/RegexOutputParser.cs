using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CompilerRunner
{
    public class RegexOutputParserConfig
    {
        public Message.MsgType Type;
        public Regex ParseRegex;
    }

    public class RegexOutputParser : IOutputParser
    {
        private List<RegexOutputParserConfig> Configuration = new List<RegexOutputParserConfig>();

        public void Add(RegexOutputParserConfig config)
        {
            Configuration.Add(config);
        }

        public string preprocess(Dictionary<string, string> InputFiles, string Output)
        {
            StringBuilder builder = new StringBuilder();
            StringReader reader = new StringReader(Output);
            String line;
            while ((line = reader.ReadLine()) != null)
            {
                foreach (var input in InputFiles)
                {
                    if (line.IndexOf(input.Key) >= 0)
                    {
                        line = line.Replace(input.Key, input.Value);
                        break;
                    }
                }

                builder.AppendLine(line);
            }

            return builder.ToString();
        }

        public List<Message> parse(string Output)
        {
            if (Configuration.Count == 0)
                throw new InvalidOperationException("No configuration added to RegexOutputParser");

            List<Message> result = new List<Message>();
            StringReader reader = new StringReader(Output);
            String line;
            while ((line = reader.ReadLine()) != null)
            {
                Message msg = parseLine(line);
                if (msg != null)
                    result.Add(msg);
            }

            return result;
        }

        private Message parseLine(string line)
        {
            foreach (var config in Configuration)
            {
                Match match = config.ParseRegex.Match(line);
                if (match.Success)
                {
                    Message msg = new Message();
                    msg.Type = config.Type;
                    if (match.Groups["file"].Success)
                        msg.FileName = match.Groups["file"].Value;
                    if (match.Groups["line"].Success)
                        msg.Line = int.Parse(match.Groups["line"].Value);
                    if (match.Groups["column"].Success)
                        msg.Column = int.Parse(match.Groups["column"].Value);
                    msg.Text = match.Groups["text"].Value;
                    msg.Id = match.Groups["id"].Value;

                    return msg;
                }
            }

            return null;
        }
    }
}

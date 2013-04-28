using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompilerRunner
{
    public class Message
    {
        public enum MsgType { Error, Warning, Notice, Unknown };

        public MsgType Type { get; set; }
        public string FileName { get; set; }
        public int Line { get; set; }
        public int Column { get; set; }
        public string Text { get; set; }
        public string Id { get; set; }
    }
}

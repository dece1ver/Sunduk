using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sunduk.PWA.Infrastructure.Sequences
{
    public class HeaderSequence : Sequence
    {
        public Machine Machine { get; set; }
        public string DetailNumber { get; set; }
        public string DetailName { get; set; }
        public string Author { get; set; }
        public double DrawVersion { get; set; }
        public string ToolTable { get; set; }
        public override string Operation => $"{Templates.Operation.Header(Machine, DetailNumber, DetailName, Author, DrawVersion)}{ToolTable}";
        public override MachineType MachineType { get => MachineType.General; }

        public HeaderSequence(Machine machine, string number, string name, string author, double drawVersion, string toolTable)
        {
            Machine = machine;
            DetailNumber = number;
            DetailName = name;
            Author = author;
            DrawVersion = drawVersion;
            ToolTable = toolTable;
        }
    }
}

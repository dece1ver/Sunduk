using Sunduk.PWA.Infrastructure.Sequences.Base;
using Sunduk.PWA.Infrastructure.Time;
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
        public string DrawVersion { get; set; }
        public string ToolTable { get; set; }
        public TimeSpan FullTime { get; set; }
        public override OperationTime MachineTime => new(0, 0);
        public override string Operation => $"{Templates.Operation.Header(Machine, DetailNumber, DetailName, Author, DrawVersion, FullTime)}{ToolTable}";
        public override MachineType MachineType => MachineType.Any;

        public HeaderSequence(Machine machine, string number, string name, string author, string drawVersion, string toolTable)
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sunduk.PWA.Infrastructure.Sequences
{
    public class HeaderSequence : Sequence
    {
        public Machines Machine { get; set; }
        public string DetailNumber { get; set; }
        public string DetailName { get; set; }
        public double DrawVersion { get; set; }

        public override string Operation => Templates.Operation.Header(Machine, DetailNumber, DetailName, DrawVersion);

        public HeaderSequence(Machines machine, string number, string name, double drawVersion)
        {
            Machine = machine;
            DetailNumber = number;
            DetailName = name;
            DrawVersion = drawVersion;
        }
    }
}

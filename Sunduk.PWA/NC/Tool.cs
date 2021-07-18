using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sunduk.PWA.NC
{
    public abstract class Tool
    {
        public int Position { get; set; }
        public string Name { get; set; }
        public SequenceType Usage { get; set; }
    }
}

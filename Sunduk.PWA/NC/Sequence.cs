using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sunduk.PWA.NC
{
    public struct Sequence
    {
        public SequenceType Type { get; set; }
        public Tool Tool { get; set; }
        public string Name { get; set; }
        public string Operation { get; set; }
    }
}

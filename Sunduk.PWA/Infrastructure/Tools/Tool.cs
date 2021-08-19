using Sunduk.PWA.Infrastructure.Sequences;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sunduk.PWA.Infrastructure.Tools
{
    public abstract class Tool
    {
        public int Position { get; set; }
        public string Name { get; set; }
    }
}

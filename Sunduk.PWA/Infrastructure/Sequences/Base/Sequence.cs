using Sunduk.PWA.Infrastructure.Tools.Base;
using System;

namespace Sunduk.PWA.Infrastructure.Sequences.Base
{
    public class Sequence
    {
        public virtual string Name { get; set; }
        public virtual string Operation { get; set; }
        public virtual MachineType MachineType { get; set; }
    }
}

using System;

namespace Sunduk.PWA.Infrastructure.Sequences
{
    public class Sequence
    {
        public string Name { get; set; }
        public virtual string Operation { get; set; }
        public virtual MachineType MachineType { get; set; }
    }
}

using Sunduk.PWA.Infrastructure.Tools.Base;
using System;
using Sunduk.PWA.Infrastructure.Time;

namespace Sunduk.PWA.Infrastructure.Sequences.Base
{
    public abstract class Sequence
    {
        public abstract string Name { get; }
        public abstract string Operation { get; }
        public abstract OperationTime MachineTime { get; }
        public abstract MachineType MachineType { get; }
    }
}

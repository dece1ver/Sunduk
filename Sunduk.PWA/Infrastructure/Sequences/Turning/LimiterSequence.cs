﻿using Sunduk.PWA.Infrastructure.Sequences.Base;
using Sunduk.PWA.Infrastructure.Time;
using Sunduk.PWA.Infrastructure.Tools.Base;

namespace Sunduk.PWA.Infrastructure.Sequences.Turning
{
    public class LimiterSequence : Sequence
    {
        public Machine Machine { get; set; }
        public Tool Tool { get; set; }
        public double ExternalDiameter { get; set; }
        public override string Operation => Templates.Operation.Limiter(Machine, Tool, ExternalDiameter);
        public override MachineType MachineType => MachineType.Turning;
        public override OperationTime MachineTime => this.OperationTime();
        public override string Name => $"Упор";

        public LimiterSequence(Machine machine, Tool tool, double externalDiameter)
        {
            Machine = machine;
            Tool = tool;
            ExternalDiameter = externalDiameter;
        }
    }
}

using System;

namespace Sunduk.PWA.Infrastructure.Tools.Base
{
    public class TurningTool : Tool
    {
        public double Radius { get; set; }
        public double Angle { get; set; }

        public override MachineType MachineType => MachineType.Turning;

    }
}

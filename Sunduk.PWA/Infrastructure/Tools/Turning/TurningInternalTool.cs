using Sunduk.PWA.Infrastructure.Tools.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sunduk.PWA.Infrastructure.Tools.Turning
{
    public sealed class TurningInternalTool : TurningTool
    {
        public double Diameter { get; set; }

        public override string Name => "RAST";

        public override MachineType MachineType => MachineType.Turning;

        public TurningInternalTool(int position, double diameter, double angle, double radius, ToolHand hand = ToolHand.Right)
        {
            Position = position;
            Diameter = diameter;
            Radius = radius;
            Angle = angle;
            Hand = hand;
        }
    }
}

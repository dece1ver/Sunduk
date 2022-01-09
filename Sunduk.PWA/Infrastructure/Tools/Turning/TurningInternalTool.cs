using Sunduk.PWA.Infrastructure.Tools.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sunduk.PWA.Infrastructure.Tools.Turning
{
    public class TurningInternalTool : Tool
    {
        public double Diameter { get; set; }
        public double Radius { get; set; }
        public double Angle { get; set; }
        public override string Name => "RAST";

        public TurningInternalTool(int position, double diameter, double angle, double radius, ToolHand hand = ToolHand.Rigth)
        {
            Position = position;
            Diameter = diameter;
            Radius = radius;
            Angle = angle;
            Hand = hand;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sunduk.PWA.Infrastructure.Tools
{
    public class GroovingInternalTool : Tool
    {
        public enum Point { Left, Right }

        public double Diameter { get; set; }
        public double Width { get; set; }
        public Point ZeroPoint { get; set; }
        public override string Name => "KANAVA";

        public GroovingInternalTool(int position, double diameter, double width, Point zeroPoint)
        {
            Position = position;
            Diameter = diameter;
            Width = width;
            ZeroPoint = zeroPoint;
        }
    }
}

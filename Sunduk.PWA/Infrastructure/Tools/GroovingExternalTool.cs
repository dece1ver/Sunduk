using Sunduk.PWA.Infrastructure.Sequences;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sunduk.PWA.Infrastructure.Tools
{
    public class GroovingExternalTool : Tool
    {
        public enum Types { Grooving, Cutting }

        public Types Type { get; set; }
        public double Width { get; set; }
        public enum Point { Left, Right }
        public Point ZeroPoint { get; set; }
        public override string Name { get => Type == Types.Grooving ? "KANAVA" : "OTR"; }

        public GroovingExternalTool(int position, Types type, double width, Point zeroPoint)
        {
            Position = position;
            Type = type;
            Width = width;
            ZeroPoint = zeroPoint;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sunduk.PWA.NC
{
    public class GroovingTool : Tool
    {
        public double Width { get; set; }
        public enum Point { Left, Right }
        public Point ZeroPoint { get; set; }
        public GroovingTool(int position, string name, double width, Point zeroPoint)
        {
            Usage = SequenceType.Grooving;
            Position = position;
            Name = name;
            Width = width;
            ZeroPoint = zeroPoint;
        }
    }
}

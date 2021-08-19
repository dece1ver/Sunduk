using Sunduk.PWA.Infrastructure.Sequences;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sunduk.PWA.Infrastructure.Tools
{
    public class GroovingExternalTool : Tool
    {
        public double Width { get; set; }
        public enum Point { Left, Right }
        public Point ZeroPoint { get; set; }

        public GroovingExternalTool(int position, string name, double width, Point zeroPoint)
        {
            Position = position;
            Name = name;
            Width = width;
            ZeroPoint = zeroPoint;
        }
    }
}

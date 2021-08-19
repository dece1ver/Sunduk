using Sunduk.PWA.Infrastructure.Sequences;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sunduk.PWA.Infrastructure.Tools
{
    public class DrillingTool : Tool
    {
        public double Diameter { get; set; }
        public double Angle { get; set; }
        public DrillingTool(int position, string name, double diameter, double angle)
        {
            Position = position;
            Name = name;
            Diameter = diameter;
            Angle = angle;
        }
    }
}

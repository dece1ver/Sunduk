using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sunduk.PWA.NC
{
    public class DrillingTool : Tool
    {
        public double Diameter { get; set; }
        public double Angle { get; set; }
        public DrillingTool(int position, string name, double diameter, double angle)
        {
            Usage = SequenceType.Drilling;
            Position = position;
            Name = name;
            Diameter = diameter;
            Angle = angle;
        }
    }
}

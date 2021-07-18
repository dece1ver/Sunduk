using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sunduk.PWA.NC
{
    public class TurningTool : Tool
    {
        public double Raduis { get; set; }
        public double Angle { get; set; }

        public TurningTool(int position, string name, double angle, double raduis)
        {
            Usage = SequenceType.Turning;
            Position = position;
            Name = name;
            Raduis = raduis;
            Angle = angle;
        }
    }
}

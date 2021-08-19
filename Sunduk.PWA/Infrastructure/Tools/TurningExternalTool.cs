using Sunduk.PWA.Infrastructure.Sequences;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sunduk.PWA.Infrastructure.Tools
{
    public class TurningExternalTool : Tool
    {
        public double Radius { get; set; }
        public double Angle { get; set; }

        public TurningExternalTool(int position, string name, double angle, double radius)
        {
            Position = position;
            Name = name;
            Radius = radius;
            Angle = angle;
        }
    }
}

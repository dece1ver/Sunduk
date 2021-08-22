using Sunduk.PWA.Infrastructure.Sequences;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sunduk.PWA.Infrastructure.Tools
{
    public class TurningExternalTool : Tool
    {
        public enum Types { Face, Bar }

        public Types Type { get; set; }
        public double Radius { get; set; }
        public double Angle { get; set; }
        public override string Name => Type == Types.Face ? "TORC" : "PROHOD";

        public TurningExternalTool(int position, Types type, double angle, double radius)
        {
            Position = position;
            Type = type;
            Radius = radius;
            Angle = angle;
        }
    }
}

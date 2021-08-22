using Sunduk.PWA.Infrastructure.Sequences;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sunduk.PWA.Infrastructure.Tools
{
    public class DrillingTool : Tool
    {
        public enum Types { Insert, Solid, Tip, Center, HSS }

        public Types Type { get; set; }
        public double Diameter { get; set; }
        public double Angle { get; set; }
        public override string Name { get => Type switch
        {
            Types.Insert => "KORP",
            Types.Solid => "TV",
            Types.Tip => "GOLOVKA",
            Types.Center => "CENTR",
            Types.HSS => "HSS",
            _ => string.Empty,
        }; }

        public DrillingTool(int position, Types type, double diameter, double angle)
        {
            Position = position;
            Type = type;
            Diameter = diameter;
            Angle = angle;
        }
    }
}

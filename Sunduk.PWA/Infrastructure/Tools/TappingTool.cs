using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sunduk.PWA.Infrastructure.Tools
{
    public class TappingTool : Tool
    {
        public enum Types { Forming, Cutting }

        public Types Type { get; set; }
        public double Diameter { get; }
        public double Pitch { get; }
        public override string Name { get => Type == Types.Forming ? "RASKATNIK" : "METCHIK"; }

        public TappingTool(int position, Types type, double diameter, double pitch)
        {
            Position = position;
            Type = type;
            Diameter = diameter;
            Pitch = pitch;
        }
        
    }
}

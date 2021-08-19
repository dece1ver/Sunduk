using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sunduk.PWA.Infrastructure.Tools
{
    public class TappingTool : Tool
    {
        public double Diameter { get; }
        public double Pitch { get; }

        public TappingTool(int position, string name, double diameter, double pitch)
        {
            Position = position;
            Name = name;
            Diameter = diameter;
            Pitch = pitch;
        }
        
    }
}

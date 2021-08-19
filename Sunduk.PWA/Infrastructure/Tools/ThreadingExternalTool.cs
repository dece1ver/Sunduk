using Sunduk.PWA.Infrastructure.Sequences;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sunduk.PWA.Infrastructure.Tools
{
    public class ThreadingExternalTool : Tool
    {
        public double Pitch { get; set; }
        public double Angle { get; set; }

        public ThreadingExternalTool(int position, string name, double pitch, double angle)
        {
            Position = position;
            Name = name;
            Pitch = pitch;
            Angle = angle;
        }
    }
}

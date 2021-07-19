using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sunduk.PWA.NC
{
    public class ThreadingTool : Tool
    {
        public double Pitch { get; }
        public double Angle { get; }

        public ThreadingTool(int position, string name, double pitch, double angle)
        {
            Position = position;
            Name = name;
            
            Usage = SequenceType.Threading;
            Pitch = pitch;
            Angle = angle;
        }
    }
}

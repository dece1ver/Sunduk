﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sunduk.PWA.Infrastructure.Tools
{
    public class ThreadingInternalTool : Tool
    {
        public double Diameter { get; set; }
        public double Pitch { get; set; }
        public double Angle { get; set; }
        public override string Name => "KANAVA";

        public ThreadingInternalTool(int position, double diameter, double pitch, double angle)
        {
            Position = position;
            Diameter = diameter;
            Pitch = pitch;
            Angle = angle;
        }
    }
}
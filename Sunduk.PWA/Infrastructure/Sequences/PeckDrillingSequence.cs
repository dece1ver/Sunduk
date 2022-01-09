﻿using Sunduk.PWA.Infrastructure.Tools;
using Sunduk.PWA.Infrastructure.Tools.Turning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sunduk.PWA.Infrastructure.Sequences
{
    public class PeckDrillingSequence : Sequence
    {
        public Machine Machine { get; set; }
        public Material Material { get; set; }
        public TurningDrillingTool Tool { get; set; }
        public double Depth { get; set; }
        public double StartZ { get; set; }
        public double EndZ { get; set; }
        public override string Operation => Templates.Operation.PeckDrilling(Machine, Material, Tool, Depth, StartZ, EndZ);

        public PeckDrillingSequence(Machine machine, Material material, TurningDrillingTool tool, double depth, double startZ, double endZ)
        {
            Machine = machine;
            Material = material;
            Tool = tool;
            Depth = depth;
            StartZ = startZ;
            EndZ = endZ;
        }
    }
}

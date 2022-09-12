using Sunduk.PWA.Infrastructure.Tools.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sunduk.PWA.Infrastructure.Tools.Milling
{
    public sealed class MillingBoreTool : Tool
    {
        public double Diameter { get; set; }
        public double CuttingLength { get; set; }
        public double Radius { get; set; }
        public override string Name => $"RAST D{Diameter.ToPrettyString()} R{Radius.ToPrettyString()}";

        public override MachineType MachineType => MachineType.Milling;


        public MillingBoreTool(int position, double diameter, double cuttingLength = 0, double radius = -1, ToolHand hand = ToolHand.Right)
        {
            Position = position;
            Diameter = diameter;
            CuttingLength = cuttingLength == 0 ? Math.Round(diameter * 3) : cuttingLength;
            Radius = radius < 0 ? 0.2 : radius;
            Hand = hand;
        }
    }
}

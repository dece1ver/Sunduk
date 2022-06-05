using Sunduk.PWA.Infrastructure.Tools.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sunduk.PWA.Infrastructure.Tools.Milling
{
    public sealed class MillingTool : Tool
    {
        public enum Types { Insert, Solid }
        public Types Type { get; set; }
        public double Diameter { get; set; }
        public double CuttingLength { get; set; }
        public int Edges { get; set; }
        public double CornerRadius { get; set; }
        public override string Name =>
            Type switch
            {
                Types.Insert => "FREZA KORP",
                Types.Solid => "FREZA TV",
                _ => string.Empty,
            };

        public override MachineType MachineType => MachineType.Milling;


        public MillingTool(int position, Types type, double diameter, double cuttingLength = 0, int edges = 4, double cornerRadius = 0, ToolHand hand = ToolHand.Right)
        {
            Position = position;
            Type = type;
            Diameter = diameter;
            Edges = edges;
            CuttingLength = cuttingLength == 0 ? (type == Types.Insert ? 10 : Math.Round(diameter * 3)) : cuttingLength;
            CornerRadius = cornerRadius == 0 ? (type == Types.Insert ? 0.8 : 0.2) : cornerRadius;
            Hand = hand;
        }
    }
}

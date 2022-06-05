using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sunduk.PWA.Infrastructure.Tools.Base
{
    public class TappingTool : Tool
    {
        public enum Types { Forming, Cutting }

        public Types Type { get; set; }
        public double Diameter { get; set; }
        public double Pitch { get; set; }
        public override string Name => Type == Types.Forming ? "RASKATNIK" : "METCHIK";

        public TappingTool(int position, Types type, double diameter, double pitch, ToolHand hand = ToolHand.Right)
        {
            Position = position;
            Type = type;
            Diameter = diameter;
            Pitch = pitch;
            Hand = hand;
        }
    }
}

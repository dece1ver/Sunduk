using Sunduk.PWA.Infrastructure.Tools.Base;

namespace Sunduk.PWA.Infrastructure.Tools.Turning
{
    public class TurningTappingTool : Tool
    {
        public enum Types { Forming, Cutting }

        public Types Type { get; set; }
        public double Diameter { get; set; }
        public double Pitch { get; set; }
        public override string Name { get => Type == Types.Forming ? "RASKATNIK" : "METCHIK"; }

        public TurningTappingTool(int position, Types type, double diameter, double pitch, ToolHand hand = ToolHand.Rigth)
        {
            Position = position;
            Type = type;
            Diameter = diameter;
            Pitch = pitch;
            Hand = hand;
        }
    }
}

namespace Sunduk.PWA.Infrastructure.Tools
{
    public class TappingTool : Tool
    {
        public enum Types { Forming, Cutting }

        public Types Type { get; set; }
        public double Diameter { get; }
        public double Pitch { get; }
        public override string Name { get => Type == Types.Forming ? "RASKATNIK" : "METCHIK"; }

        public TappingTool(int position, Types type, double diameter, double pitch, ToolHand hand = ToolHand.Rigth)
        {
            Position = position;
            Type = type;
            Diameter = diameter;
            Pitch = pitch;
            Hand = hand;
        }
    }
}

namespace Sunduk.PWA.Infrastructure.Tools
{
    public class DrillingTool : Tool
    {
        public enum Types { Insert, Solid, Tip, Center, HSS }

        public Types Type { get; set; }
        public double Diameter { get; set; }
        public double Angle { get; set; }
        public override string Name
        {
            get => Type switch
            {
                Types.Insert => "SV KORP",
                Types.Solid => "SV TV",
                Types.Tip => "SV GOLOVKA",
                Types.Center => "CENTR",
                Types.HSS => "SV HSS",
                _ => string.Empty,
            };
        }

        public DrillingTool(int position, Types type, double diameter, double angle, ToolHand hand = ToolHand.Rigth)
        {
            Position = position;
            Type = type;
            Diameter = diameter;
            Angle = angle;
            Hand = hand;
        }
    }
}

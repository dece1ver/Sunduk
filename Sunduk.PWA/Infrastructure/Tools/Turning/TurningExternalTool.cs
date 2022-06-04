using Sunduk.PWA.Infrastructure.Tools.Base;

namespace Sunduk.PWA.Infrastructure.Tools.Turning
{
    public class TurningExternalTool : Tool
    {
        public enum Types { Face, Bar }

        public Types Type { get; set; }
        public double Radius { get; set; }
        public double Angle { get; set; }
        public override string Name => Type == Types.Face ? "TORC" : "PROHOD";

        public override MachineType MachineType => MachineType.Turning;

        public TurningExternalTool(int position, Types type, double angle, double radius, ToolHand hand = ToolHand.Rigth)
        {
            Position = position;
            Type = type;
            Radius = radius;
            Angle = angle;
            Hand = hand;
        }
    }
}

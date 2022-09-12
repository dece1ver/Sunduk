using Sunduk.PWA.Infrastructure.Tools.Base;

namespace Sunduk.PWA.Infrastructure.Tools.Turning
{
    public sealed class TurningExternalTool : TurningTool
    {
        public enum Types { Face, Bar }

        public Types Type { get; set; }
        public override string Name => Type == Types.Face ? "TORC" : "PROHOD";

        public override MachineType MachineType => MachineType.Turning;

        public TurningExternalTool(int position, Types type, double angle, double radius, ToolHand hand = ToolHand.Right)
        {
            Position = position;
            Type = type;
            Angle = angle;
            Radius = radius;
            Type = type;
            Hand = hand;
        }
    }
}

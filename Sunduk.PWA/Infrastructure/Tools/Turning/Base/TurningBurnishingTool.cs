using Sunduk.PWA.Infrastructure.Tools.Base;

namespace Sunduk.PWA.Infrastructure.Tools.Turning.Base
{
    public class TurningBurnishingTool : Tool
    {
        public override MachineType MachineType => MachineType.Turning;
        public enum Types { Diamond, Roller }
        public Types Type { get; set; }
        public override string Name => "NAKATKA " + (Type is Types.Diamond ? "ALMAZ" : "ROLIK");

        public TurningBurnishingTool(int position, Types type, ToolHand hand = ToolHand.Right)
        {
            Position = position;
            Type = type;
            Hand = hand;
        }
    }
}

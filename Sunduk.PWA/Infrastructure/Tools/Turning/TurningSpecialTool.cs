using Sunduk.PWA.Infrastructure.Tools.Base;

namespace Sunduk.PWA.Infrastructure.Tools.Turning
{
    public sealed class TurningSpecialTool : Tool
    {
        public override MachineType MachineType => MachineType.Turning;

        public TurningSpecialTool(int position, string name, ToolHand hand = ToolHand.Right)
        {
            Position = position;
            Name = name;
            Hand = hand;
        }
    }
}

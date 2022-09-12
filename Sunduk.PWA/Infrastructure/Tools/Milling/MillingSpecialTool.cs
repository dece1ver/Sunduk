using Sunduk.PWA.Infrastructure.Tools.Base;

namespace Sunduk.PWA.Infrastructure.Tools.Milling
{
    public sealed class MillingSpecialTool : Tool
    {
        public override MachineType MachineType => MachineType.Milling;

        public MillingSpecialTool(int position, string name, ToolHand hand = ToolHand.Right)
        {
            Position = position;
            Name = name;
            Hand = hand;
        }
    }
}

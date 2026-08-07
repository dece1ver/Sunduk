using Sunduk.PWA.Infrastructure.Tools.Base;

namespace Sunduk.PWA.Infrastructure.Tools.Milling
{
    public sealed class MillingSpecialTool : Tool
    {
        public override MachineType MachineType => MachineType.Milling;
        public override string Description(Util.ToolDescriptionOption option) => option switch
        {
            Util.ToolDescriptionOption.General => $"T{Position:D2} ({Name})",
            Util.ToolDescriptionOption.MillingToolChange => $"T{Position} M6 ({Name})",
            Util.ToolDescriptionOption.ToolTable => Description(Util.ToolDescriptionOption.General).Split('(')[1].TrimEnd(')'),
            _ => string.Empty,
        };

        public MillingSpecialTool(int position, string name, ToolHand hand = ToolHand.Right)
        {
            Position = position;
            Name = name;
            Hand = hand;
        }
    }
}

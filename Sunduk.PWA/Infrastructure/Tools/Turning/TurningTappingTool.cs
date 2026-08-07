using Sunduk.PWA.Infrastructure.Sequences;
using Sunduk.PWA.Infrastructure.Tools.Base;

namespace Sunduk.PWA.Infrastructure.Tools.Turning
{
    public sealed class TurningTappingTool : TappingTool
    {
        public override MachineType MachineType => MachineType.Turning;
        public override string Description(Util.ToolDescriptionOption option) => option switch
        {
            Util.ToolDescriptionOption.General => $"T{Position.ToolNumber()} ({Name})",
            Util.ToolDescriptionOption.L230 => $"T{Position.ToolNumber()} ({Name})",
            Util.ToolDescriptionOption.GoodwayLeft => $"T{Position.ToolNumber()} G54 M58 ({Name})",
            Util.ToolDescriptionOption.GoodwayRight => $"T{Position.ToolNumber()} G55 M58 ({Name})",
            Util.ToolDescriptionOption.ToolTable => Description(Util.ToolDescriptionOption.General).Split('(')[1].TrimEnd(')'),
            _ => string.Empty,
        };

        public TurningTappingTool(int position, Types type, double diameter, double pitch, ThreadStandard threadStandard, string standardTemplate = "", ToolHand hand = ToolHand.Right)
            :base(position, type, diameter, pitch, threadStandard, standardTemplate, hand)
        {
            Position = position;
            Type = type;
            Diameter = diameter;
            Pitch = pitch;
            ThreadStandard = threadStandard;
            StandardTemplate = standardTemplate;
            Hand = hand;
        }
    }
}

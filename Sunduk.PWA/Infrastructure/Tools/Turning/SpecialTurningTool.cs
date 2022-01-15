using Sunduk.PWA.Infrastructure.Tools.Base;

namespace Sunduk.PWA.Infrastructure.Tools.Turning
{
    public class SpecialTurningTool : Tool
    {
        public SpecialTurningTool(int position, string name, ToolHand hand = ToolHand.Rigth)
        {
            Position = position;
            Name = name;
            Hand = hand;
        }
    }
}

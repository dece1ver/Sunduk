namespace Sunduk.PWA.Infrastructure.Tools
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

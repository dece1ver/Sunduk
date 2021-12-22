namespace Sunduk.PWA.Infrastructure.Tools
{
    public class SpecialTool : Tool
    {
        public SpecialTool(int position, string name, ToolHand hand = ToolHand.Rigth)
        {
            Position = position;
            Name = name;
            Hand = hand;
        }
    }
}

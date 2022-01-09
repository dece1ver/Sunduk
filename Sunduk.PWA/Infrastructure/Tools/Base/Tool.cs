namespace Sunduk.PWA.Infrastructure.Tools.Base
{
    public class Tool
    {
        public enum ToolHand { Rigth, Left }
        public int Position { get; set; }
        public virtual string Name { get; set; }
        public virtual ToolHand Hand { get; set; }
    }
}

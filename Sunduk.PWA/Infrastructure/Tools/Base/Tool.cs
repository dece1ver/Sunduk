namespace Sunduk.PWA.Infrastructure.Tools.Base
{
    public class Tool
    {
        public enum ToolHand { Right, Left }
        public int Position { get; set; }
        public virtual string Name { get; set; }
        public virtual ToolHand Hand { get; set; }

        public virtual MachineType MachineType { get; }
    }
}

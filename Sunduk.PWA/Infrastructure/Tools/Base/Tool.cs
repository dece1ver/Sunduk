namespace Sunduk.PWA.Infrastructure.Tools.Base
{
    public abstract class Tool
    {
        public enum ToolHand { Right, Left }
        public int Position { get; set; }
        public virtual string Name { get; set; }
        public virtual ToolHand Hand { get; set; }

        public abstract MachineType MachineType { get; }
        public abstract string Description(Util.ToolDescriptionOption option);
    }
}

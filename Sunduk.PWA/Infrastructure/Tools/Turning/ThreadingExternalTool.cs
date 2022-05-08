using Sunduk.PWA.Infrastructure.Tools.Base;
using Sunduk.PWA.Infrastructure.Tools.Turning.Base;

namespace Sunduk.PWA.Infrastructure.Tools.Turning
{
    public class ThreadingExternalTool : ThreadingTool
    {
        public override MachineType MachineType { get => MachineType.Turning; }
        public ThreadingExternalTool(int position, double pitch, double angle, ToolHand hand = ToolHand.Rigth) 
            : base(position, pitch, angle, hand)
        {
        }
    }
}

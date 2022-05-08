using Sunduk.PWA.Infrastructure.Tools.Base;
using Sunduk.PWA.Infrastructure.Tools.Turning.Base;

namespace Sunduk.PWA.Infrastructure.Tools.Turning
{
    public class GroovingExternalTool : TurningGroovingTool
    {
        public enum Types { Grooving, Cutting, Blade }
        public Types Type { get; set; }
        public override string Name
        {
            get => Type switch
            {
                Types.Grooving => "KANAVA",
                Types.Cutting => "OTR",
                Types.Blade => "LEZVIE",
                _ => string.Empty,
            };
        }

        public override MachineType MachineType { get => MachineType.Turning; } 

        public GroovingExternalTool(
            int position, 
            Types type, 
            double width,
            Point zeroPoint, 
            ToolHand hand = ToolHand.Rigth, 
            double cornerRadius = 0.2) 
            : base (position, width, zeroPoint, hand, cornerRadius)
        {
            Type = type;
        }
    }
}

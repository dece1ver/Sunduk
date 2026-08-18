using Sunduk.PWA.Components.Stuff;

namespace Sunduk.PWA.Infrastructure.State
{
    /// <summary>
    /// Состояние DrillAngleComponent на время жизни приложения (singleton в WASM).
    /// </summary>
    public class DrillAngleState
    {
        public DrillAngleComponent.Images CurrentImage { get; set; } = DrillAngleComponent.Images.Drill;
        public string DrillDiameter { get; set; }
        public string DrillAngle { get; set; }
    }
}

using Sunduk.PWA.Components.Stuff;

namespace Sunduk.PWA.Infrastructure.State
{
    /// <summary>
    /// Состояние HoleCapacityComponent на время жизни приложения (singleton в WASM).
    /// </summary>
    public class HoleCapacityState
    {
        public bool CalcToolSize { get; set; }
        public HoleCapacityComponent.Images CurrentImage { get; set; } = HoleCapacityComponent.Images.Base;
        public HoleCapacityComponent.Radius RadiusType { get; set; } = HoleCapacityComponent.Radius.External;

        public string HoleDiameter { get; set; } = string.Empty;
        public string ShankDiameter { get; set; } = string.Empty;
        public string ToolSize { get; set; }
        public string MinimalDiameter { get; set; }
    }
}

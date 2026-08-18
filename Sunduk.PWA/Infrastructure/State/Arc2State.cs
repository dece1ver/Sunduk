using Sunduk.PWA.Components.Stuff;

namespace Sunduk.PWA.Infrastructure.State
{
    /// <summary>
    /// Состояние Arc2Component на время жизни приложения (singleton в WASM).
    /// </summary>
    public class Arc2State
    {
        public bool RoundCorner { get; set; }
        public Arc2Component.Images CurrentImage { get; set; } = Arc2Component.Images.Base;
        public Arc2Component.Radius RadiusType { get; set; } = Arc2Component.Radius.External;

        public string PartDiameter { get; set; } = string.Empty;
        public string RadiusSize { get; set; } = string.Empty;
        public string StartDiameter { get; set; } = string.Empty;
        public string InsertRadius { get; set; } = string.Empty;
        public string Blunt { get; set; } = "0.3";
    }
}

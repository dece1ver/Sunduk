using Sunduk.PWA.Components.Stuff;

namespace Sunduk.PWA.Infrastructure.State
{
    /// <summary>
    /// Состояние NippleComponent на время жизни приложения (singleton в WASM).
    /// </summary>
    public class NippleState
    {
        public bool RoundCorner { get; set; }
        public NippleComponent.Images CurrentImage { get; set; } = NippleComponent.Images.NippleBase;
        public NippleComponent.Radius RadiusType { get; set; } = NippleComponent.Radius.External;

        public string NippleDiameter { get; set; } = string.Empty;
        public string ArcCenter { get; set; } = string.Empty;
        public string RadiusSize { get; set; } = string.Empty;
        public string InsertRadius { get; set; } = string.Empty;
        public string NippleBlunt { get; set; } = "0.3";
    }
}

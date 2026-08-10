namespace Sunduk.PWA.Infrastructure.State
{
    /// <summary>
    /// Состояние AngleConverterComponent на время жизни приложения (singleton в WASM),
    /// чтобы уход на другую страницу и обратно не сбрасывал введённые значения.
    /// </summary>
    public class AngleConverterState
    {
        public bool CalcDecimal { get; set; }
        public bool InputRadians { get; set; }

        public decimal DecimalAngle { get; set; }
        public decimal RadiansAngle { get; set; }

        public int IntAngle { get; set; }
        public int MinAngle { get; set; }
        public decimal SecAngle { get; set; }

        public string DecimalAngleString { get; set; } = string.Empty;
        public string RadiansAngleString { get; set; } = string.Empty;
        public string IntAngleString { get; set; } = string.Empty;
        public string MinAngleString { get; set; } = string.Empty;
        public string SecAngleString { get; set; } = string.Empty;
    }
}

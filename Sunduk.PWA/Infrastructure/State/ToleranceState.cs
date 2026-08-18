namespace Sunduk.PWA.Infrastructure.State
{
    /// <summary>
    /// Состояние ToleranceComponent на время жизни приложения (singleton в WASM).
    /// </summary>
    public class ToleranceState
    {
        /// <summary>Номинальный размер как строка (мм, запятая как десятичный разделитель).</summary>
        public string Nominal { get; set; } = string.Empty;

        /// <summary>Квалитет IT (5–14).</summary>
        public int? Grade { get; set; } = 7;

        /// <summary>Поле допуска (буква).</summary>
        public string? Field { get; set; } = "H";

        /// <summary>Вал (false) или отверстие (true).</summary>
        public bool IsHole { get; set; } = true;
    }
}

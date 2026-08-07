using System.Text;

namespace Sunduk.PWA.Infrastructure.Templates
{
    /// <summary>
    /// Заменяет цепочки "строка1 + \n + строка2 + \n + ..." в *Operation.cs на читаемую
    /// последовательность вызовов. <see cref="Line"/> сам добавляет перевод строки —
    /// передавайте текст без завершающего \n. <see cref="Raw"/> добавляет текст как есть,
    /// без своего \n — для констант вроде <see cref="Operation.TurningReferentPoint"/>,
    /// которые уже заканчиваются переводом строки.
    /// </summary>
    public class GCodeBuilder
    {
        private readonly StringBuilder _sb = new();

        public GCodeBuilder Raw(string text)
        {
            _sb.Append(text);
            return this;
        }

        public GCodeBuilder Line(string text)
        {
            _sb.Append(text).Append('\n');
            return this;
        }

        public GCodeBuilder LineIf(bool condition, string text) => condition ? Line(text) : this;

        public override string ToString() => _sb.ToString();
    }
}

using System;

namespace Sunduk.PWA.Infrastructure.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Проверяет, начинается ли строка с символа, удовлетворяющего предикату.
        /// </summary>
        public static bool StartsWith(this string str, Func<char, bool> predicate)
        {
            return !string.IsNullOrEmpty(str) && predicate(str[0]);
        }

        /// <summary>
        /// Проверяет, заканчивается ли строка символом, удовлетворяющим предикату.
        /// </summary>
        public static bool EndsWith(this string str, Func<char, bool> predicate)
        {
            return !string.IsNullOrEmpty(str) && predicate(str[^1]);
        }

        /// <summary>
        /// Удаляет все начальные вхождения указанной подстроки из текущей строки.
        /// </summary>
        /// <param name="source">Исходная строка.</param>
        /// <param name="prefix">Подстрока, которую нужно удалить с начала строки.</param>
        /// <param name="comparisonType">Тип сравнения (по умолчанию — Ordinal).</param>
        /// <returns>Строка без начальных вхождений <paramref name="prefix"/>.</returns>
        public static string TrimStart(this string source, string prefix, StringComparison comparisonType = StringComparison.Ordinal)
        {
            if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(prefix))
                return source;

            while (source.StartsWith(prefix, comparisonType))
            {
                source = source[prefix.Length..];
            }

            return source;
        }
    }
}

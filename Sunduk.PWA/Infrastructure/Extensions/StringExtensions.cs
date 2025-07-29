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
    }
}

using System;
using System.Collections.Generic;
using System.Linq;

namespace Sunduk.PWA.Infrastructure.Tolerances
{
    /// <summary>
    /// Допуски и посадки по ГОСТ 25346-89 / ISO 286 (система ЕСДП).
    /// Значения квалитетов (IT) и основных отклонений взяты из опубликованных
    /// таблиц ISO 286-2 / ГОСТ 25347 (сверено с таблицами RoyMech).
    /// Диапазон номинальных размеров — до 500 мм, квалитеты IT5–IT14
    /// (IT4 хранится только для правила Δ при переходных/натяг-посадках отверстий).
    /// </summary>
    public static class Tolerance
    {
        /// <summary>Максимальный поддерживаемый номинальный размер, мм.</summary>
        public const double MaxNominal = 500.0;

        /// <summary>Квалитеты, доступные пользователю.</summary>
        public static readonly int[] Grades = Enumerable.Range(5, 10).ToArray(); // 5..14

        /// <summary>Поля допуска вала (строчные).</summary>
        public static readonly string[] ShaftLetters =
        {
            "a", "b", "c", "cd", "d", "e", "ef", "f", "fg", "g", "h",
            "js", "j",
            "k", "m", "n", "p", "r", "s", "t", "u", "v", "x", "y", "z", "za", "zb", "zc",
        };

        /// <summary>Поля допуска отверстия (заглавные).</summary>
        public static readonly string[] HoleLetters =
        {
            "A", "B", "C", "CD", "D", "E", "EF", "F", "FG", "G", "H",
            "JS", "J",
            "K", "M", "N", "P", "R", "S", "T", "U", "V", "X", "Y", "Z", "ZA", "ZB", "ZC",
        };

        /// <summary>Результат расчёта поля допуска (отклонения в мкм, номинал в мм).</summary>
        public sealed record Result(double Nominal, double UpperDeviation, double LowerDeviation, double ToleranceValue, bool IsHole)
        {
            /// <summary>Верхнее предельное отклонение (ES/es), мм.</summary>
            public double UpperDeviationMm => UpperDeviation / 1000.0;
            /// <summary>Нижнее предельное отклонение (EI/ei), мм.</summary>
            public double LowerDeviationMm => LowerDeviation / 1000.0;
            /// <summary>Допуск IT, мм.</summary>
            public double ToleranceValueMm => ToleranceValue / 1000.0;
            /// <summary>Наибольший предельный размер, мм.</summary>
            public double MaxSize => Nominal + UpperDeviationMm;
            /// <summary>Наименьший предельный размер, мм.</summary>
            public double MinSize => Nominal + LowerDeviationMm;
            /// <summary>Координата середины поля допуска, мкм.</summary>
            public double MiddleDeviation => (UpperDeviation + LowerDeviation) / 2.0;
            /// <summary>Координата середины поля допуска, мм.</summary>
            public double MiddleDeviationMm => MiddleDeviation / 1000.0;
            /// <summary>Середина поля допуска как абсолютный размер, мм.</summary>
            public double MiddleSize => Nominal + MiddleDeviationMm;
        }

        #region Интервалы размеров и квалитеты IT

        /// <summary>Основные интервалы для IT: (нижняя граница, верхняя включительно).</summary>
        private static readonly (double Over, double Incl)[] ItRanges =
        {
            (0, 3), (3, 6), (6, 10), (10, 18), (18, 30), (30, 50), (50, 80),
            (80, 120), (120, 180), (180, 250), (250, 315), (315, 400), (400, 500),
        };

        /// <summary>Допуски IT по квалитетам (мкм), IT4 хранится ради правила Δ (не для UI).</summary>
        private static readonly Dictionary<int, double[]> ItValues = new()
        {
            [4]  = new[] { 3.0, 4, 4, 5, 6, 7, 8, 10, 12, 14, 16, 18, 20 },
            [5]  = new[] { 4.0, 5, 6, 8, 9, 11, 13, 15, 18, 20, 23, 25, 27 },
            [6]  = new[] { 6.0, 8, 9, 11, 13, 16, 19, 22, 25, 29, 32, 36, 40 },
            [7]  = new[] { 10.0, 12, 15, 18, 21, 25, 30, 35, 40, 46, 52, 57, 63 },
            [8]  = new[] { 14.0, 18, 22, 27, 33, 39, 46, 54, 63, 72, 81, 89, 97 },
            [9]  = new[] { 25.0, 30, 36, 43, 52, 62, 74, 87, 100, 115, 130, 140, 155 },
            [10] = new[] { 40.0, 48, 58, 70, 84, 100, 120, 140, 160, 185, 210, 230, 250 },
            [11] = new[] { 60.0, 75, 90, 110, 130, 160, 190, 220, 250, 290, 320, 360, 400 },
            [12] = new[] { 100.0, 120, 150, 180, 210, 250, 300, 350, 400, 460, 520, 570, 630 },
            [13] = new[] { 140.0, 180, 220, 270, 330, 390, 460, 540, 630, 720, 810, 890, 970 },
            [14] = new[] { 250.0, 300, 360, 430, 520, 620, 740, 870, 1000, 1150, 1300, 1400, 1550 },
        };

        #endregion

        #region Основные отклонения вала

        /// <summary>Подразделённые интервалы основных отклонений (нижняя, верхняя включительно).</summary>
        private static readonly (double Over, double Incl)[] DevRanges =
        {
            (0, 3), (3, 6), (6, 10), (10, 14), (14, 18), (18, 24), (24, 30), (30, 40),
            (40, 50), (50, 65), (65, 80), (80, 100), (100, 120), (120, 140), (140, 160),
            (160, 180), (180, 200), (200, 225), (225, 250), (250, 280), (280, 315),
            (315, 355), (355, 400), (400, 450), (450, 500),
        };

        /// <summary>Верхние отклонения es вала (мкм) для a..h, j (нуль-направленные отрицательные).</summary>
        private static readonly Dictionary<string, int?[]> ShaftEs = new()
        {
            ["a"]  = new int?[] { -270, -270, -280, -290, -290, -300, -300, -310, -320, -340, -360, -380, -410, -460, -520, -580, -660, -740, -820, -920, -1050, -1200, -1350, -1500, -1650 },
            ["b"]  = new int?[] { -140, -140, -150, -150, -150, -160, -160, -170, -180, -190, -200, -220, -240, -260, -280, -310, -340, -380, -420, -480, -540, -600, -680, -760, -840 },
            ["c"]  = new int?[] { -60, -70, -80, -95, -95, -110, -110, -120, -130, -140, -150, -170, -180, -200, -210, -230, -240, -260, -280, -300, -330, -360, -400, -440, -480 },
            ["cd"] = new int?[] { -34, -46, -56, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null },
            ["d"]  = new int?[] { -20, -30, -40, -50, -50, -65, -65, -80, -80, -100, -100, -120, -120, -145, -145, -145, -170, -170, -170, -190, -190, -210, -210, -230, -230 },
            ["e"]  = new int?[] { -14, -20, -25, -32, -32, -40, -40, -50, -50, -60, -60, -72, -72, -85, -85, -85, -100, -100, -100, -110, -110, -125, -125, -135, -135 },
            ["ef"] = new int?[] { -10, -14, -18, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null },
            ["f"]  = new int?[] { -6, -10, -13, -16, -16, -20, -20, -25, -25, -30, -30, -36, -36, -43, -43, -43, -50, -50, -50, -56, -56, -62, -62, -68, -68 },
            ["fg"] = new int?[] { -4, -6, -8, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null },
            ["g"]  = new int?[] { -2, -4, -5, -6, -6, -7, -7, -9, -9, -10, -10, -12, -12, -14, -14, -14, -15, -15, -15, -17, -17, -18, -18, -20, -20 },
            ["h"]  = new int?[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
        };

        /// <summary>Верхние отклонения es вала для j по квалитетам (j5/j6/j7; j8 = j7).</summary>
        private static readonly Dictionary<int, int?[]> ShaftJsEs = new()
        {
            [5] = new int?[] { -2, -2, -2, -3, -3, -3, -3, -4, -4, -5, -7, -9, -9, -11, -11, -11, -13, -13, -13, -16, -16, -18, -18, -20, -20 },
            [6] = new int?[] { -2, -2, -2, -3, -3, -3, -3, -4, -4, -5, -7, -9, -9, -11, -11, -11, -13, -13, -13, -16, -16, -18, -18, -20, -20 },
            [7] = new int?[] { -4, -4, -5, -6, -6, -8, -8, -10, -10, -12, -12, -15, -15, -18, -18, -18, -21, -21, -21, -26, -26, -28, -28, -32, -32 },
        };

        /// <summary>Нижние отклонения ei вала (мкм) для k..zc (положительные).</summary>
        private static readonly Dictionary<string, int?[]> ShaftEi = new()
        {
            ["k"]  = new int?[] { 0, 1, 1, 1, 1, 2, 2, 2, 2, 2, 2, 3, 3, 3, 3, 3, 4, 4, 4, 4, 4, 4, 4, 5, 5 },
            ["m"]  = new int?[] { 2, 4, 6, 7, 7, 8, 8, 9, 9, 11, 11, 13, 13, 15, 15, 15, 17, 17, 17, 20, 20, 21, 21, 23, 23 },
            ["n"]  = new int?[] { 4, 8, 10, 12, 12, 15, 15, 17, 17, 20, 20, 23, 23, 27, 27, 27, 31, 31, 31, 34, 34, 37, 37, 40, 40 },
            ["p"]  = new int?[] { 6, 12, 15, 18, 18, 22, 22, 26, 26, 32, 32, 37, 37, 43, 43, 43, 50, 50, 50, 56, 56, 62, 62, 68, 68 },
            ["r"]  = new int?[] { 10, 15, 19, 23, 23, 28, 28, 34, 34, 41, 43, 51, 54, 63, 65, 68, 77, 80, 84, 94, 98, 108, 114, 126, 132 },
            ["s"]  = new int?[] { 14, 19, 23, 28, 28, 35, 35, 43, 43, 53, 59, 71, 79, 92, 100, 108, 122, 130, 140, 158, 170, 190, 208, 232, 252 },
            ["t"]  = new int?[] { null, null, null, null, null, null, 41, 48, 54, 66, 75, 91, 104, 122, 134, 146, 166, 180, 196, 218, 240, 268, 294, 330, 360 },
            ["u"]  = new int?[] { 18, 23, 28, 33, 33, 41, 48, 60, 70, 87, 102, 124, 144, 170, 190, 210, 236, 258, 284, 315, 350, 390, 435, 490, 540 },
            ["v"]  = new int?[] { null, null, null, null, 39, 47, 55, 68, 81, 102, 120, 146, 172, 202, 228, 252, 284, 310, 340, 385, 425, 475, 530, 595, 660 },
            ["x"]  = new int?[] { 20, 28, 34, 40, 45, 54, 64, 80, 97, 122, 146, 178, 210, 248, 280, 310, 350, 385, 425, 475, 525, 590, 660, 740, 820 },
            ["y"]  = new int?[] { null, null, null, null, null, 63, 75, 94, 114, 144, 174, 214, 254, 300, 340, 380, 425, 470, 520, 580, 650, 730, 820, 920, 1000 },
            ["z"]  = new int?[] { 26, 35, 42, 50, 60, 73, 88, 112, 136, 172, 210, 258, 310, 365, 415, 465, 520, 575, 640, 710, 790, 900, 1000, 1100, 1250 },
            ["za"] = new int?[] { 32, 42, 52, 64, 77, 98, 118, 148, 180, 226, 274, 335, 400, 470, 535, 600, 670, 740, 820, 920, 1000, 1150, 1300, 1450, 1600 },
            ["zb"] = new int?[] { 40, 50, 67, 90, 108, 136, 160, 200, 242, 300, 360, 445, 525, 620, 700, 780, 880, 960, 1050, 1200, 1300, 1500, 1650, 1850, 2100 },
            ["zc"] = new int?[] { 60, 80, 97, 130, 150, 188, 218, 274, 325, 405, 480, 585, 690, 800, 900, 1000, 1150, 1250, 1350, 1550, 1700, 1900, 2100, 2400, 2600 },
        };

        #endregion

        /// <summary>
        /// Расчёт поля допуска по номинальному размеру и обозначению (например "h7", "H7", "k6").
        /// </summary>
        /// <param name="nominal">Номинальный размер, мм (0–500).</param>
        /// <param name="field">Обозначение поля допуска: буква(ы) + квалитет.</param>
        /// <param name="result">Результат расчёта.</param>
        /// <param name="error">Сообщение об ошибке, если расчёт не удался.</param>
        public static bool TryCalculate(double nominal, string field, out Result result, out string error)
        {
            result = null!;
            error = string.Empty;

            if (nominal <= 0 || nominal > MaxNominal)
            {
                error = $"Номинальный размер должен быть от 0 до {MaxNominal} мм";
                return false;
            }

            if (!TryParseField(field, out var letter, out var grade, out var isHole))
            {
                error = "Не удалось разобрать обозначение поля допуска";
                return false;
            }

            if (grade < Grades[0] || grade > Grades[^1])
            {
                error = $"Квалитет IT{grade} не поддерживается (доступны IT{Grades[0]}–IT{Grades[^1]})";
                return false;
            }

            int itIndex = IndexOfRange(ItRanges, nominal);
            double tolerance = ItValues[grade][itIndex];

            if (!TryGetDeviation(letter, grade, isHole, nominal, tolerance, out double upper, out double lower))
            {
                error = $"Поле допуска «{field}» не определено для размера {nominal} мм";
                return false;
            }

            result = new Result(nominal, upper, lower, tolerance, isHole);
            return true;
        }

        /// <summary>
        /// Разбирает обозначение "h7"/"H7"/"k6"/"js7"/"za6" на букву, квалитет и вал/отверстие.
        /// </summary>
        private static bool TryParseField(string field, out string letter, out int grade, out bool isHole)
        {
            letter = string.Empty;
            grade = 0;
            isHole = false;

            if (string.IsNullOrWhiteSpace(field)) return false;

            field = field.Trim();
            int i = 0;
            while (i < field.Length && char.IsLetter(field[i])) i++;
            if (i == 0 || i == field.Length) return false;

            string letters = field[..i];
            if (!int.TryParse(field[i..], out grade)) return false;

            string key = letters.ToLowerInvariant();
            bool shaftExists = ShaftLetters.Contains(key);
            bool holeExists = HoleLetters.Contains(key.ToUpperInvariant());

            if (!shaftExists && !holeExists) return false;

            isHole = letters.Any(char.IsUpper) && holeExists;
            letter = isHole ? key.ToUpperInvariant() : key;

            return true;
        }

        /// <summary>Возвращает верхнее и нижнее предельные отклонения (мкм) для поля допуска.</summary>
        private static bool TryGetDeviation(string letter, int grade, bool isHole, double nominal, double tolerance, out double upper, out double lower)
        {
            upper = 0;
            lower = 0;

            if (!isHole)
            {
                return TryGetShaftDeviation(letter, grade, nominal, tolerance, out upper, out lower);
            }

            return TryGetHoleDeviation(letter, grade, nominal, tolerance, out upper, out lower);
        }

        private static bool TryGetShaftDeviation(string letter, int grade, double nominal, double tolerance, out double es, out double ei)
        {
            es = 0;
            ei = 0;

            // js — симметричное поле.
            if (letter == "js")
            {
                es = tolerance / 2.0;
                ei = -tolerance / 2.0;
                return true;
            }

            int devIndex = IndexOfRange(DevRanges, nominal);

            // j — табличное по квалитетам (j5/j6/j7, j8 = j7).
            if (letter == "j")
            {
                int jGrade = grade >= 8 ? 7 : grade;
                if (!ShaftJsEs.TryGetValue(jGrade, out var jArr) || jArr[devIndex] is not { } jEs) return false;
                es = jEs;
                ei = jEs - tolerance;
                return true;
            }

            // Буквы a..h — верхнее отклонение es из таблицы.
            if (ShaftEs.TryGetValue(letter, out var esArr))
            {
                if (esArr[devIndex] is not { } esVal) return false;
                es = esVal;
                ei = esVal - tolerance;
                return true;
            }

            // Буквы k..zc — нижнее отклонение ei из таблицы.
            if (ShaftEi.TryGetValue(letter, out var eiArr))
            {
                if (eiArr[devIndex] is not { } eiVal) return false;
                // k для IT8 и грубее: ei = 0.
                if (letter == "k" && grade >= 8) eiVal = 0;
                ei = eiVal;
                es = eiVal + tolerance;
                return true;
            }

            return false;
        }

        private static bool TryGetHoleDeviation(string letter, int grade, double nominal, double tolerance, out double es, out double ei)
        {
            es = 0;
            ei = 0;

            // JS — симметричное поле.
            if (letter == "JS")
            {
                es = tolerance / 2.0;
                ei = -tolerance / 2.0;
                return true;
            }

            int devIndex = IndexOfRange(DevRanges, nominal);
            int itIndex = IndexOfRange(ItRanges, nominal);
            string shaftLetter = letter.ToLowerInvariant();

            // Отверстия A..H — общее правило: EI = -es(вала).
            if (ShaftEs.TryGetValue(shaftLetter, out var esArr))
            {
                if (esArr[devIndex] is not { } shaftEs) return false;
                ei = -shaftEs;
                es = ei + tolerance;
                return true;
            }

            // Отверстие J — специальное правило (до IT8 включительно) через ei(j) = es(j) - IT.
            if (shaftLetter == "j")
            {
                int jGrade = grade >= 8 ? 7 : grade;
                if (!ShaftJsEs.TryGetValue(jGrade, out var jArr) || jArr[devIndex] is not { } jEs) return false;
                double shaftEi = jEs - tolerance;
                double delta = UsesSpecialRule(letter, grade) ? Delta(itIndex, grade) : 0.0;
                es = -shaftEi + delta;
                ei = es - tolerance;
                return true;
            }

            // Отверстия K..ZC — общее правило ES = -ei(вала), со специальным правилом +Δ для
            // K/M/N (до IT8) и P..ZC (до IT7).
            if (ShaftEi.TryGetValue(shaftLetter, out var eiArr))
            {
                if (eiArr[devIndex] is not { } shaftEiVal) return false;
                if (shaftLetter == "k" && grade >= 8) shaftEiVal = 0;
                double delta = UsesSpecialRule(letter, grade) ? Delta(itIndex, grade) : 0.0;
                es = -shaftEiVal + delta;
                ei = es - tolerance;
                return true;
            }

            return false;
        }

        /// <summary>Применяется ли специальное правило Δ для отверстия данного поля и квалитета.</summary>
        private static bool UsesSpecialRule(string letter, int grade)
        {
            return letter switch
            {
                "J" or "K" or "M" or "N" => grade <= 8,
                "P" or "R" or "S" or "T" or "U" or "V" or "X" or "Y" or "Z"
                    or "ZA" or "ZB" or "ZC" => grade <= 7,
                _ => false,
            };
        }

        /// <summary>Правило Δ = IT(n) - IT(n-1) для специального правила отверстий (в мкм).</summary>
        private static double Delta(int itIndex, int grade)
        {
            if (ItValues.TryGetValue(grade, out var itN) && ItValues.TryGetValue(grade - 1, out var itN1))
            {
                return itN[itIndex] - itN1[itIndex];
            }
            return 0.0;
        }

        /// <summary>Находит индекс интервала (lower < value <= upper).</summary>
        private static int IndexOfRange((double Over, double Incl)[] ranges, double value)
        {
            for (int i = 0; i < ranges.Length; i++)
            {
                if (value > ranges[i].Over && value <= ranges[i].Incl) return i;
            }
            return ranges.Length - 1;
        }
    }
}

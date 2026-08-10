using System;
using System.Collections.Generic;
using System.Text;
using Sunduk.PWA.Infrastructure.Sequences;
using Sunduk.PWA.Infrastructure.Sequences.ContourElements;
using Sunduk.PWA.Infrastructure.Sequences.ContourElements.Base;
using Sunduk.PWA.Infrastructure.Tools.Base;
using static Sunduk.PWA.Infrastructure.Util;

namespace Sunduk.PWA.Infrastructure.Templates
{
    /// <summary>
    /// Заменяет цепочки "строка1 + \n + строка2 + \n + ..." в *Operation.cs на читаемую
    /// последовательность вызовов. <see cref="Line"/> сам добавляет перевод строки —
    /// передавайте текст без завершающего \n. <see cref="Raw"/> добавляет текст как есть,
    /// без своего \n — для констант вроде <see cref="Operation.TurningReferentPoint"/>,
    /// которые уже заканчиваются переводом строки. Кроме этих низкоуровневых примитивов,
    /// есть набор именованных команд перехода (<see cref="ToolCall"/>,
    /// <see cref="CoordinateSystemFallback"/>, <see cref="CoolantOn"/>/<see cref="CoolantOff"/>,
    /// <see cref="ReferentPoint"/>, <see cref="Contour"/>, <see cref="HolePattern"/>) —
    /// каждая инкапсулирует один смысловой шаг перехода вместо того, чтобы вызывающий метод
    /// вручную повторял постоянно одинаковую логику подстановки/подавления по шаблону станка.
    /// Тела канонических циклов (G72/G74/G75/G76/G81/G83/G84/G166 и т.п.) этой моделью не
    /// покрываются намеренно — остаются одной-двумя параметризованными строками через
    /// <see cref="Line"/>/<see cref="Raw"/>, как и раньше.
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

        public GCodeBuilder RawIf(bool condition, string text) => condition ? Raw(text) : this;

        /// <summary>
        /// Вызов инструмента — строка по шаблону станка (<see cref="Util.ToolCall"/>).
        /// </summary>
        public GCodeBuilder ToolCall(Tool tool, Machine machine, CoordinateSystem coordinateSystem, Coolant coolant)
            => Line(tool.ToolCall(machine, coordinateSystem, coolant));

        /// <summary>
        /// Гарантированная отдельная строка с СК сразу после вызова инструмента — только для
        /// токарных переходов; не дублируется, если СК уже подставлена в
        /// <see cref="Machine.ToolCallTemplate"/> через {CS}. У фрезерных переходов СК зашита в
        /// строку самого рабочего перемещения — эта команда там не используется.
        /// </summary>
        public GCodeBuilder CoordinateSystemFallback(Machine machine, CoordinateSystem coordinateSystem)
            => LineIf(machine.CoordinateSystems.Count > 1 && !machine.ToolCallTemplateHasCoordinateSystem(), coordinateSystem.ToString());

        /// <summary>
        /// Гарантированное включение СОЖ сразу после вызова инструмента — не дублируется, если
        /// СОЖ уже подставлена в <see cref="Machine.ToolCallTemplate"/> через {COOLANT}.
        /// <paramref name="also"/> — дополнительное условие конкретного перехода (например
        /// «не выводить здесь, если станок уже вернулся в референтную точку и включит СОЖ там»).
        /// </summary>
        public GCodeBuilder CoolantOn(Machine machine, Coolant coolant, bool also = true)
            => LineIf(also && !machine.ToolCallTemplateHasCoolant(), Operation.CoolantOn(machine, coolant));

        /// <summary>
        /// Выключение СОЖ — в отличие от <see cref="CoolantOn"/> никогда не подавляется шаблоном
        /// вызова инструмента (тот управляет только включением).
        /// </summary>
        public GCodeBuilder CoolantOff(Machine machine, Coolant coolant)
            => Line(Operation.CoolantOff(machine, coolant));

        /// <summary>
        /// Возврат в референтную точку — токарный, управляется
        /// <see cref="Machine.LeadingReferentPoint"/>/<see cref="Machine.TrailingReferentPoint"/>.
        /// </summary>
        public GCodeBuilder ReferentPoint(Machine machine, bool leading)
            => RawIf(leading ? machine.LeadingReferentPoint : machine.TrailingReferentPoint, Operation.TurningReferentPoint);

        /// <summary>
        /// Паттерн отверстий фрезерных циклов: параметризованная строка цикла на первом
        /// отверстии (<paramref name="firstHoleCycleLine"/>), затем повторы на остальных —
        /// оборачивает уже существующую логику <see cref="Util.AddPoints"/> (сравнение координат,
        /// нормализация полярного угла), не переизобретает её.
        /// </summary>
        public GCodeBuilder HolePattern(List<Hole> holes, bool polar, Func<Hole, string> firstHoleCycleLine)
        {
            if (holes is null || holes.Count == 0) return this;
            var result = firstHoleCycleLine(holes[0]) + "\n";
            AddPoints(ref result, holes, polar);
            return Raw(result);
        }

        /// <summary>
        /// Контур точения (Point/Line/Arc) — первый узел как подвод G0, дальше G1 (Line/Point)
        /// или G2/G3 (Arc, CW/CCW). X узла — диаметр. Пропущенные X/Z у узла наследуются от
        /// предыдущего (как в превью, см. <see cref="Util.PathFromContour"/>). Скругление/фаска
        /// на узле (<see cref="Point.Blunt"/>, <see cref="ContourElements.Line.Blunt"/>) даёт
        /// суффикс R/C на перемещении к этому узлу — размер увеличивается на
        /// <paramref name="toolRadius"/> (для скругления — целиком, для фаски — наполовину, как
        /// уже считает <c>GroovingOperation.CutOffSequence</c> для SimpleChamfer), чтобы
        /// автоматическая обработка угла контроллером сама скруглила/срезала угол с учётом
        /// физического радиуса пластины. R/C не пишется, если следующий узел — дуга (контроллер
        /// так скруглить не умеет — см. <see cref="Geometry.ToolTipCompensation"/>, которая для
        /// этого случая либо уже вставила отдельную маленькую дугу, либо оставила известным
        /// ограничением). Скругление на самом первом узле контура (стык с мнимым торцом перед
        /// ним) реализовано отдельной короткой "мнимой" линией перед подводом — см. её врезку в
        /// теле метода — R/C-адрес нельзя повесить на сам G0. Если радиус дуги меньше половины
        /// хорды — как и в превью, дуга вырождается в прямую. Line.Angle не обрабатывается —
        /// ожидается, что X/Z узла уже посчитаны из угла на уровне редактора контура (см.
        /// ContourComponent.razor), в этот метод контур приходит с уже разрешёнными координатами
        /// (и, для контурного точения, уже скомпенсированными на радиус пластины на острых
        /// стыках — см. <see cref="Geometry.ToolTipCompensation"/> — этот метод сам компенсацию
        /// не делает).
        /// </summary>
        public GCodeBuilder Contour(List<Element> contour, bool speedOnFirstMove, int speed, double feed, double toolRadius = 0)
        {
            if (contour is null || contour.Count == 0) return this;
            double? prevX = null;
            double? prevZ = null;
            for (var i = 0; i < contour.Count; i++)
            {
                var element = contour[i];
                var x = element.X ?? prevX;
                var z = element.Z ?? prevZ;
                if (x is null || z is null) { prevX = x; prevZ = z; continue; }

                if (i == 0)
                {
                    // Скругление/фаска на самом первом узле контура (стык с мнимым торцом перед
                    // ним) не может пойти суффиксом на этот же подвод — это G0 (рапид), а R/C
                    // скругляет угол МЕЖДУ ДВУМЯ прямыми реза (G1). Как на реальном станке: перед
                    // первым узлом программируется короткая "мнимая" линия по торцу — рапид чуть
                    // в стороне по X, затем реальный G1-отрезок к узлу с адресом R/C, скругляющим
                    // переход к следующему элементу. Не применимо, если следующий элемент — дуга
                    // (см. BluntSuffix) — там нужна отдельная дуга сопряжения, а не R/C.
                    var startBlunt = element switch
                    {
                        Point p => p.Blunt,
                        Sunduk.PWA.Infrastructure.Sequences.ContourElements.Line l => l.Blunt,
                        _ => 0,
                    };
                    var startBluntType = element is Sunduk.PWA.Infrastructure.Sequences.ContourElements.Line startLine ? startLine.BluntType : Blunt.Radius;
                    var startSuffix = BluntSuffix(startBlunt, startBluntType, toolRadius, 0, contour);
                    if (startSuffix.Length > 0)
                    {
                        var compensated = CompensatedBluntSize(startBlunt, startBluntType, toolRadius);
                        var approachX = x.Value + 2 * compensated;
                        var firstSpeedFeed = speedOnFirstMove ? $" S{speed} F{feed.NC()}" : string.Empty;
                        Line($"G0 X{approachX.NC(0)} Z{z.Value.NC()}");
                        Line($"G1 X{x.Value.NC(0)} Z{z.Value.NC()}{startSuffix}{firstSpeedFeed}");
                    }
                    else
                    {
                        Line($"G0 X{x.Value.NC(0)} Z{z.Value.NC()}");
                    }
                    prevX = x; prevZ = z;
                    continue;
                }

                var speedFeedSuffix = speedOnFirstMove && i == 1 ? $" S{speed} F{feed.NC()}" : string.Empty;
                switch (element)
                {
                    case Arc arc when IsValidArc(arc, prevX, prevZ):
                        Line($"{(arc.Direction == Direction.CW ? "G2" : "G3")} X{x.Value.NC(0)} Z{z.Value.NC()} R{arc.Radius.NC()}{speedFeedSuffix}");
                        break;
                    case Point point:
                        Line($"G1 X{x.Value.NC(0)} Z{z.Value.NC()}{BluntSuffix(point.Blunt, Blunt.Radius, toolRadius, i, contour)}{speedFeedSuffix}");
                        break;
                    case Sunduk.PWA.Infrastructure.Sequences.ContourElements.Line line:
                        Line($"G1 X{x.Value.NC(0)} Z{z.Value.NC()}{BluntSuffix(line.Blunt, line.BluntType, toolRadius, i, contour)}{speedFeedSuffix}");
                        break;
                    default:
                        Line($"G1 X{x.Value.NC(0)} Z{z.Value.NC()}{speedFeedSuffix}");
                        break;
                }
                prevX = x; prevZ = z;
            }
            return this;
        }

        private static bool IsValidArc(Arc arc, double? prevX, double? prevZ)
        {
            if (arc.Radius <= 0) return false;
            var xDifference = (arc.X - prevX) / 2;
            var zDifference = arc.Z - prevZ;
            var chord = Math.Sqrt(Math.Pow(xDifference ?? 0, 2) + Math.Pow(zDifference ?? 0, 2));
            return arc.Radius * 2 >= chord;
        }

        private static double CompensatedBluntSize(double blunt, Blunt bluntType, double toolRadius)
            => bluntType is Blunt.SimpleChamfer ? blunt + toolRadius / 2 : blunt + toolRadius;

        private static string BluntSuffix(double blunt, Blunt bluntType, double toolRadius, int index, List<Element> contour)
        {
            if (blunt <= 0 || index + 1 >= contour.Count) return string.Empty;
            // R/C-адрес скругляет угол между ДВУМЯ прямыми (см. Geometry.ToolTipCompensation) —
            // если следующий элемент контура дуга, контроллер так скруглить не умеет; такой узел
            // либо уже материализован в отдельную маленькую дугу компенсатором (Blunt тогда уже
            // обнулён), либо остаётся её известным ограничением — в обоих случаях адрес R/C сюда
            // писать нельзя.
            if (contour[index + 1] is Arc) return string.Empty;
            var isChamfer = bluntType is Blunt.SimpleChamfer;
            return $" {(isChamfer ? "C" : "R")}{CompensatedBluntSize(blunt, bluntType, toolRadius).NC()}";
        }

        public override string ToString() => _sb.ToString();
    }
}

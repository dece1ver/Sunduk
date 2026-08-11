using System;
using System.Collections.Generic;
using Sunduk.PWA.Infrastructure.Sequences.ContourElements;
using Sunduk.PWA.Infrastructure.Sequences.ContourElements.Base;

namespace Sunduk.PWA.Infrastructure.Geometry
{
    /// <summary>
    /// Компенсация радиуса пластины для точения по контуру — координаты пересчитываются с
    /// номинальных (чертёжных) на координаты мнимой вершины инструмента (вектор №3: пересечение
    /// касательных к окружности радиуса пластины слева и снизу — типовой наружный проходной
    /// резец). Вдоль участков, идущих строго по Z (цилиндр) или строго по X (торец), координата
    /// по другой оси не меняется — прямые кромки пластины у мнимой вершины и так ориентированы
    /// вдоль этих направлений.
    /// <br/><br/>
    /// Узлы с явно заданным Blunt/BluntType на стыке прямая-прямая этот модуль НЕ трогает — их
    /// скругление уже делает сам G-код через стандартную автоматическую обработку угла
    /// контроллера (см. <see cref="Templates.GCodeBuilder.Contour"/> и его BluntSuffix, адреса
    /// R/C на G1) — компенсация там сводится к тому, чтобы прибавить радиус пластины к
    /// запрограммированному размеру фаски/скругления. R/C-адрес работает только когда ОБЕ
    /// стороны стыка — прямые (включая мнимый торец перед первым узлом контура); если одна из
    /// сторон — дуга, контроллер так скруглить не умеет, и этот модуль сам материализует
    /// скругление отдельной маленькой дугой (см. <see cref="ArcAnchor.Face"/>/<see cref="ArcAnchor.Cylinder"/>,
    /// по образцу `NippleComponent.GetBluntedShape`/`Arc2Component.GetBluntedShape` — сама формула
    /// вынесена в общий <see cref="ArcAnchor"/>, чтобы её же использовал калькулятор произвольной
    /// дуги в "Прочее", без второй копии). Важно: Blunt
    /// живёт только на Point/Line, не на Arc — поэтому скругление стыка прямая-дуга представимо
    /// только когда прямая ИДЁТ ПЕРЕД дугой (Blunt стоит на прямой, дуга — следующий узел); стык,
    /// где дуга идёт первой и Blunt нужен на её собственном конце, в текущей модели контура задать
    /// вообще нельзя (не хватает поля) — это ограничение редактора контура, а не этого модуля.
    /// <br/><br/>
    /// <b>Реализовано и численно сверено с эталонными калькуляторами (см. отчёт по задаче):</b>
    /// острые (без Blunt) стыки прямая-прямая под углом — <see cref="Calc.ChamferShifts"/>, та же
    /// формула, что уже использует `FacingOperation`/`GroovingOperation`; совпадает с
    /// `Components/Stuff/ChamferComponent.razor` (Precise-режим). Острые стыки торец(X-выровненный
    /// сосед, включая мнимый торец в начале контура)-дуга под НЕкасательным углом — сверено с
    /// простым (без скругления) примером `NippleComponent`. Явное скругление (Blunt) на стыке
    /// прямая-дуга — вставка отдельной маленькой дуги сопряжения — реализовано для ОБОИХ
    /// направлений прямой: торец (<see cref="ArcAnchor.Face"/>, сверено с `NippleComponent.GetBluntedShape`)
    /// и цилиндр (<see cref="ArcAnchor.Cylinder"/>, сверено с `Arc2Component.GetBluntedShape`).
    /// Формула для цилиндра выведена зеркальным переносом (X↔Z) уже рабочей формулы для торца,
    /// используя ТОЛЬКО номинальные центр/радиус самой дуги — без какого-либо допущения о том, чем
    /// скомпенсирован другой конец дуги (в частности, не зависит от того, что у Arc2 вход дуги в
    /// торец на другом конце — касательный, частный случай) — и при этом совпадает с числами Arc2
    /// точно, что подтверждает и корректность, и независимость формулы от этого частного случая.
    /// Все сверки — точное численное совпадение на нескольких наборах параметров через временный
    /// консольный проект (удалён после проверки, в репозитории не остался). Радиус дуги ± радиус
    /// пластины — всегда, для любой дуги (центр дуги при этом не двигаем — он используется этим
    /// модулем только как номинальный ориентир для касательных, а не как истинный центр
    /// скомпенсированной дуги, которая после сдвига координат концов геометрически уже не обязана
    /// иметь центр ровно там).
    /// <br/><br/>
    /// <b>Не реализовано (известные ограничения этой версии) — во всех случаях ниже узел остаётся
    /// в номинальных координатах, R/C-адрес на нём подавлен (см. <c>GCodeBuilder.BluntSuffix</c>),
    /// а если на узле стоял явный Blunt — он молча теряется (ни доп.дуга не вставляется, ни R/C не
    /// пишется), а не считается по неподтверждённой формуле:</b> (1) острые (без Blunt) стыки
    /// цилиндр(Z-выровненный сосед)-дуга — пробовал зеркально перенести (X↔Z) рабочую формулу для
    /// торца, как это удалось для Blunt-случая (<see cref="CylinderArcBlunt"/>) — НЕ сошлось.
    /// Уточнение по ходу проверки: простой (без скругления) пример `Arc2Component` — НЕ годный
    /// эталон для этого случая, хоть и выглядит похоже; его цилиндрический конец в этом примере
    /// на самом деле касательный (дуга тангенциально входит в торец на ДРУГОМ конце, а
    /// `StartDiamWithShift` там — прямой линейный сдвиг на радиус пластины без тригонометрии), то
    /// есть это пример для касательного случая (см. п.2 ниже), а не для общего острого стыка.
    /// Отдельная попытка проверить зеркальную формулу независимым построением (через положение
    /// центра носика инструмента в момент перехода плоская кромка → дуга, а не напрямую через
    /// мнимую вершину) дала близкое, но не точное совпадение с уже проверенным Nipple-примером
    /// (11.696 вместо 11.6, зеркально) — говорит, что ChamferShifts-конструкция для стыка
    /// прямая-дуга опирается на что-то более тонкое, чем прямой перенос "мнимая вершина = центр
    /// носика ± R", и представление о том, что "вершина" и "центр носика" эквивалентны на дуге,
    /// но НЕ эквивалентны на прямой кромке (собственно то, ради чего вся эта компенсация и нужна)
    /// — разобраться в точной границе между этими двумя режимами за отведённое время не удалось.
    /// (2) Касательный (или близкий к касательному) вход/выход дуги — для торца это отдельная,
    /// более простая формула (прямой сдвиг на радиус пластины без тригонометрии, как раз
    /// `Arc2Component.StartDiamWithShift`), которую в отведённое время не удалось надёжно увязать
    /// с общей моделью. (3) Стыки дуга-дуга — нет эталонного примера для сверки вообще. (4) Вся
    /// компенсация на стыке прямая-дуга сделана только для наружного точения
    /// (<paramref name="external"/> = true) — нет проверенного примера с внутренним для сверки
    /// знака. Работает в истинных радиусных координатах (не диаметр) — конвертация на границе
    /// модуля.
    /// </summary>
    public static class ToolTipCompensation
    {
        private const double Tolerance = 1e-6;

        /// <summary>Виртуальный торец перед первым узлом контура — направление вдоль X.</summary>
        private static readonly GDirection VirtualFaceDirection = new(1, 0);

        /// <summary>
        /// Точка входа: номинальный контур (X — диаметр, как везде в проекте) → контур,
        /// готовый для рендера <see cref="Templates.GCodeBuilder.Contour"/>, с учтённым радиусом
        /// пластины <paramref name="toolRadius"/> на острых стыках и дугах. <paramref name="external"/>
        /// — наружное (true) или внутреннее (false) точение, определяет знак поправки радиусов дуг.
        /// </summary>
        public static List<Element> Compensate(List<Element> contour, double toolRadius, bool external)
        {
            if (contour is null || contour.Count == 0 || toolRadius <= 0) return contour;

            var nodes = ToRadiusNodes(contour);
            var (compensated, finalizedArcs) = CompensateCorners(nodes, toolRadius, external);
            AdjustArcRadii(compensated, toolRadius, external, finalizedArcs);
            return ToDiameterElements(compensated);
        }

        // ------------------------------------------------------------------
        // Внутреннее представление: те же типы узлов (Point/Line/Arc), но X уже переведён в
        // истинный радиус вместо диаметра — вся геометрия ниже считается в этих координатах.
        // ------------------------------------------------------------------

        private static List<Element> ToRadiusNodes(List<Element> contour)
        {
            var result = new List<Element>(contour.Count);
            foreach (var e in contour)
            {
                var r = e.X / 2;
                result.Add(e switch
                {
                    Point p => new Point(r, p.Z, p.Blunt),
                    Line l => new Line(r, l.Z, l.Angle, l.Blunt, l.BluntType),
                    Arc a => new Arc(r, a.Z, a.Radius, a.Direction, a.CenterX / 2, a.CenterZ),
                    _ => e,
                });
            }
            return result;
        }

        private static List<Element> ToDiameterElements(List<Element> nodes)
        {
            var result = new List<Element>(nodes.Count);
            foreach (var e in nodes)
            {
                var d = e.X * 2;
                result.Add(e switch
                {
                    Point p => new Point(d, p.Z, p.Blunt),
                    Line l => new Line(d, l.Z, l.Angle, l.Blunt, l.BluntType),
                    Arc a => new Arc(d, a.Z, a.Radius, a.Direction, a.CenterX * 2, a.CenterZ),
                    _ => e,
                });
            }
            return result;
        }

        // ------------------------------------------------------------------
        // Единый проход по стыкам контура. Строит НОВЫЙ список узлов (стык с явным Blunt на
        // границе с дугой превращается из одного узла в два — сдвинутая точка на торце + отдельная
        // маленькая дуга сопряжения), поэтому не мутирует `nodes` — все TangentInto/TangentOutOf
        // подстановки читают из исходного (немутированного) `nodes` по оригинальным индексам,
        // что корректно: вставка узла в другом месте контура не меняет геометрию соседних стыков.
        // Возвращает также индексы (в РЕЗУЛЬТИРУЮЩЕМ списке) только что вставленных маленьких дуг
        // сопряжения — их радиус уже финальный (Blunt + toolRadius), AdjustArcRadii не должен
        // прибавлять radius пластины к ним ещё раз.
        // ------------------------------------------------------------------

        /// <summary>Стык дуги от 1° до 89° от оси Z — вне этого диапазона (почти касательный
        /// вход/выход) формула не сверялась (для касательного случая нужна отдельная, более
        /// простая формула), такие стыки намеренно остаются нескомпенсированными.</summary>
        private const double ArcAngleGuardLow = 1;
        private const double ArcAngleGuardHigh = 89;

        private static (List<Element> Nodes, HashSet<int> FinalizedArcs) CompensateCorners(List<Element> nodes, double toolRadius, bool external)
        {
            var result = new List<Element>(nodes.Count);
            var finalizedArcs = new HashSet<int>();

            for (var i = 0; i < nodes.Count; i++)
            {
                var curr = nodes[i];
                if (curr.X is null || curr.Z is null) { result.Add(curr); continue; }

                var next = i + 1 < nodes.Count ? nodes[i + 1] : null;
                var hasExplicitBlunt = curr switch
                {
                    Point p => p.Blunt > 0,
                    Line l => l.Blunt > 0,
                    _ => false,
                };
                var incomingIsArc = curr is Arc;
                var outgoingIsArc = next is Arc;

                // Явное скругление на стыке прямая-дуга (торец ИЛИ цилиндр перед дугой) — R/C-адрес
                // контроллера тут не работает (умеет только прямая-прямая, см. класс-докстринг),
                // нужна отдельная маленькая дуга сопряжения, как в NippleComponent.GetBluntedShape
                // (RoundCorner-режим) для торца и Arc2Component.GetBluntedShape для цилиндра.
                if (hasExplicitBlunt && external && !incomingIsArc && outgoingIsArc && next is Arc arcNext)
                {
                    var blunt = curr switch { Point p => p.Blunt, Line l => l.Blunt, _ => 0 };
                    var inDirBlunt = i == 0 ? VirtualFaceDirection : TangentInto(nodes, i);
                    if (inDirBlunt is not null && IsFaceDirection(inDirBlunt.Value))
                    {
                        var blunted = ArcAnchor.Face(curr.Z.Value, arcNext.CenterX, arcNext.CenterZ, arcNext.Radius, blunt, toolRadius);
                        if (blunted is not null)
                        {
                            result.Add(curr switch
                            {
                                Point p => new Point(blunted.Value.StartX, curr.Z.Value, 0),
                                Line l => new Line(blunted.Value.StartX, curr.Z.Value, l.Angle, 0, l.BluntType),
                                _ => curr,
                            });
                            finalizedArcs.Add(result.Count);
                            result.Add(new Arc(blunted.Value.EndX, blunted.Value.EndZ, blunt + toolRadius, arcNext.Direction, arcNext.CenterX, arcNext.CenterZ));
                            continue;
                        }
                    }
                    else if (inDirBlunt is not null && IsCylinderDirection(inDirBlunt.Value))
                    {
                        var blunted = ArcAnchor.Cylinder(curr.X.Value, arcNext.CenterX, arcNext.CenterZ, arcNext.Radius, blunt, toolRadius);
                        if (blunted is not null)
                        {
                            result.Add(curr switch
                            {
                                Point p => new Point(curr.X.Value, blunted.Value.TangentZ, 0),
                                Line l => new Line(curr.X.Value, blunted.Value.TangentZ, l.Angle, 0, l.BluntType),
                                _ => curr,
                            });
                            finalizedArcs.Add(result.Count);
                            result.Add(new Arc(blunted.Value.HandoffX, blunted.Value.HandoffZ, blunt + toolRadius, arcNext.Direction, arcNext.CenterX, arcNext.CenterZ));
                            continue;
                        }
                    }
                }

                if (hasExplicitBlunt) { result.Add(curr); continue; } // line-line blunt — обработает BluntSuffix

                if (next is null) { result.Add(curr); continue; } // последний узел — стыка после него нет

                var outgoingAligned = IsAxisAligned(curr, next);
                var incomingAligned = i == 0 || IsAxisAligned(nodes[i - 1], curr);
                if (incomingAligned && outgoingAligned) { result.Add(curr); continue; } // прямой выровненный угол — поправка не нужна

                if (!incomingIsArc && !outgoingIsArc)
                {
                    var inDir = i == 0 ? VirtualFaceDirection : TangentInto(nodes, i);
                    var outDir = TangentOutOf(nodes, i);
                    if (inDir is null || outDir is null) { result.Add(curr); continue; }

                    var corner = new GPoint(curr.X.Value, curr.Z.Value);

                    // Прямая-прямая под углом (не 90° — выровненный-выровненный уже отсеян выше):
                    // Calc.ChamferShifts(угол наклонной кромки от Z, r) даёт (X, Z) — величины,
                    // на которые нужно "перебежать" от номинального угла в сторону уже снятого
                    // материала: по R — к центру для наружного точения (от центра для
                    // внутреннего), по Z — всегда в сторону отрицательных Z (торец детали — это
                    // Z=0, материал всегда в сторону минуса), ровно как считает ChamferComponent
                    // (Precise-режим).
                    var angle = IncludedAngleFromZ(inDir.Value, outDir.Value);
                    var shift = Calc.ChamferShifts(angle, toolRadius);
                    var rSign = external ? -1 : 1;
                    var compensated = new GPoint(corner.X + rSign * shift.X, corner.Z - shift.Z);

                    result.Add(curr switch
                    {
                        Point p => new Point(compensated.X, compensated.Z, p.Blunt),
                        Line l => new Line(compensated.X, compensated.Z, l.Angle, l.Blunt, l.BluntType),
                        Arc a => new Arc(compensated.X, compensated.Z, a.Radius, a.Direction, a.CenterX, a.CenterZ),
                        _ => curr,
                    });
                    continue;
                }

                // Ровно одна сторона стыка — дуга (дуга-дуга ниже намеренно пропускается, см.
                // класс-докстринг). Реализовано только для торца (X-выровненный сосед) на
                // наружном точении, под НЕкасательным углом — сверено с простым (без скругления)
                // примером NippleComponent. Торец+дуга под касательным углом и цилиндр
                // (Z-выровненный сосед)+дуга — известные ограничения, не реализованы. Цилиндр+дуга
                // (острый, без Blunt) пробовал зеркальным (X↔Z) переносом этой же формулы — НЕ
                // сошлось: попытка проверить независимым построением (нос-инструмента-центр вместо
                // мнимой вершины на границе плоская кромка/дуга) дала 11.696 вместо доверенных 11.6
                // на зеркале уже проверенного Nipple-примера — расхождение мелкое, но говорит, что
                // используемая здесь ChamferShifts-конструкция для стыка прямая-дуга опирается на
                // что-то более тонкое, чем прямой перенос "мнимая вершина = центр носика ± R",
                // разобраться в котором за отведённое время не удалось. Ранее казавшийся годным
                // эталон (простой пример Arc2Component без скругления) тоже НЕ подходит для этого
                // случая — его цилиндрический конец на самом деле касательный (arc входит в торец
                // тангенциально на ДРУГОМ конце, а у StartDiamWithShift в Arc2 — прямой линейный
                // сдвиг на радиус пластины, не тригонометрия), то есть это пример для касательного
                // случая (см. ниже), а не для общего острого стыка цилиндр-дуга. Оставлено
                // нескомпенсированным, как и раньше — не подставлять неподтверждённую формулу.
                if (external && incomingIsArc != outgoingIsArc)
                {
                    GDirection? arcTangent;
                    GDirection? straightDir;
                    if (outgoingIsArc)
                    {
                        arcTangent = TangentOutOf(nodes, i);
                        straightDir = i == 0 ? VirtualFaceDirection : TangentInto(nodes, i);
                    }
                    else
                    {
                        arcTangent = TangentInto(nodes, i);
                        straightDir = TangentOutOf(nodes, i);
                    }

                    if (arcTangent is not null && straightDir is not null && IsFaceDirection(straightDir.Value))
                    {
                        var angle = Math.Atan2(Math.Abs(arcTangent.Value.X), Math.Abs(arcTangent.Value.Z)).Degrees();
                        if (angle > ArcAngleGuardLow && angle < ArcAngleGuardHigh)
                        {
                            var shift = Calc.ChamferShifts(angle, toolRadius);
                            var compensated = new GPoint(curr.X.Value - shift.Z, curr.Z.Value);
                            result.Add(curr switch
                            {
                                Point p => new Point(compensated.X, compensated.Z, p.Blunt),
                                Line l => new Line(compensated.X, compensated.Z, l.Angle, l.Blunt, l.BluntType),
                                Arc a => new Arc(compensated.X, compensated.Z, a.Radius, a.Direction, a.CenterX, a.CenterZ),
                                _ => curr,
                            });
                            continue;
                        }
                    }
                }

                result.Add(curr);
            }

            return (result, finalizedArcs);
        }

        private static bool IsFaceDirection(GDirection d) => Math.Abs(d.Z) < Tolerance;

        private static bool IsCylinderDirection(GDirection d) => Math.Abs(d.X) < Tolerance;

        private static bool IsConvexToTool(Arc arc, bool external)
            => external; // приближение первой версии — точное определение выпуклости требует
                         // знания стороны материала на каждой конкретной дуге, не только типа резца

        // ------------------------------------------------------------------
        // Радиусы дуг: ± радиус пластины (всегда, независимо от выравнивания соседей), кроме уже
        // финализированных маленьких дуг сопряжения из CompensateCorners (их радиус — уже точный
        // Blunt + toolRadius, повторно прибавлять toolRadius нельзя).
        // ------------------------------------------------------------------

        private static void AdjustArcRadii(List<Element> nodes, double toolRadius, bool external, HashSet<int> finalizedArcs)
        {
            for (var i = 0; i < nodes.Count; i++)
            {
                if (nodes[i] is not Arc arc || finalizedArcs.Contains(i)) continue;
                var convex = IsConvexToTool(arc, external);
                var newRadius = convex ? arc.Radius + toolRadius : arc.Radius - toolRadius;
                if (newRadius <= 0) continue; // пластина не влезает в вогнутую дугу — оставляем номинальный радиус
                nodes[i] = new Arc(arc.X, arc.Z, newRadius, arc.Direction, arc.CenterX, arc.CenterZ);
            }
        }

        // ------------------------------------------------------------------
        // Геометрические примитивы и служебные функции
        // ------------------------------------------------------------------

        private readonly record struct GPoint(double X, double Z)
        {
            public static GPoint operator -(GPoint a, GPoint b) => new(a.X - b.X, a.Z - b.Z);
            public static GPoint operator +(GPoint a, GPoint b) => new(a.X + b.X, a.Z + b.Z);
        }

        private readonly record struct GDirection(double X, double Z);

        private static GDirection? TangentInto(List<Element> nodes, int index)
        {
            if (index <= 0) return null;
            var prev = nodes[index - 1];
            var curr = nodes[index];
            if (prev.X is null || prev.Z is null || curr.X is null || curr.Z is null) return null;
            // Сегмент, приходящий в узел index, описан данными самого curr (если curr — Arc, это
            // она рисует отрезок prev→curr, см. GCodeBuilder.Contour), а не prev. Раньше здесь по
            // ошибке проверялось `prev is Arc`, что при curr=Line/Point и prev=Arc брало центр
            // ЧУЖОЙ (предыдущей) дуги как будто он относится к касательной текущего отрезка —
            // исправлено.
            if (curr is Arc arc)
            {
                var radial = new GPoint(curr.X.Value, curr.Z.Value) - new GPoint(arc.CenterX, arc.CenterZ);
                return Normalize(new GDirection(-radial.Z, radial.X), arc.Direction);
            }
            return Normalize(new GDirection(curr.X.Value - prev.X.Value, curr.Z.Value - prev.Z.Value));
        }

        private static GDirection? TangentOutOf(List<Element> nodes, int index)
        {
            if (index >= nodes.Count - 1) return null;
            var curr = nodes[index];
            var next = nodes[index + 1];
            if (curr.X is null || curr.Z is null || next.X is null || next.Z is null) return null;
            if (next is Arc arc)
            {
                var radial = new GPoint(curr.X.Value, curr.Z.Value) - new GPoint(arc.CenterX, arc.CenterZ);
                return Normalize(new GDirection(-radial.Z, radial.X), arc.Direction);
            }
            return Normalize(new GDirection(next.X.Value - curr.X.Value, next.Z.Value - curr.Z.Value));
        }

        private static GDirection? Normalize(GDirection d, Direction? arcDirection = null)
        {
            var len = Math.Sqrt(d.X * d.X + d.Z * d.Z);
            if (len < Tolerance) return null;
            var n = new GDirection(d.X / len, d.Z / len);
            if (arcDirection == Direction.CCW) n = new GDirection(-n.X, -n.Z);
            return n;
        }

        private static bool IsAxisAligned(Element a, Element b)
        {
            if (a.X is null || a.Z is null || b.X is null || b.Z is null) return true;
            if (b is Arc) return false;
            var dx = Math.Abs(b.X.Value - a.X.Value);
            var dz = Math.Abs(b.Z.Value - a.Z.Value);
            return dx < Tolerance || dz < Tolerance;
        }

        /// <summary>Угол наклонной кромки от оси Z, 0..90 — тот же вход, что у Calc.ChamferShifts.</summary>
        private static double IncludedAngleFromZ(GDirection inDir, GDirection outDir)
        {
            // используем ту из двух кромок, что не выровнена по оси — если обе наклонные,
            // берём исходящую (первая версия, см. известные ограничения)
            var d = !IsNearlyAxisAligned(outDir) ? outDir : inDir;
            return Math.Atan2(Math.Abs(d.X), Math.Abs(d.Z)).Degrees();
        }

        private static bool IsNearlyAxisAligned(GDirection d)
            => Math.Abs(d.X) < Tolerance || Math.Abs(d.Z) < Tolerance;
    }
}

using System;
using System.Collections.Generic;
using Sunduk.PWA.Infrastructure.Sequences.Base;
using Sunduk.PWA.Infrastructure.Sequences.Turning;
using Sunduk.PWA.Infrastructure.Tools.Base;
using Sunduk.PWA.Infrastructure.Tools.Turning;
using Sunduk.PWA.Infrastructure.Tools.Turning.Base;

namespace Sunduk.PWA.Infrastructure
{
    /// <summary>
    /// Профиль заготовки токарной детали — наружный и внутренний диаметр как функция Z, начиная
    /// от исходной заготовки и с учётом изменений от предыдущих переходов программы (см.
    /// <see cref="Build"/>). Не учитывает фрезерные переходы и не хранит настоящую 2D/3D
    /// геометрию — только по одному диаметру на каждый Z, этого достаточно, чтобы следующий
    /// переход мог спросить «какой сейчас диаметр в этом месте», не сканируя программу вручную
    /// (как раньше делал MachineTime-независимый GetCurrentInternalDiameter).
    /// </summary>
    public sealed class StockProfile
    {
        private readonly List<(double FromZ, double ToZ, double Diameter)> _external = new();
        private readonly List<(double FromZ, double ToZ, double Diameter)> _internal = new();

        /// <summary><paramref name="rawStockToZ"/> — реальная правая граница сырой заготовки (в
        /// сторону +Z, откуда торчит необработанный припуск под торцовку), если известна (см.
        /// <see cref="Build"/>) — до неё материал есть безусловно, дальше его нет вообще, даже если
        /// в программе ещё не было ни одной торцовки. По умолчанию (+∞) — прежнее поведение для
        /// вызовов, которым эта граница не важна (например, точечные запросы диаметра на Z=0).</summary>
        public StockProfile(double externalDiameter, double internalDiameter, double rawStockToZ = double.PositiveInfinity)
        {
            _external.Add((double.NegativeInfinity, rawStockToZ, externalDiameter));
            _internal.Add((double.NegativeInfinity, rawStockToZ, internalDiameter));
        }

        public double ExternalDiameterAt(double z) => DiameterAt(_external, z);
        public double InternalDiameterAt(double z) => DiameterAt(_internal, z);

        /// <summary>Сегменты наружного диаметра как есть (крайние — от/до бесконечности, пока
        /// заготовка не обрезана переходом с этой стороны) — для отрисовки силуэта.</summary>
        public IReadOnlyList<(double FromZ, double ToZ, double Diameter)> ExternalSegments => _external;

        /// <summary>Сегменты внутреннего диаметра как есть — см. <see cref="ExternalSegments"/>.</summary>
        public IReadOnlyList<(double FromZ, double ToZ, double Diameter)> InternalSegments => _internal;

        /// <summary>Резать/сверлить можно только там, где материал физически уже есть — переходы
        /// часто задают Z как безопасную точку подвода (например, сверловка "от Z2", когда торец
        /// уже обработан до 0.2), а не как реальную границу материала. Без клипа такая запись
        /// создавала бы "дырку в воздухе" за пределами текущего наружного профиля — в отрисовке
        /// (заливка через fill-rule=evenodd) это выглядит как ПОЯВИВШИЙСЯ из ниоткуда материал
        /// (нечётное число пересечений вне охватывающего наружного контура = закрашено), хотя
        /// реально там ничего не должно меняться.</summary>
        private void WriteExternal(double z1, double z2, double diameter)
        {
            var fromZ = Math.Min(z1, z2);
            var toZ = Math.Max(z1, z2);
            foreach (var (segFrom, segTo) in ExistingMaterialRanges(fromZ, toZ)) Write(_external, segFrom, segTo, diameter);
        }

        private void WriteInternal(double z1, double z2, double diameter)
        {
            var fromZ = Math.Min(z1, z2);
            var toZ = Math.Max(z1, z2);
            foreach (var (segFrom, segTo) in ExistingMaterialRanges(fromZ, toZ)) Write(_internal, segFrom, segTo, diameter);
        }

        /// <summary>Части [fromZ, toZ], пересекающиеся с уже существующим наружным материалом
        /// (Diameter &gt; 0) — см. <see cref="WriteExternal"/>/<see cref="WriteInternal"/>.</summary>
        private IEnumerable<(double FromZ, double ToZ)> ExistingMaterialRanges(double fromZ, double toZ)
        {
            foreach (var segment in _external)
            {
                if (segment.Diameter <= 0) continue;
                var segFrom = Math.Max(segment.FromZ, fromZ);
                var segTo = Math.Min(segment.ToZ, toZ);
                if (segFrom < segTo) yield return (segFrom, segTo);
            }
        }

        private static double DiameterAt(List<(double FromZ, double ToZ, double Diameter)> segments, double z)
        {
            foreach (var segment in segments)
            {
                if (z >= segment.FromZ && z <= segment.ToZ) return segment.Diameter;
            }
            return 0;
        }

        private static void Write(List<(double FromZ, double ToZ, double Diameter)> segments, double z1, double z2, double diameter)
        {
            var fromZ = Math.Min(z1, z2);
            var toZ = Math.Max(z1, z2);
            if (fromZ == toZ) return;
            var result = new List<(double FromZ, double ToZ, double Diameter)>(segments.Count + 2);
            foreach (var segment in segments)
            {
                if (segment.ToZ <= fromZ || segment.FromZ >= toZ)
                {
                    result.Add(segment);
                    continue;
                }
                if (segment.FromZ < fromZ) result.Add((segment.FromZ, fromZ, segment.Diameter));
                if (segment.ToZ > toZ) result.Add((toZ, segment.ToZ, segment.Diameter));
            }
            result.Add((fromZ, toZ, diameter));
            segments.Clear();
            segments.AddRange(result);
        }

        /// <summary>
        /// Строит профиль по программе от исходной заготовки, применяя изменения от переходов
        /// <c>program[0..uptoIndex)</c> по порядку — последняя запись в конкретный участок Z
        /// побеждает, что соответствует реальности (последний рез в этом месте детали — тот, что
        /// остался). Переходы без вычислимой геометрии (произвольный G-код и т.п.) пропускаются,
        /// профиль в этом месте не меняется. <paramref name="rawStockToZ"/> — см. конструктор.
        /// </summary>
        public static StockProfile Build(IReadOnlyList<Sequence> program, int uptoIndex, double externalDiameter, double internalDiameter, double rawStockToZ = double.PositiveInfinity)
        {
            var profile = new StockProfile(externalDiameter, internalDiameter, rawStockToZ);
            for (var i = 0; i < uptoIndex && i < program.Count; i++)
            {
                Apply(profile, program[i]);
            }
            return profile;
        }

        private static void Apply(StockProfile profile, Sequence sequence)
        {
            switch (sequence)
            {
                // Торцовка обрезает заготовку по Z: всё, что было дальше фактического нового
                // торца (в сторону +Z, откуда торчит необработанный припуск), физически снято.
                // Реальный конечный Z зависит от конкретного вида торцовки — он НЕ всегда равен
                // полю ProfStockAllow: у объединённой чёрное+чистовое (FacingSequence) и у G70
                // чистовой по циклу (FinishFacingCycleSequence) профиль P/Q в самом G-коде всегда
                // целится в Z=0 (см. FacingOperation.Facing/FinishFacingCycle, endZ = cycleProfStockAllow
                // ? 0 : profStockAllow, а FinishFacingCycle всегда перезапускает тот же P/Q блок) —
                // ProfStockAllow там участвует только как параметр W у цикла G72 (сколько оставить
                // под последующую чистовую), не как реальная координата. У самостоятельной черновой
                // без запланированного G70 (RoughFacingSequence, RoughFacingCycleSequence до своей
                // чистовой) и у самостоятельной чистовой (FinishFacingSequence) ProfStockAllow —
                // это и есть реальный конечный Z.
                case FacingSequence facing:
                    ClipFace(profile, 0);
                    break;
                case RoughFacingSequence roughFacing:
                    ClipFace(profile, roughFacing.ProfStockAllow);
                    break;
                case RoughFacingCycleSequence roughFacingCycle:
                    ClipFace(profile, roughFacingCycle.ProfStockAllow);
                    break;
                case FinishFacingSequence finishFacing:
                    ClipFace(profile, finishFacing.ProfStockAllow);
                    break;
                case FinishFacingCycleSequence:
                    ClipFace(profile, 0);
                    break;
                case RoughTurningSequence roughTurning:
                    ApplyContour(profile, roughTurning.Tool, roughTurning.Contour);
                    break;
                case FinishTurningSequence finishTurning:
                    ApplyContour(profile, finishTurning.Tool, finishTurning.Contour);
                    break;
                case HighSpeedDrillingSequence highSpeedDrilling:
                    profile.WriteInternal(highSpeedDrilling.StartZ, highSpeedDrilling.EndZ, highSpeedDrilling.Tool.Diameter);
                    break;
                case PeckDrillingSequence peckDrilling:
                    profile.WriteInternal(peckDrilling.StartZ, peckDrilling.EndZ, peckDrilling.Tool.Diameter);
                    break;
                case PeckDeepDrillingSequence peckDeepDrilling:
                    profile.WriteInternal(peckDeepDrilling.StartZ, peckDeepDrilling.EndZ, peckDeepDrilling.Tool.Diameter);
                    break;
                case TurningInternalGroovingSequence internalGrooving:
                    ApplyGrooving(profile, internalGrooving.Tool, internalGrooving.CuttingPoint, internalGrooving.Width, internalGrooving.InternalDiameter, isExternal: false);
                    break;
                case TurningInternalRoughGroovingSequence internalRoughGrooving:
                    ApplyGrooving(profile, internalRoughGrooving.Tool, internalRoughGrooving.CuttingPoint, internalRoughGrooving.Width, internalRoughGrooving.InternalDiameter, isExternal: false);
                    break;
                case TurningExternalGroovingSequence externalGrooving:
                    ApplyGrooving(profile, externalGrooving.Tool, externalGrooving.CuttingPoint, externalGrooving.Width, externalGrooving.InternalDiameter, isExternal: true);
                    break;
                case TurningExternalRoughGroovingSequence externalRoughGrooving:
                    ApplyGrooving(profile, externalRoughGrooving.Tool, externalRoughGrooving.CuttingPoint, externalRoughGrooving.Width, externalRoughGrooving.InternalDiameter, isExternal: true);
                    break;
                case TurningFaceGroovingSequence faceGrooving:
                    // у торцевых канавок Width/CuttingPoint — это сразу Z-начало/Z-конец, а не якорь+протяжённость
                    profile.WriteExternal(faceGrooving.Width, faceGrooving.CuttingPoint, faceGrooving.InternalDiameter);
                    break;
                case TurningFaceRoughGroovingSequence faceRoughGrooving:
                    profile.WriteExternal(faceRoughGrooving.Width, faceRoughGrooving.CuttingPoint, faceRoughGrooving.InternalDiameter);
                    break;
            }
        }

        /// <summary>Обрезает и наружный, и внутренний профиль от фактического нового торца
        /// <paramref name="faceZ"/> до +∞ (материала физически больше нет). Вызывает низкоуровневый
        /// <see cref="Write"/> напрямую, а не <see cref="WriteExternal"/>/<see cref="WriteInternal"/> —
        /// им обеим нужно обнулиться БЕЗУСЛОВНО (это они и есть источник границы материала), а не
        /// клипаться друг относительно друга: наружный вызов первым же обнулил бы область, и
        /// внутреннему после него было бы не с чем пересекаться.</summary>
        private static void ClipFace(StockProfile profile, double faceZ)
        {
            Write(profile._external, faceZ, double.PositiveInfinity, 0);
            Write(profile._internal, faceZ, double.PositiveInfinity, 0);
        }

        private static void ApplyGrooving(StockProfile profile, TurningGroovingTool tool, double cuttingPoint, double width, double diameter, bool isExternal)
        {
            var startPoint = tool.ZeroPoint == TurningGroovingTool.Point.Right ? cuttingPoint : cuttingPoint - tool.Width;
            var endPoint = startPoint - (width - tool.Width);
            if (isExternal) profile.WriteExternal(startPoint, endPoint, diameter);
            else profile.WriteInternal(startPoint, endPoint, diameter);
        }

        private static void ApplyContour(StockProfile profile, TurningTool tool, List<Element> contour)
        {
            if (contour is null || contour.Count < 2 || tool is null) return;
            var isExternal = tool is TurningExternalTool;
            if (!isExternal && tool is not TurningInternalTool) return;
            for (var i = 0; i < contour.Count - 1; i++)
            {
                var from = contour[i];
                var to = contour[i + 1];
                if (from.Z is null || to.Z is null || to.X is null) continue;
                if (isExternal) profile.WriteExternal(from.Z.Value, to.Z.Value, to.X.Value);
                else profile.WriteInternal(from.Z.Value, to.Z.Value, to.X.Value);
            }
        }
    }
}

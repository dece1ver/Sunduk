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

        public StockProfile(double externalDiameter, double internalDiameter)
        {
            _external.Add((double.NegativeInfinity, double.PositiveInfinity, externalDiameter));
            _internal.Add((double.NegativeInfinity, double.PositiveInfinity, internalDiameter));
        }

        public double ExternalDiameterAt(double z) => DiameterAt(_external, z);
        public double InternalDiameterAt(double z) => DiameterAt(_internal, z);

        /// <summary>Сегменты наружного диаметра как есть (крайние — от/до бесконечности, пока
        /// заготовка не обрезана переходом с этой стороны) — для отрисовки силуэта.</summary>
        public IReadOnlyList<(double FromZ, double ToZ, double Diameter)> ExternalSegments => _external;

        /// <summary>Сегменты внутреннего диаметра как есть — см. <see cref="ExternalSegments"/>.</summary>
        public IReadOnlyList<(double FromZ, double ToZ, double Diameter)> InternalSegments => _internal;

        private void WriteExternal(double z1, double z2, double diameter) => Write(_external, z1, z2, diameter);
        private void WriteInternal(double z1, double z2, double diameter) => Write(_internal, z1, z2, diameter);

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
        /// профиль в этом месте не меняется.
        /// </summary>
        public static StockProfile Build(IReadOnlyList<Sequence> program, int uptoIndex, double externalDiameter, double internalDiameter)
        {
            var profile = new StockProfile(externalDiameter, internalDiameter);
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

        private static void ApplyGrooving(StockProfile profile, TurningGroovingTool tool, double cuttingPoint, double width, double diameter, bool isExternal)
        {
            var startPoint = tool.ZeroPoint == TurningGroovingTool.Point.Right ? cuttingPoint : cuttingPoint - tool.Width;
            var endPoint = startPoint - (width - tool.Width);
            if (isExternal) profile.WriteExternal(startPoint, endPoint, diameter);
            else profile.WriteInternal(startPoint, endPoint, diameter);
        }

        private static void ApplyContour(StockProfile profile, TurningTool tool, List<Sequences.ContourElements.Base.Element> contour)
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

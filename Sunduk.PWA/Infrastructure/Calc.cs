using Sunduk.PWA.Infrastructure.Sequences.Base;
using Sunduk.PWA.Infrastructure.Sequences.Turning;
using Sunduk.PWA.Infrastructure.Templates;
using Sunduk.PWA.Infrastructure.Tools.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sunduk.PWA.Components.Knowledge;
using Sunduk.PWA.Infrastructure.CAM;
using Sunduk.PWA.Infrastructure.Time;
using Sunduk.PWA.Infrastructure.Tools.Turning;
using static MudBlazor.Defaults;

namespace Sunduk.PWA.Infrastructure
{
    public static class Calc
    {
        /// <summary>
        /// Считает угол полярной системы координат от точек в декартовой
        /// </summary>
        /// <param name="x">Координата Х в декартовой СК</param>
        /// <param name="y">Координата Х в декартовой СК</param>
        /// <returns>Угол для полярной СК</returns>
        public static double GetAngleFromXY(double x, double y)
        {
            if (x > 0 && y > 0)
            {
                return Math.Atan(x / y).Degrees();
            }
            if (x > 0 && y < 0)
            {
                return (90 + (90 - Math.Atan(x / -y).Degrees()));
            }
            if (x < 0 && y < 0)
            {
                return (180 + (90 - Math.Atan(-x / -y).Degrees()));
            }
            if (x < 0 && y > 0)
            {
                return (270 + (90 - Math.Atan(-x / y).Degrees()));
            }
            return 0;
        }

        /// <summary>
        /// Переводит радианы в угол</summary>
        /// <param name="radians">Значение в радианах</param>
        /// <returns>Угловое значение</returns>
        public static double Degrees(this double radians)
        {
            return radians * 180 / Math.PI;
        }

        /// <summary>
        /// Переводит угол в радианы
        /// </summary>
        /// <param name="degrees">Угловое значение</param>
        /// <returns>Радиан</returns>
        public static double Radians(this double degrees)
        {
            return degrees! * Math.PI / 180;
        }

        /// <summary>
        /// Переводит угол в радианы
        /// </summary>
        /// <param name="degrees">Угловое значение</param>
        /// <returns>Радиан</returns>
        public static double Radians(this int degrees)
        {
            return degrees * Math.PI / 180;
        }


        /// <summary>
        /// Округляет
        /// </summary>
        /// <param name="rounder">Значение округления</param>
        /// <returns>Радиан</returns>
        public static int Round(this int value, int rounder = 1)
        {
            if (rounder < 1) rounder = 1;
            if (value < rounder) return value;
            return value / rounder * rounder;
        }

        /// <summary>
        /// Округляет
        /// </summary>
        /// <param name="rounder">Значение округления</param>
        /// <returns>Радиан</returns>
        public static int Round(this double value, int rounder = 1)
        {
            if (rounder < 1) rounder = 1;
            if (value < rounder) return (int)value;
            return (int)Math.Round(value / rounder) * rounder;
        }

        /// <summary>
        /// Миллиметры в микроны
        /// </summary>
        /// <param name="value">Число в миллиметрах</param>
        /// <returns>Отформатированную строку</returns>
        public static int Microns(this double value)
        {
            return (value * 1000).Round(10);
        }

        /// <summary>
        /// Миллиметры в микроны
        /// </summary>
        /// <param name="value">Число в миллиметрах</param>
        /// <returns>Отформатированную строку</returns>
        public static int Microns(this int value)
        {
            return (value * 1000).Round(10);
        }

        /// <summary>
        /// Скорость резания в обороты
        /// </summary>
        /// <param name="cutSpeed">Скорость</param>
        /// <param name="diameter">Диаметр</param>
        /// <returns>Обороты шпинделя</returns>
        public static int ToSpindleSpeed(this int cutSpeed, double diameter, int round = 0) => (cutSpeed * 1000 / (int)(diameter * Math.PI)).Round(round);

        /// <summary>
        /// Скорость резания в обороты
        /// </summary>
        /// <param name="cutSpeed">Скорость</param>
        /// <param name="diameter">Диаметр</param>
        /// <returns>Обороты шпинделя</returns>
        public static int ToSpindleSpeed(this double cutSpeed, double diameter, int round = 0) => (cutSpeed * 1000 / (int)(diameter * Math.PI)).Round(round);

        /// <summary>
        /// Подачу на оборот в минутную подачу
        /// </summary>
        /// <param name="cutSpeed">Скорость</param>
        /// <param name="diameter">Диаметр</param>
        /// <returns>Обороты шпинделя</returns>
        public static int ToFeedPerMin(this int cutFeed, double spindleSpeed, int edges = 1, int round = 0) => (cutFeed * spindleSpeed * edges).Round(round);

        /// <summary>
        /// Подачу на оборот в минутную подачу
        /// </summary>
        /// <param name="cutSpeed">Скорость</param>
        /// <param name="diameter">Диаметр</param>
        /// <returns>Обороты шпинделя</returns>
        public static int ToFeedPerMin(this double cutFeed, double spindleSpeed, int edges = 1, int round = 0) => (cutFeed * spindleSpeed * edges).Round(round);

        /// <summary>
        /// Длина конуса от диаметра сверла
        /// </summary>
        /// <param name="drillingTool">Сверло</param>
        /// <returns></returns>
        public static double PointLength(this DrillingTool drillingTool) => (drillingTool.Diameter / 2 * Math.Tan((90 - drillingTool.Angle / 2).Radians()));

        /// <summary>
        /// Длина конуса от диаметра сверла
        /// </summary>
        /// <param name="drillingTool">Сверло</param>
        /// <returns></returns>
        public static double PointLength(this double diameter, double angle) => (diameter / 2 * Math.Tan((90 - angle / 2).Radians()));

        /// <summary>
        /// Смещение от виртуальных точек пересечения до концов радиусов
        /// </summary>
        /// <param name="angle">Угол фаски от горизонтальной оси</param>
        /// <param name="radius">Радиус на углах фаски</param>
        /// <returns></returns>
        public static (double X, double Z) ChamferRadiusLengths(double angle, double radius, int round = 3)
        {
            return (
                Math.Round(Math.Tan(((90 - angle) / 2).Radians()) * radius, round, MidpointRounding.ToPositiveInfinity),
                Math.Round(Math.Tan((angle / 2).Radians()) * radius, round, MidpointRounding.ToPositiveInfinity));
        }

        /// <summary>
        /// Смещение координат фаски с учетом радиуса пластины
        /// </summary>
        /// <param name="angle"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public static (double X, double Z) ChamferShifts(double angle, double radius)
        {
            return 
                (Math.Tan(angle.Radians()) * (radius - radius / Math.Tan((90 - angle / 2).Radians())),
                radius - radius / Math.Tan((90 - angle / 2).Radians()));
        }
        

        public static double AxialTurningTime(this double length, double spins, double feed)
        {
            return 60 * length / (feed * spins);
        }

        public static double AxialRapidTime(this double length)
        {
            return 60 * length / (Operation.RapidSpeed());
        }

        public static double CrossTurningTime(double maxDiam, double minDiam, double spins, double feed)
        {
            return 60 * Math.Abs(maxDiam - minDiam) / (2 * feed * spins);
        }

        public static double CrossRapidTime(double maxDiam, double minDiam)
        {
            return 60 * Math.Abs(maxDiam - minDiam) / (2 * Operation.RapidSpeed());
        }


        /// <summary>
        /// Время обработки при высокоскоростном сверлении
        /// </summary>
        public static OperationTime OperationTime(this HighSpeedDrillingSequence highSpeedDrillingSequence)
        {
            double cuttingTime = 0;
            double rapidTime = 5;
            var fullLength = (Math.Abs(highSpeedDrillingSequence.EndZ) + Math.Abs(highSpeedDrillingSequence.StartZ));
            var feed = highSpeedDrillingSequence.Feed;
            var speed = highSpeedDrillingSequence.Speed;
            var spins = (speed * 1000) / (Math.PI * highSpeedDrillingSequence.Tool.Diameter);
            if (spins > 3000) spins = 3000;
            cuttingTime += fullLength.AxialTurningTime(spins, feed);
            rapidTime += fullLength.AxialRapidTime();
            return new OperationTime(cuttingTime, rapidTime);
        }

        /// <summary>
        /// Время обработки при глубоком сверлении
        /// </summary>
        public static OperationTime OperationTime(this PeckDeepDrillingSequence peckDeepDrillingSequence)
        {
            double cuttingTime = 0;
            double rapidTime = 5;
            var fullLength = (Math.Abs(peckDeepDrillingSequence.EndZ) + Math.Abs(peckDeepDrillingSequence.StartZ));
            var steps = (int)Math.Round(fullLength / peckDeepDrillingSequence.Depth, MidpointRounding.ToPositiveInfinity);
            var stepLength = peckDeepDrillingSequence.Depth + Operation.Escaping();
            if (stepLength > fullLength)
            {
                stepLength = fullLength;
                steps = 1;
            }

            if (steps > 2) steps -= 1;

            var lastStep = fullLength - steps * peckDeepDrillingSequence.Depth + Operation.Escaping();
            var feed = peckDeepDrillingSequence.Feed;
            var speed = peckDeepDrillingSequence.Speed;
            var spins = (speed * 1000) / (Math.PI * peckDeepDrillingSequence.Tool.Diameter);
            if (spins > 3000) spins = 3000;
            double currentLength = 0;
            // время резания
            cuttingTime += steps * stepLength.AxialTurningTime(spins, feed) + 
                          lastStep.AxialTurningTime(spins, feed);
            // время ввода/вывода сверла
            if (steps > 1) steps++;
            for (var i = 0; i < steps; i++)
            {
                currentLength += stepLength;
                rapidTime += 2 * currentLength.AxialRapidTime();
            }

            return new OperationTime(cuttingTime, rapidTime);
        }

        /// <summary>
        /// Время обработки при прерывистом сверлении
        /// </summary>
        public static OperationTime OperationTime(this PeckDrillingSequence peckDrillingSequence)
        {
            double cuttingTime = 0;
            double rapidTime = 5;
            var fullLength = (Math.Abs(peckDrillingSequence.EndZ) + Math.Abs(peckDrillingSequence.StartZ));
            var steps = (int)Math.Round(fullLength / peckDrillingSequence.Depth, MidpointRounding.ToPositiveInfinity);
            var feed = peckDrillingSequence.Feed;
            var speed = peckDrillingSequence.Speed;
            var spins = (speed * 1000) / (Math.PI * peckDrillingSequence.Tool.Diameter);
            if (spins > 3000) spins = 3000;
            var stepLength = peckDrillingSequence.Depth + Operation.Escaping();
            if (stepLength > fullLength)
            {
                stepLength = fullLength;
                steps = 1;
            }
            if (steps > 2) steps -= 1;

            var lastStep = fullLength - steps * peckDrillingSequence.Depth + Operation.Escaping();

            // время резания
            cuttingTime += steps * stepLength.AxialTurningTime(spins, feed) + 
                           lastStep.AxialTurningTime(spins, feed);
            // время ввода/вывода сверла
            if (steps > 1) steps++;
            rapidTime += steps * Operation.Escaping().AxialRapidTime();
            rapidTime += fullLength.AxialRapidTime();
            return new OperationTime(cuttingTime, rapidTime);
        }

        

        /// <summary>
        /// Время обработки при черновом торцевании
        /// </summary>
        public static OperationTime OperationTime(this RoughFacingSequence roughFacingSequence)
        {
            double cuttingTime = 0;
            double rapidTime = 5;
            
            var startX = roughFacingSequence.ExternalDiameter;
            var endX = roughFacingSequence.InternalDiameter;
            var startZ = roughFacingSequence.RoughStockAllow;
            var endZ = roughFacingSequence.ProfStockAllow;
            var speedRough = roughFacingSequence.SpeedRough;
            var feedRough = roughFacingSequence.FeedRough;
            var fullLength = startZ - endZ;
            var steps = (int)Math.Round(fullLength / roughFacingSequence.StepOver, MidpointRounding.ToPositiveInfinity);
            var spins = (speedRough * 1000) / (Math.PI * ((startX - endX) / 2));
            if (spins > 3000) spins = 3000;
            cuttingTime += steps * CrossTurningTime(startX, endX, spins, feedRough);

            rapidTime += steps * CrossRapidTime(startX, endX);

            return new OperationTime(cuttingTime, rapidTime);
        }

        /// <summary>
        /// Время обработки при черновом точении
        /// </summary>
        public static OperationTime OperationTime(this RoughTurningSequence roughTurningSequence)
        {
            double cuttingTime = 0;
            double rapidTime = 5;
            var startX = Math.Abs(roughTurningSequence.Contour[0].X ?? 0);
            var endX = Math.Abs(roughTurningSequence.Contour[1].X ?? 0);
            var startZ = Math.Abs(roughTurningSequence.Contour[0].Z ?? 0);
            var endZ = Math.Abs(roughTurningSequence.Contour[1].Z ?? 0);
            var fullLength = startZ + endZ;
            var speed = roughTurningSequence.SpeedRough;
            var feed = roughTurningSequence.FeedRough;
            int steps;
            if (Math.Abs(startX - endX) < 0.001)
            {
                steps = 1;
            }
            else
            {
                steps = startX > endX 
                ? (int)Math.Round((startX - endX) / 2 / roughTurningSequence.StepOver, MidpointRounding.ToPositiveInfinity) 
                : (int)Math.Round((endX - startX) / 2 / roughTurningSequence.StepOver, MidpointRounding.ToPositiveInfinity);
            }
            var spins = (speed * 1000) / (Math.PI * ((startX + endX) / 2));
            if (spins > 3000) spins = 3000;
            cuttingTime += steps * AxialTurningTime(fullLength + Operation.Escaping(), spins, feed);

            rapidTime += steps * AxialRapidTime(fullLength);
            rapidTime += steps * 2 * (roughTurningSequence.StepOver.AxialRapidTime()); // подъемы и опускания между проходами

            return new OperationTime(cuttingTime, rapidTime);
        }

        /// <summary>
        /// Время обработки при чистовом точении
        /// </summary>
        public static OperationTime OperationTime(this FinishTurningSequence finishTurningSequence)
        {
            double cuttingTime = 0;
            double rapidTime = 5;
            var startX = Math.Abs(finishTurningSequence.Contour[0].X ?? 0);
            var endX = Math.Abs(finishTurningSequence.Contour[1].X ?? 0);
            var startZ = Math.Abs(finishTurningSequence.Contour[0].Z ?? 0);
            var endZ = Math.Abs(finishTurningSequence.Contour[1].Z ?? 0);
            var fullHeight = Math.Abs(startX - endX) / 2;
            var fullLength = startZ + endZ;
            var speed = finishTurningSequence.SpeedFinish;
            var feed = finishTurningSequence.FeedFinish;
            var spins = (speed * 1000) / (Math.PI * endX);
            if (spins > 3000) spins = 3000;
            cuttingTime += AxialTurningTime(fullLength + Operation.Escaping(), spins, feed);
            cuttingTime += AxialTurningTime(fullHeight + Operation.Escaping(), spins, feed);

            rapidTime += AxialRapidTime(fullLength);
            rapidTime += fullLength.AxialRapidTime();

            return new OperationTime(cuttingTime, rapidTime);
        }

        /// <summary>
        /// Время обработки при отрезке
        /// </summary>
        public static OperationTime OperationTime(this TurningCutOffSequence turningCutOffSequence)
        {
            double cuttingTime = 0;
            double rapidTime = 5;
            var startX = turningCutOffSequence.ExternalDiameter;
            var endX = turningCutOffSequence.InternalDiameter;
            var fullLength = (startX - endX) / 2;
            var steps = (int)Math.Round(fullLength / turningCutOffSequence.StepOver, MidpointRounding.ToPositiveInfinity);
            var speed = turningCutOffSequence.SpeedRough;
            var feed = turningCutOffSequence.FeedRough;
            var spins = (speed * 1000) / (Math.PI * ((startX + endX) / 2));
            if (spins > 3000) spins = 3000;
            cuttingTime += steps * (turningCutOffSequence.StepOver + Operation.Escaping()).AxialTurningTime(spins, feed);

            rapidTime += steps * AxialRapidTime(turningCutOffSequence.StepOver + Operation.Escaping());
            rapidTime += AxialRapidTime(fullLength);

            return new OperationTime(cuttingTime, rapidTime);
        }

        /// <summary>
        /// Время обработки наружной канавки
        /// </summary>
        public static OperationTime OperationTime(this TurningExternalGroovingSequence turningExternalGroovingSequence)
            {
            double cuttingTime = 0;
            double rapidTime = 5;
            var startX = turningExternalGroovingSequence.ExternalDiameter;
            var endX = turningExternalGroovingSequence.InternalDiameter;
            var fullLengthX = (startX - endX) / 2;
            var stepsX = (int)Math.Round(fullLengthX / turningExternalGroovingSequence.StepOver, MidpointRounding.ToPositiveInfinity);
            var width = turningExternalGroovingSequence.Width;
            if (width < turningExternalGroovingSequence.Tool.Width) width = turningExternalGroovingSequence.Tool.Width;
            var stepsZ = (int)Math.Round(width / (turningExternalGroovingSequence.Tool.Width * 2 ), MidpointRounding.ToPositiveInfinity);
            var roughSpeed = turningExternalGroovingSequence.SpeedRough;
            var roughFeed = turningExternalGroovingSequence.FeedRough;
            var finishSpeed = turningExternalGroovingSequence.SpeedFinish;
            var finishFeed = turningExternalGroovingSequence.FeedFinish;
            var roughSpins = (roughSpeed * 1000) / (Math.PI * ((startX + endX) / 2));
            var finishSpins = (finishSpeed * 1000) / (Math.PI * ((startX + endX) / 2));
            if (roughSpins > 3000) roughSpins = 3000;
            if (finishSpins > 3000) finishSpins = 3000;
            cuttingTime += stepsZ * (stepsX * (turningExternalGroovingSequence.StepOver + Operation.Escaping()).AxialTurningTime(roughSpins, roughFeed));
            cuttingTime += 2 * (fullLengthX + Operation.Escaping()).AxialTurningTime(finishSpins, finishFeed);

            rapidTime += stepsZ * (stepsX * AxialRapidTime(turningExternalGroovingSequence.StepOver + Operation.Escaping()));
            rapidTime += 3 * AxialRapidTime(fullLengthX + Operation.Escaping());
            rapidTime += AxialRapidTime(fullLengthX); // ???

            return new OperationTime(cuttingTime, rapidTime);
        }

        /// <summary>
        /// Время обработки внутренней канавки
        /// </summary>
        public static OperationTime OperationTime(this TurningInternalGroovingSequence turningInternalGroovingSequence)
        {
            double cuttingTime = 0;
            double rapidTime = 5;
            var startX = turningInternalGroovingSequence.ExternalDiameter;
            var endX = turningInternalGroovingSequence.InternalDiameter;
            var fullLengthX = (startX - endX) / 2;
            var stepsX = (int)Math.Round(fullLengthX / turningInternalGroovingSequence.StepOver, MidpointRounding.ToPositiveInfinity);
            var width = turningInternalGroovingSequence.Width;
            if (width < turningInternalGroovingSequence.Tool.Width) width = turningInternalGroovingSequence.Tool.Width;
            var stepsZ = (int)Math.Round(width / turningInternalGroovingSequence.Tool.Width, MidpointRounding.ToPositiveInfinity);
            var roughSpeed = turningInternalGroovingSequence.SpeedRough;
            var roughFeed = turningInternalGroovingSequence.FeedRough;
            var finishSpeed = turningInternalGroovingSequence.SpeedFinish;
            var finishFeed = turningInternalGroovingSequence.FeedFinish;
            var roughSpins = (roughSpeed * 1000) / (Math.PI * ((startX + endX) / 2));
            var finishSpins = (finishSpeed * 1000) / (Math.PI * ((startX + endX) / 2));
            if (roughSpins > 3000) roughSpins = 3000;
            if (finishSpins > 3000) finishSpins = 3000;
            cuttingTime += stepsZ * (stepsX * (turningInternalGroovingSequence.StepOver + Operation.Escaping()).AxialTurningTime(roughSpins, roughFeed));
            cuttingTime += 2 * (fullLengthX + Operation.Escaping()).AxialTurningTime(finishSpins, finishFeed);

            rapidTime += stepsZ * (stepsX * AxialRapidTime(turningInternalGroovingSequence.StepOver + Operation.Escaping()));
            rapidTime += 3 * AxialRapidTime(fullLengthX + Operation.Escaping());
            rapidTime += 2 * AxialRapidTime(Math.Abs(turningInternalGroovingSequence.CuttingPoint) + Operation.Escaping());
            rapidTime += AxialRapidTime(fullLengthX); // ???

            return new OperationTime(cuttingTime, rapidTime);
        }

        /// <summary>
        /// Время обработки торцевой канавки
        /// </summary>
        public static OperationTime OperationTime(this TurningFaceGroovingSequence turningFaceGroovingSequence)
            {
            double cuttingTime = 0;
            double rapidTime = 5;
            var startX = turningFaceGroovingSequence.ExternalDiameter;
            var endX = turningFaceGroovingSequence.InternalDiameter;
            var width = (turningFaceGroovingSequence.ExternalDiameter - turningFaceGroovingSequence.InternalDiameter) / 2;
            var length = Math.Abs(turningFaceGroovingSequence.CuttingPoint) + Math.Abs(turningFaceGroovingSequence.Width); // надо переделать, вместо ширины сделать конечный Z или что-то такое
            var stepsZ = (int)Math.Round(length / turningFaceGroovingSequence.StepOver, MidpointRounding.ToPositiveInfinity);
            if (width < turningFaceGroovingSequence.Tool.Width) width = turningFaceGroovingSequence.Tool.Width;
            var stepsX = (int)Math.Round(width * 2 / turningFaceGroovingSequence.Tool.Width, MidpointRounding.ToPositiveInfinity);
            var roughSpeed = turningFaceGroovingSequence.SpeedRough;
            var roughFeed = turningFaceGroovingSequence.FeedRough;
            var finishSpeed = turningFaceGroovingSequence.SpeedFinish;
            var finishFeed = turningFaceGroovingSequence.FeedFinish;
            var roughSpins = (roughSpeed * 1000) / (Math.PI * ((startX + endX) / 2));
            var finishSpins = (finishSpeed * 1000) / (Math.PI * ((startX + endX) / 2));
            if (roughSpins > 3000) roughSpins = 3000;
            if (finishSpins > 3000) finishSpins = 3000;
            cuttingTime += stepsX * (stepsZ * (turningFaceGroovingSequence.StepOver + Operation.Escaping()).AxialTurningTime(roughSpins, roughFeed));
            cuttingTime += 2 * (length + Operation.Escaping()).AxialTurningTime(finishSpins, finishFeed);

            rapidTime += stepsX * (stepsZ * AxialRapidTime(turningFaceGroovingSequence.StepOver + Operation.Escaping()));
            rapidTime += 3 * AxialRapidTime(length + Operation.Escaping());
            rapidTime += AxialRapidTime(length); // ???

            return new OperationTime(cuttingTime, rapidTime);
        }

        /// <summary>
        /// Время обработки при накатывании
        /// </summary>
        public static OperationTime OperationTime(this TurningBurnishingSequence turningBurnishingSequence)
        {
            
            double cuttingTime = 0;
            double rapidTime = 5;
            var fullLength = (Math.Abs(turningBurnishingSequence.StartZ) + Math.Abs(turningBurnishingSequence.EndZ));
            var additionalRapidMove = turningBurnishingSequence.Tool is TurningExternalBurnishingTool ? 1 : 1 + fullLength;
            var speed = turningBurnishingSequence.SpeedFinish;
            var feed = turningBurnishingSequence.FeedFinish;
            var spins = (speed * 1000) / (Math.PI * turningBurnishingSequence.Diameter);
            if (spins > 1000) spins = 1000;
            cuttingTime += (fullLength + 1).AxialTurningTime(spins, feed);
            cuttingTime += Operation.Escaping().AxialTurningTime(spins, feed);
            rapidTime += fullLength.AxialRapidTime();
            rapidTime += additionalRapidMove.AxialRapidTime();
            return new OperationTime(cuttingTime, rapidTime);
        }

        
        /// <summary>
        /// Время обработки при нарезании резьбы метчиком
        /// </summary>
        public static OperationTime OperationTime(this TappingSequence tappingSequence)
        {
            double cuttingTime = 0;
            double rapidTime = 5;
            var fullLength = (Math.Abs(tappingSequence.EndZ) + Math.Abs(tappingSequence.StartZ));
            var feed = tappingSequence.Tool.Pitch;
            var speed = tappingSequence.CutSpeed;
            var spins = (speed * 1000) / (Math.PI * tappingSequence.Tool.Diameter);
            if (spins > 3000) spins = 3000;
            cuttingTime += 2 * fullLength.AxialTurningTime(spins, feed);
            rapidTime += 1;
            return new OperationTime(cuttingTime, rapidTime);
        }

        /// <summary>
        /// Время обработки при нарезании резьбы резцом
        /// </summary>
        public static OperationTime OperationTime(this ThreadCuttingSequence threadCuttingSequence)
        {
            double cuttingTime = 0;
            double rapidTime = 5;
            var fullLength = Math.Abs(threadCuttingSequence.EndZ) + Math.Abs(threadCuttingSequence.StartZ) + Thread.ThreadRunout(threadCuttingSequence.ThreadStandard, threadCuttingSequence.ThreadPitch, CuttingType.External);
            var feed = threadCuttingSequence.ThreadPitch;
            var spins = threadCuttingSequence.Speed.ToSpindleSpeed(threadCuttingSequence.ThreadDiameter);
            if (spins > 2000) spins = 2000;
            var passes = Thread.PassesCount(threadCuttingSequence.ThreadStandard, threadCuttingSequence.ThreadPitch) + 3;
            cuttingTime += passes * fullLength.AxialTurningTime(spins, feed);
            rapidTime += passes * fullLength.AxialRapidTime();
            rapidTime += passes * 2 * (2.0.AxialRapidTime()); // 2 мм подъемы и опускания между проходами
            return new OperationTime(cuttingTime, rapidTime);
        }

        /// <summary>
        /// Время работы упора
        /// </summary>
        public static OperationTime OperationTime(this LimiterSequence _) => new(0, 15);

        /// <summary>
        /// Время работы останова
        /// </summary>
        public static OperationTime OperationTime(this StopSequence _) => new(0, 30);

        /// <summary>
        /// Время подвода задней бабки
        /// </summary>
        public static OperationTime OperationTime(this TailstockOnSequence _) => new(0, 15);

        /// <summary>
        /// Время отвода задней бабки
        /// </summary>
        public static OperationTime OperationTime(this TailstockOffSequence _) => new(0, 15);




        /// <summary>
        /// Общее время обработки
        /// </summary>
        public static OperationTime FullOperationTime(this List<Sequence> sequences)
        {
            if (sequences is null || sequences.Count < 1) return new OperationTime(0,0);
            var cuttingTime = sequences.Sum(x => x.MachineTime.CuttingTime);
            var rapidTime = sequences.Sum(x => x.MachineTime.RapidTime);
            var sequencesWithRapidMovement = sequences.Count(x => x is 
                not TailstockOnSequence and 
                not TailstockOffSequence and 
                not StopSequence);
            var toolChangeTime = (sequencesWithRapidMovement - 1) * 2;
            if (toolChangeTime < 0) toolChangeTime = 0;
            rapidTime += toolChangeTime;
            return new OperationTime(cuttingTime, rapidTime);
        }
    }
}

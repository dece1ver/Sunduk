using Sunduk.PWA.Infrastructure.Sequences.Base;
using Sunduk.PWA.Infrastructure.Sequences.Turning;
using Sunduk.PWA.Infrastructure.Templates;
using Sunduk.PWA.Infrastructure.Tools.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        public static int Round(this int value, int rounder = 10)
        {
            if (value < rounder) return value;
            return value / rounder * rounder;
        }

        /// <summary>
        /// Округляет
        /// </summary>
        /// <param name="rounder">Значение округления</param>
        /// <returns>Радиан</returns>
        public static int Round(this double value, int rounder = 10)
        {
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

        public static double OperationTime(this HighSpeedDrillingSequence highSpeedDrillingSequence, Material material)
        {
            var fullLength = (Math.Abs(highSpeedDrillingSequence.EndZ) + Math.Abs(highSpeedDrillingSequence.StartZ));
            var feed = Operation.DrillFeed(Machine.L230A, material, highSpeedDrillingSequence.Tool);
            var speed = Operation.DrillCuttingSpeed(material, highSpeedDrillingSequence.Tool);
            return (60 * Math.PI * fullLength * highSpeedDrillingSequence.Tool.Diameter) / 
                   (1000 * feed * speed);
        }

        
        public static double OperationTime(this PeckDeepDrillingSequence peckDeepDrillingSequence, Material material)
        {
            var fullLength = (Math.Abs(peckDeepDrillingSequence.EndZ) + Math.Abs(peckDeepDrillingSequence.StartZ));
            var steps = Math.Round(fullLength / peckDeepDrillingSequence.Depth, MidpointRounding.ToPositiveInfinity);
            var stepLength = peckDeepDrillingSequence.Depth + Operation.Escaping();
            double currentLength = 0;
            // время резания
            var result = steps *
                         (60 * Math.PI * (stepLength + Operation.Escaping()) * peckDeepDrillingSequence.Tool.Diameter) /
                         (1000 * Operation.DrillFeed(Machine.L230A, material, peckDeepDrillingSequence.Tool) * Operation.DrillCuttingSpeed(material, peckDeepDrillingSequence.Tool));
            // время ввода/вывода сверла
            for (int i = 0; i < steps; i++)
            {
                currentLength += stepLength;
                result += 2 * (60 * (currentLength)) / Operation.RapidSpeed();
            }

            return result;
        }

        public static double OperationTime(this PeckDrillingSequence peckDrillingSequence, Material material)
        {
            var steps = Math.Round((Math.Abs(peckDrillingSequence.EndZ) + Math.Abs(peckDrillingSequence.StartZ)) /
                                   peckDrillingSequence.Depth, MidpointRounding.ToPositiveInfinity);
            var stepLength = peckDrillingSequence.Depth + Operation.Escaping();
            return steps *
                   (60 * Math.PI * (stepLength) * peckDrillingSequence.Tool.Diameter) 
                   /
                   (1000 * Operation.DrillFeed(Machine.L230A, material, peckDrillingSequence.Tool) * Operation.DrillCuttingSpeed(material, peckDrillingSequence.Tool)) + 
                   steps * 
                   (60 * Operation.Escaping()) / Operation.RapidSpeed();
        }

    }
}

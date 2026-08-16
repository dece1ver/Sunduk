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
using Sunduk.PWA.Infrastructure.Sequences;
using Sunduk.Geometry;

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
        /// Общее время обработки
        /// </summary>
        public static OperationTime FullOperationTime(this List<Sequence> sequences)
        {
            if (sequences is null || sequences.Count < 1) return new OperationTime(0,0);
            var cuttingTime = sequences.Sum(x => x.MachineTime.CuttingTime);
            var rapidTime = sequences.Sum(x => x.MachineTime.RapidTime);
            var sequencesWithRapidMovement = sequences.Count(x => x is 
                not HeaderSequence and 
                not SafetyStringSequence and 
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

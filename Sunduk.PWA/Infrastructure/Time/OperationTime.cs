namespace Sunduk.PWA.Infrastructure.Time
{
    public class OperationTime
    {
        public double CuttingTime { get; set; }
        public double RapidTime { get; set; }
        public double FullTime => CuttingTime + RapidTime;

        public OperationTime(double cuttingTime, double rapidTime)
        {
            CuttingTime = cuttingTime;
            RapidTime = rapidTime;
        }
    }
}

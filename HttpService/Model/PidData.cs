namespace FanRemote.Model
{
    public class PidData
    {
        public DateTimeOffset Timestamp { get; set; }
        public int Temp { get; set; }
        public int Target { get; set; }
        public double Proportional { get; set; }
        public double Integral { get; set; }
        public double Derivative { get; set; }

        public int Speed { get; set; }

        public int Error
        {
            get
            {
                if (Temp > Target)
                    return Temp - Target;
                else
                    return 0;
            }
        }
        public bool InError
        {
            get
            {
                return Error > 0;
            }
        }
    }
}
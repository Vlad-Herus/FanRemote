namespace FanRemote.Services
{
    public interface ISpeedControl
    {
        // Returns fan speed between 0 and 255
        public int GetSpeed();
    }
}
namespace FanRemote.Services
{
    public interface IGpuTempSensor
    {
        Task<int> GetGpuTempInC();
    }
}
namespace FanRemote.Interfaces;

public interface IGpuTempSensor
{
    Task<int> GetGpuTempInC(CancellationToken cancellationToken);
}
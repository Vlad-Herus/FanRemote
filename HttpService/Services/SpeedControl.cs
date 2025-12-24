using FanRemote.Services;
using Microsoft.Extensions.Options;

public class SpeedControl : ISpeedControl
{
    private readonly IGpuTempHistoryStore _gpuTempHistoryStore;
    private readonly FanControlOptions _fanControlOptions;

    public SpeedControl(IGpuTempHistoryStore gpuTempHistoryStore, IOptionsSnapshot<FanControlOptions> fanControlOptions)
    {
        _gpuTempHistoryStore = gpuTempHistoryStore;
        _fanControlOptions = fanControlOptions.Value;
    }

    public int GetSpeed()
    {
        var temps = _gpuTempHistoryStore.GetTemps();
        if (temps == null || temps.Count() < 2)
        {
            return 0;
        }

        var currentTemp = temps.First();

        var proportional = currentTemp - _fanControlOptions.GpuTempCeiling;
        var integral = temps.Sum(temp => temp - _fanControlOptions.GpuTempCeiling);
        var derivative = temps.First() - temps.Skip(1).First();



        return 0;
    }
}
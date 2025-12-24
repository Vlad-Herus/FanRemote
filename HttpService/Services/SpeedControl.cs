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

        var target = _fanControlOptions.GpuTempCeiling;
        Func<int, int> getError = input => input - target;
        var currentTemp = temps.First();
        var previousTemp = temps.Skip(1).First();

        var proportional = getError(currentTemp);
        var integral = temps.Sum(temp => getError(temp));
        var derivative = getError(currentTemp) - getError(previousTemp);



        return 0;
    }
}
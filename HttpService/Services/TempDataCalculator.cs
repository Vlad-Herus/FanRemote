using FanRemote.Interfaces;
using FanRemote.Model;
using FanRemote.Services;
using Microsoft.Extensions.Options;

public class TempDataCalculator : ITempDataCalculator
{
    private readonly IGpuTempSensor _gpuTempSensor;
    private readonly FanControlOptions _fanControlOptions;

    public TempDataCalculator(
        IGpuTempSensor gpuTempSensor,
        IOptionsMonitor<FanControlOptions> fanControlOptions)
    {
        _gpuTempSensor = gpuTempSensor;
        _fanControlOptions = fanControlOptions.CurrentValue;
    }

    public async Task<TempData> Calculate(IEnumerable<TempData> historicalData, CancellationToken cancellationToken)
    {
        var pids = historicalData.OrderByDescending(pid => pid.Timestamp);
        var temp = await _gpuTempSensor.GetGpuTempInC(cancellationToken);
        var target = _fanControlOptions.TempFloor;

        return new TempData
        {
            Timestamp = DateTimeOffset.UtcNow,
            Temp = temp,
            Target = target
        };
    }
}

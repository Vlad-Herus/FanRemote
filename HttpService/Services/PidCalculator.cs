using FanRemote.Interfaces;
using FanRemote.Model;
using FanRemote.Services;
using Microsoft.Extensions.Options;

public class PidCalculator : IPidCalculator
{
    private readonly IGpuTempSensor _gpuTempSensor;
    private readonly FanControlOptions _fanControlOptions;

    public PidCalculator(
        IGpuTempSensor gpuTempSensor,
        IOptionsMonitor<FanControlOptions> fanControlOptions)
    {
        _gpuTempSensor = gpuTempSensor;
        _fanControlOptions = fanControlOptions.CurrentValue;
    }

    public async Task<PidData> Calculate(IEnumerable<PidData> historicalData, CancellationToken cancellationToken)
    {
        var pids = historicalData.OrderByDescending(pid => pid.Timestamp);
        var temp = await _gpuTempSensor.GetGpuTempInC(cancellationToken);
        var target = _fanControlOptions.GpuTempCeiling;

        Func<PidData, int> getErrorPid = input => input.Temp - target;
        Func<int, int> getError = input => input - target;

        return new PidData
        {
            Timestamp = DateTimeOffset.UtcNow,
            Temp = temp,
            Target = target,
            Proportional = getError(temp),
            Integral = pids.Sum(pid => getErrorPid(pid)),
            Derivative = pids.Any()
                 ? getError(temp) - getErrorPid(pids.First())
                 : 0
        };
    }
}

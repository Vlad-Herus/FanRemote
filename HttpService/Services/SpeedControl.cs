using FanRemote.Interfaces;
using FanRemote.Model;
using FanRemote.Services;
using Microsoft.Extensions.Options;

public class SpeedControl : ISpeedControl
{
    const int OFF_SPEED = 0;
    const int MAX_SPEED = 255;

    private readonly IOptionsMonitor<FanControlOptions> _fanControlOptionsMonitor;
    private readonly FanControlConfiguration _fanControlConfiguration;
    private int currentSpeed = 0;

    public SpeedControl(
        IOptionsMonitor<FanControlOptions> fanControlOptionsMonitor,
        FanControlConfiguration fanControlConfiguration
    )
    {
        _fanControlOptionsMonitor = fanControlOptionsMonitor;
        _fanControlConfiguration = fanControlConfiguration;
    }

    public int GetSpeed(TempData TempData)
    {
        if (_fanControlConfiguration.ForcedSpeed is int forcedSpeed)
        {
            currentSpeed = 0;
            return forcedSpeed;
        }

        var options = _fanControlOptionsMonitor.CurrentValue;

        if (TempData.Temp >= options.TempCeiling)
        {
            currentSpeed = MAX_SPEED;
            return currentSpeed;
        }

        var maxSpeedForTemp = CalculateMaxSpeedForTemp(TempData.Temp, options);

        var desiredIncrement = MAX_SPEED * (options.StepPercentage / 100d);
        var desiredSpeed = currentSpeed + (int)Math.Floor(desiredIncrement);

        if (desiredSpeed >= maxSpeedForTemp)
            currentSpeed = maxSpeedForTemp;
        else
            currentSpeed = desiredSpeed;

        return currentSpeed;

    }

    private static int CalculateMaxSpeedForTemp(int temp, FanControlOptions options)
    {
        if (temp >= options.TempCeiling)
            return MAX_SPEED;

        if (temp <= options.TempFloor)
            return OFF_SPEED;

        double error = temp - options.TempFloor;
        double maxError = options.TempCeiling - options.TempFloor;
        var maxSpeedFraction = error / maxError;
        int maxSpeed = (int)Math.Floor(MAX_SPEED * maxSpeedFraction);

        return maxSpeed;
    }
}
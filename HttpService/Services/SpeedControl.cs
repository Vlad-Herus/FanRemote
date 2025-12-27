using FanRemote.Interfaces;
using FanRemote.Model;
using FanRemote.Services;

public class SpeedControl : ISpeedControl
{
    private readonly PidConfiguration _pidConfiguration;

    public SpeedControl(PidConfiguration pidConfiguration)
    {
        _pidConfiguration = pidConfiguration;
    }

    public int GetSpeed(PidData pidData)
    {
        var result = pidData.Proportional * _pidConfiguration.Proportional;
        result += pidData.Derivative * _pidConfiguration.Derivative;
        result += pidData.Integral * _pidConfiguration.Integral;
        return (int)result;
    }
}
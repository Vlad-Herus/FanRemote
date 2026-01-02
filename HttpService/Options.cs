public class FanControlOptions
{
    public int TempFloor { get; set; }
    public int TempCeiling { get; set; }
    public int StepPercentage { get; set; }
    public int StepIntervalSeconds { get; set; }
}

public class NvidiaSmiOptions()
{
    public string? MvidiaSmiExeLocation { get; set; }
}

public class FanControlConfiguration
{
    public int? ForcedSpeed = null;
}
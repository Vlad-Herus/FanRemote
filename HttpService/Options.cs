public class FanControlOptions
{
    public int GpuTempCeiling { get; set; }
    public int GpuTempRecoveryThreshold { get; set; }
    public int CpuTempCeiling { get; set; }
    public int FanSpeed { get; set; }
}

public class NvidiaSmiOptions()
{
    public string? MvidiaSmiExeLocation { get; set; }
}

public class PidOptions
{
    public double Proportional { get; set; }
    public double Integral { get; set; }
    public double Derivative { get; set; }
}

public class PidConfiguration
{
    public double Proportional { get; set; }
    public double Integral { get; set; }
    public double Derivative { get; set; }
}
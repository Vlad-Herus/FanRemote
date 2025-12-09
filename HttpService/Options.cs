public class FanControlOptions
{
    public int GpuTempCeiling { get; set; }
    public int GpuTempRecoveryThreshold { get; set; }
    public int CpuTempCeiling { get; set; }
}

public class NvidiaSmiOptions()
{
    public string? MvidiaSmiExeLocation { get; set; }
}
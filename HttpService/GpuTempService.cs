using System.Diagnostics;
using Microsoft.Extensions.Options;

public interface IGpuTempService
{
    Task<int> GetGpuTempInC();
}

public class GpuTempService : IGpuTempService
{
    private readonly NvidiaSmiOptions _nvidiaSmiOptions;
    private readonly ILogger<GpuTempService> _logger;

    public GpuTempService(IOptionsSnapshot<NvidiaSmiOptions> nvidiaSmiOptions, ILogger<GpuTempService> logger)
    {
        _nvidiaSmiOptions = nvidiaSmiOptions?.Value ?? throw new ArgumentNullException(nameof(nvidiaSmiOptions));
        _logger = logger;
    }

    public async Task<int> GetGpuTempInC()
    {
        if (File.Exists(_nvidiaSmiOptions.MvidiaSmiExeLocation) is false)
        {
            _logger.LogError($"{nameof(_nvidiaSmiOptions.MvidiaSmiExeLocation)} is misconfigured");
            return 0;
        }

        Process process = new Process();
        process.StartInfo.FileName = _nvidiaSmiOptions.MvidiaSmiExeLocation;
        process.StartInfo.Arguments = "--query-gpu=temperature.gpu --format=csv,noheader"; // Optional arguments
        process.StartInfo.CreateNoWindow = true;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardOutput  = true;
        process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

        try
        {
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit(); // Optionally wait for the process to complete
            
            if (int.TryParse(output, out int gpuTemp))
            {
                return gpuTemp;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error starting process: {ex.Message}");
        }
        
        return 0;
    }
}
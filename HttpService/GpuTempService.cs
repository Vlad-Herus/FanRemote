using System.Diagnostics;
using Microsoft.Extensions.Options;

public interface IGpuTempService
{
    Task<int> GetGpuTempInC();
}

public class GpuTempService : IGpuTempService
{
    private readonly NvidiaSmiOptions _nvidiaSmiOptions;

    public GpuTempService(IOptions<NvidiaSmiOptions> nvidiaSmiOptions)
    {
        _nvidiaSmiOptions = nvidiaSmiOptions?.Value ?? throw new ArgumentNullException(nameof(nvidiaSmiOptions));
    }

    public async Task<int> GetGpuTempInC()
    {
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
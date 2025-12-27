using System.Diagnostics;
using FanRemote.Interfaces;
using Microsoft.Extensions.Options;

namespace FanRemote.Services
{
    public class GpuTempSensor : IGpuTempSensor
    {
        private readonly NvidiaSmiOptions _nvidiaSmiOptions;
        private readonly ILogger<GpuTempSensor> _logger;

        public GpuTempSensor(IOptionsMonitor<NvidiaSmiOptions> nvidiaSmiOptions, ILogger<GpuTempSensor> logger)
        {
            _nvidiaSmiOptions = nvidiaSmiOptions?.CurrentValue ?? throw new ArgumentNullException(nameof(nvidiaSmiOptions));
            _logger = logger;
        }

        public async Task<int> GetGpuTempInC(CancellationToken cancellationToken)
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
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

            try
            {
                process.Start();
                string output = await process.StandardOutput.ReadToEndAsync();
                await process.WaitForExitAsync(cancellationToken); // Optionally wait for the process to complete

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
}
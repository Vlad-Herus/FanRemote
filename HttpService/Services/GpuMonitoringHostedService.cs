namespace FanRemote.Services
{
    public class GpuMonitoringHostedService : IHostedService
    {
        private readonly IGpuTempHistoryStore _gpuTempHistoryStore;
        private readonly IGpuTempSensor _gpuTempSensor;
        private bool _stopping;

        public GpuMonitoringHostedService(IGpuTempHistoryStore gpuTempHistoryStore, IGpuTempSensor gpuTempSensor)
        {
            _gpuTempHistoryStore = gpuTempHistoryStore;
            _gpuTempSensor = gpuTempSensor;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            while (_stopping is false)
            {
                var temp = await _gpuTempSensor.GetGpuTempInC();
                _gpuTempHistoryStore.LogTemp(temp);

                await Task.Delay(10 * 1000);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _stopping = true;
            return Task.CompletedTask;
        }
    }
}
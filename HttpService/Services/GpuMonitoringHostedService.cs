namespace FanRemote.Services
{
    public class GpuMonitoringHostedService : BackgroundService
    {
        private readonly IGpuTempHistoryStore _gpuTempHistoryStore;
        private readonly IGpuTempSensor _gpuTempSensor;
        private bool _stopping;

        public GpuMonitoringHostedService(IGpuTempHistoryStore gpuTempHistoryStore, IGpuTempSensor gpuTempSensor)
        {
            _gpuTempHistoryStore = gpuTempHistoryStore;
            _gpuTempSensor = gpuTempSensor;
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _stopping = true;
            return Task.CompletedTask;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (_stopping is false)
            {
                var temp = await _gpuTempSensor.GetGpuTempInC();
                _gpuTempHistoryStore.LogTemp(temp);

                await Task.Delay(10 * 1000);
            }
        }
    }
}
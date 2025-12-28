using FanRemote.Interfaces;

namespace FanRemote.Services
{
    public class GpuMonitoringHostedService : BackgroundService
    {
        private readonly ITempHistoryStore _TempHistoryStore;
        private readonly ITempDataCalculator _TempDataCalculator;
        private readonly ISpeedControl _speedControl;
        private bool _stopping;

        public GpuMonitoringHostedService(
            ITempHistoryStore TempHistoryStore, 
            ITempDataCalculator TempDataCalculator,
            ISpeedControl speedControl)
        {
            _TempHistoryStore = TempHistoryStore;
            _TempDataCalculator = TempDataCalculator;
            _speedControl = speedControl;
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
                var pid = await _TempDataCalculator.Calculate(_TempHistoryStore.GetTemps(), stoppingToken);
                pid.Speed = _speedControl.GetSpeed(pid);
                _TempHistoryStore.LogTemp(pid);

                if (pid.InError)
                    await Task.Delay(1 * 1000);
                else
                    await Task.Delay(10 * 1000);
            }
        }
    }
}
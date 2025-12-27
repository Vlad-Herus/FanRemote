using FanRemote.Interfaces;

namespace FanRemote.Services
{
    public class GpuMonitoringHostedService : BackgroundService
    {
        private readonly IPidHistoryStore _pidHistoryStore;
        private readonly IPidCalculator _pidCalculator;
        private readonly ISpeedControl _speedControl;
        private bool _stopping;

        public GpuMonitoringHostedService(
            IPidHistoryStore pidHistoryStore, 
            IPidCalculator pidCalculator,
            ISpeedControl speedControl)
        {
            _pidHistoryStore = pidHistoryStore;
            _pidCalculator = pidCalculator;
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
                var pid = await _pidCalculator.Calculate(_pidHistoryStore.GetTemps(), stoppingToken);
                pid.Speed = _speedControl.GetSpeed(pid);
                _pidHistoryStore.LogTemp(pid);

                if (pid.InError)
                    await Task.Delay(1 * 1000);
                else
                    await Task.Delay(10 * 1000);
            }
        }
    }
}
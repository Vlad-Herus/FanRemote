using FanRemote.Interfaces;
using Microsoft.Extensions.Options;

namespace FanRemote.Services
{
    public class GpuMonitoringHostedService : BackgroundService
    {
        private readonly ITempHistoryStore _TempHistoryStore;
        private readonly ITempDataCalculator _TempDataCalculator;
        private readonly ISpeedControl _speedControl;
        private readonly IOptionsMonitor<FanControlOptions> _fanControlOptions;
        private bool _stopping;

        public GpuMonitoringHostedService(
            ITempHistoryStore TempHistoryStore,
            ITempDataCalculator TempDataCalculator,
            ISpeedControl speedControl,
            IOptionsMonitor<FanControlOptions> fanControlOptions)
        {
            _TempHistoryStore = TempHistoryStore;
            _TempDataCalculator = TempDataCalculator;
            _speedControl = speedControl;
            _fanControlOptions = fanControlOptions;
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
                var options =_fanControlOptions.CurrentValue;
                var pid = await _TempDataCalculator.Calculate(_TempHistoryStore.GetTemps(), stoppingToken);
                pid.Speed = _speedControl.GetSpeed(pid);
                _TempHistoryStore.LogTemp(pid);

                await Task.Delay(options.StepIntervalSeconds * 1000);
            }
        }
    }
}
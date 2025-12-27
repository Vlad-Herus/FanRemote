using FanRemote.Model;

namespace FanRemote.Interfaces;

public interface IPidCalculator
{
    public Task<PidData> Calculate(IEnumerable<PidData> historicalData, CancellationToken cancellationToken);
}

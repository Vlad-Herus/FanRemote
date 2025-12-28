using FanRemote.Model;

namespace FanRemote.Interfaces;

public interface ITempDataCalculator
{
    public Task<TempData> Calculate(IEnumerable<TempData> historicalData, CancellationToken cancellationToken);
}

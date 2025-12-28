using FanRemote.Model;

namespace FanRemote.Interfaces;

public interface ITempHistoryStore
{
    void LogTemp(TempData data);

    /// <summary>
    /// History of GPU temp change. 
    /// </summary>
    /// <returns>First item is the most recent reading. Last item is the oldest.</returns>
    IEnumerable<TempData> GetTemps();
}
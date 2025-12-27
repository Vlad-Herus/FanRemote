using FanRemote.Model;

namespace FanRemote.Interfaces;

public interface IPidHistoryStore
{
    void LogTemp(PidData data);

    /// <summary>
    /// History of GPU temp change. 
    /// </summary>
    /// <returns>First item is the most recent reading. Last item is the oldest.</returns>
    IEnumerable<PidData> GetTemps();
}
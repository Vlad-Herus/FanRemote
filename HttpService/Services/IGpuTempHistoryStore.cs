namespace FanRemote.Services
{
    public interface IGpuTempHistoryStore
    {
        void LogTemp(int temp);

        /// <summary>
        /// History of GPU temp change. 
        /// </summary>
        /// <returns>First item is the most recent reading. Last item is the oldest.</returns>
        IEnumerable<int> GetTemps();
    }
}
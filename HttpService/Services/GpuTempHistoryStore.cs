namespace FanRemote.Services
{
    public class GpuTempHistoryStore : IGpuTempHistoryStore
    {
        private const int MaxValues = 6;

        List<int> _Temps = new();
        public void LogTemp(int temp)
        {
            lock (_Temps)
            {
                _Temps.Add(temp);
                if (_Temps.Count > MaxValues)
                {
                    _Temps.Remove(_Temps.ElementAt(0));
                }
            }
        }

        public IEnumerable<int> GetTemps()
        {
            lock (_Temps)
            {
                return _Temps.ToArray().Reverse();
            }
        }

    }
}
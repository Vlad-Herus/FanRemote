using FanRemote.Interfaces;
using FanRemote.Model;

namespace FanRemote.Services
{
    public class PidHistoryStore : IPidHistoryStore
    {
        private const int MaxValues = 6;

        List<PidData> _Datas = new();
        public void LogTemp(PidData data)
        {
            lock (_Datas)
            {
                _Datas.Add(data);
                if (_Datas.Count > MaxValues)
                {
                    _Datas.Remove(_Datas.ElementAt(0));
                }
            }
        }

        public IEnumerable<PidData> GetTemps()
        {
            lock (_Datas)
            {
                return _Datas.ToArray().Reverse();
            }
        }

    }
}
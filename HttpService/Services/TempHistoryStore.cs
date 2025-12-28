using FanRemote.Interfaces;
using FanRemote.Model;

namespace FanRemote.Services
{
    public class TempHistoryStore : ITempHistoryStore
    {
        private const int MaxValues = 60;

        List<TempData> _Datas = new();
        public void LogTemp(TempData data)
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

        public IEnumerable<TempData> GetTemps()
        {
            lock (_Datas)
            {
                return _Datas.ToArray().Reverse();
            }
        }

    }
}
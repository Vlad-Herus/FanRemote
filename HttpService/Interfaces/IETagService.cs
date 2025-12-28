using FanRemote.Model;

namespace FanRemote.Interfaces;

public interface IETagService
{
    public string? GetETag(IEnumerable<TempData> TempDatas);
    public IEnumerable<TempData> Filter(IEnumerable<TempData> TempDatas, string? eTag);
}
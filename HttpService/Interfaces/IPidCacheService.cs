using FanRemote.Model;

namespace FanRemote.Interfaces;

public interface IPidCacheService
{
    public string? GetETag(IEnumerable<PidData> pidDatas);
    public IEnumerable<PidData> Filter(IEnumerable<PidData> pidDatas, string? eTag);
}
using System.Buffers.Text;
using System.Security.Cryptography;
using FanRemote.Interfaces;
using FanRemote.Model;
using Microsoft.VisualBasic;

namespace FanRemote.Services;

public class PidCacheService : IPidCacheService
{
    public IEnumerable<PidData> Filter(IEnumerable<PidData> pidDatas, string? eTag)
    {
        if (string.IsNullOrWhiteSpace(eTag))
            return pidDatas;

        var matchingItem = pidDatas.FirstOrDefault(data => GetETag(data) == eTag);

        if (matchingItem is null)
            return pidDatas;
        else
            return pidDatas.Where(data => data.Timestamp > matchingItem.Timestamp);
    }

    public string? GetETag(IEnumerable<PidData> pidDatas)
    {
        var latest = pidDatas
            .OrderByDescending(data => data.Timestamp)
            .FirstOrDefault();

        if (latest is null)
            return null;
        else
            return GetETag(latest);
    }

    private string GetETag(PidData pidData)
    {
        byte[] bytes = GetBytes(pidData.Timestamp);
        var hash = MD5.Create().ComputeHash(bytes);
        var eTag = Convert.ToBase64String(hash);

        return eTag;
    }

    private byte[] GetBytes(DateTimeOffset dateTimeOffset)
    {
        byte[] ticksBytes = BitConverter.GetBytes(dateTimeOffset.Ticks);

        // Convert Offset.TotalMinutes (short/Int16, 2 bytes) to bytes
        short offsetMinutes = (short)dateTimeOffset.Offset.TotalMinutes;
        byte[] offsetBytes = BitConverter.GetBytes(offsetMinutes);

        // Combine and return the 10 bytes
        return ticksBytes.Concat(offsetBytes).ToArray();
    }
}
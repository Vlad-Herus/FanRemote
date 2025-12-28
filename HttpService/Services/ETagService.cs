using System.Buffers.Text;
using System.Security.Cryptography;
using FanRemote.Interfaces;
using FanRemote.Model;
using Microsoft.VisualBasic;

namespace FanRemote.Services;

public class ETagService : IETagService
{
    public IEnumerable<TempData> Filter(IEnumerable<TempData> TempDatas, string? eTag)
    {
        if (string.IsNullOrWhiteSpace(eTag))
            return TempDatas;

        var matchingItem = TempDatas.FirstOrDefault(data => GetETag(data) == eTag);

        if (matchingItem is null)
            return TempDatas;
        else
            return TempDatas.Where(data => data.Timestamp > matchingItem.Timestamp);
    }

    public string? GetETag(IEnumerable<TempData> TempDatas)
    {
        var latest = TempDatas
            .OrderByDescending(data => data.Timestamp)
            .FirstOrDefault();

        if (latest is null)
            return null;
        else
            return GetETag(latest);
    }

    private string GetETag(TempData TempData)
    {
        byte[] bytes = GetBytes(TempData.Timestamp);
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
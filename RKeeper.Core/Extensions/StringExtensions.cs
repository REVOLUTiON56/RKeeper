using System;
using System.Runtime.InteropServices;
using CommunityToolkit.HighPerformance.Buffers;

namespace RKeeper.Core.Extensions;

public static class StringExtensions
{
    public static string? ToUpperString(this object? obj)
    {
        var defaultString = obj?.ToString();
        if (defaultString == null)
        {
            return null;
        }

        var chars = MemoryMarshal.CreateSpan(ref MemoryMarshal.GetReference(defaultString.AsSpan()), defaultString.Length);
        for (var i = 0; i < chars.Length; i++)
        {
            chars[i] = char.ToUpperInvariant(chars[i]);
        }

        return StringPool.Shared.GetOrAdd(chars);
    }
}

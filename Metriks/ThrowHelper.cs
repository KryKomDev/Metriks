//
// Metriks
//  Copyright (c) MIT License 2025, KryKom & ZlomenyMesic
//

using System.Diagnostics.CodeAnalysis;

namespace Metriks;

internal static class ThrowHelper {
    
    public static void ThrowIfLt(IComparable a, IComparable b, Exception? exception = null) {
        if (a.CompareTo(b) == -1) 
            throw exception ?? new Exception();
    }
    
    public static void ThrowIfLeq(IComparable a, IComparable b, Exception? exception = null) {
        if (a.CompareTo(b) <= 0) 
            throw exception ?? new Exception();
    }

    public static void ThrowIfGt(IComparable a, IComparable b, Exception? exception = null) {
        if (a.CompareTo(b) == 1) 
            throw exception ?? new Exception();
    }
    
    public static void ThrowIfGeq(IComparable a, IComparable b, Exception? exception = null) {
        if (a.CompareTo(b) >= 0) 
            throw exception ?? new Exception();
    }

    public static void ThrowIf([DoesNotReturnIf(true)] bool cond, Exception? exception = null) {
        if (cond) 
            throw exception ?? new Exception();
    }

    public static void ThrowIfNegative(int value, Exception? exception = null) {
        if (value < 0) 
            throw exception ?? new Exception();
    }
    
    public static void ThrowIfNull([NotNull] object? obj, Exception? exception = null) {
        if (obj is null) 
            throw exception ?? new ArgumentNullException();
    }

    public static void ThrowMaybe(Exception? exception = null) {
        if (new Random().Next() % 2 == 0) 
            throw exception ?? new Exception();
    }
}
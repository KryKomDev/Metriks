//
// Metriks
//  Copyright (c) MIT License 2025, KryKom & ZlomenyMesic
//

using System;
using System.Diagnostics.CodeAnalysis;

namespace Metriks;

internal static class ThrowHelper {
    
    public static void ThrowIfLessThan(int a, int b, Exception? exception = null) {
        if (a < b) 
            throw exception ?? new ArgumentOutOfRangeException();
    }

    public static void ThrowIf([DoesNotReturnIf(true)] bool cond, Exception? exception = null) {
        if (cond) 
            throw exception ?? new Exception();
    }

    
    public static void ThrowIfNull([NotNull] object? obj, Exception? exception = null) {
        if (obj is null) 
            throw exception ?? new ArgumentNullException();
    }
}
//
// Metriks
//  Copyright (c) MIT License 2025, KryKom & ZlomenyMesic and others... 
//

using System;

namespace Metriks;

public static class Extensions {
    public static void ThrowIfLessThan(int a, int b, Exception? exception = null) {
        if (a < b) throw exception ?? new ArgumentOutOfRangeException();
    }

    public static void ThrowIf(bool cond, Exception? exception = null) {
        if (cond) throw exception ?? new ArgumentNullException();
    }

    public static bool Contains<T>(this T[] array, T element) {
        for (int i = 0; i < array.Length; i++)
            if (array[i]!.Equals(element)) 
                return true;
        
        return false;
    }

    public static void Fill<T>(T[] array, T element) {
        for (int i = 0; i < array.Length; i++) {
            array[i] = element;
        }
    }
}
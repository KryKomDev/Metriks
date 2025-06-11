//
// Metriks
//  Copyright (c) MIT License 2025, KryKom & ZlomenyMesic
//

using System;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace Metriks;

public static class Extensions {

#region COMPATIBILITY
    
    /// <summary>
    /// Returns whether a specified element is contained in the supplied array.
    /// </summary>
    internal static bool Contains<T>(this T[] array, T element) {
        for (int i = 0; i < array.Length; i++)
            if (array[i]!.Equals(element)) 
                return true;
        
        return false;
    }

    /// <summary>
    /// Fills the array using the supplied element.
    /// </summary>
    internal static void Fill<T>(T[] array, T element) {
        for (int i = 0; i < array.Length; i++) {
            array[i] = element;
        }
    }

    /// <summary>
    /// Clamps the specified integer between the maximum and minimum values.
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Clamp(this int value, int min, int max) => value < min ? min : value > max ? max : value;

    /// <summary>
    /// Clamps the specified <see cref="IComparable{T}"/> between the maximum and minimum values.
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Clamp<T>(this T value, T min, T max) where T : IComparable<T> => 
        value.CompareTo(min) < 0 ? min : 
        value.CompareTo(max) > 0 ? max : value;

    /// <summary>
    /// Returns a new random double floating-point number in the specified range.
    /// </summary>
    /// <param name="min">The range minimum (inclusive)</param>
    /// <param name="max">The range maximum (exclusive)</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double NextDouble(this Random random, double min, double max)
        => random.NextDouble() * (max - min) / 2 + min;

    /// <summary>
    /// Returns a new random 64-bit unsigned integer.
    /// </summary>
    public static ulong NextUInt64(this Random random) {
        byte[] bytes = new byte[8];
        random.NextBytes(bytes);
        return BitConverter.ToUInt64(bytes, 0);
    }

    /// <summary>
    /// Fits the value into a smooth sigmoid curve.
    /// </summary>
    /// <param name="deviation">The maximum y-axis deviation of the curve</param>
    /// <param name="stretch"></param>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Sigmoid(this double value, double deviation, double stretch)
        => deviation * Math.Tanh(value / stretch);

    
#endregion

#region ARRAY_2D
    
    /// <summary>
    /// Returns the length of the first dimension of an array.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [Pure]
    public static int Len0<T>(this T[,] array) => array.GetLength(0);
    
    /// <summary>
    /// Returns the length of the second dimension of an array.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [Pure]
    public static int Len1<T>(this T[,] array) => array.GetLength(1);

    /// <summary>
    /// Fills the array using the supplied element.
    /// </summary>
    public static void Fill<T>(this T[,]? array, T value) {
        ThrowHelper.ThrowIfNull(array, new ArgumentNullException(nameof(array)));
        
        for (int i0 = 0; i0 < array.Len0(); i0++) {
            for (int i1 = 0; i1 < array.Len1(); i1++) {
                array[i0, i1] = value;
            }
        }
    }

    private const string NON_NEG_INDEX_MSG = "The supplied index must be non-negative.";
    
    /// <summary>
    /// Fills the array using the specified element in the region defined by the start indexes and counts.
    /// </summary>
    public static void Fill<T>(this T[,]? array, T value, int start0, int count0, int start1, int count1) {
        ThrowHelper.ThrowIfNull(array, new ArgumentNullException(nameof(array)));

        int end0 = start0 + count0;
        int end1 = start1 + count1;
        
        ThrowHelper.ThrowIf(start0 < 0, new ArgumentOutOfRangeException(nameof(start0), NON_NEG_INDEX_MSG));
        ThrowHelper.ThrowIf(start1 < 0, new ArgumentOutOfRangeException(nameof(start0), NON_NEG_INDEX_MSG));
        ThrowHelper.ThrowIf(end0 < 0, new ArgumentOutOfRangeException(nameof(start0), NON_NEG_INDEX_MSG));
        ThrowHelper.ThrowIf(end1 < 0, new ArgumentOutOfRangeException(nameof(start0), NON_NEG_INDEX_MSG));
        
        int len0 = array.Len0();
        int len1 = array.Len1();
        
        ThrowHelper.ThrowIf(start0 >= len0, new ArgumentOutOfRangeException(nameof(start0), NON_NEG_INDEX_MSG));
        ThrowHelper.ThrowIf(start1 >= len1, new ArgumentOutOfRangeException(nameof(start0), NON_NEG_INDEX_MSG));
        ThrowHelper.ThrowIf(end0 >= len0, new ArgumentOutOfRangeException(nameof(start0), NON_NEG_INDEX_MSG));
        ThrowHelper.ThrowIf(end1 >= len1, new ArgumentOutOfRangeException(nameof(start0), NON_NEG_INDEX_MSG));
        
        for (int i0 = start0; i0 <= end0; i0++) {
            for (int i1 = start1; i1 < end1; i1++) {
                array[i0, i1] = value;
            }
        }
    }

    /// <summary>
    /// Fills the array using the specified element in the region defined by the start indexes and counts.
    /// If any part of the subregion is out of bounds, the subregion will be set to not be out of bounds.
    /// </summary>
    public static void SafeFill<T>(this T[,]? array, T value, int start0, int count0, int start1, int count1) {
        ThrowHelper.ThrowIfNull(array, new ArgumentNullException(nameof(array)));
        
        start0 = start0.Clamp(0, array.Len0() - 1);
        int end0 = (start0 + count0).Clamp(0, array.Len0() - 1);
        start1 = start1.Clamp(0, array.Len1() - 1);
        int end1 = (start1 + count1).Clamp(0, array.Len1() - 1);

        for (int i0 = start0; i0 < end0; i0++) {
            for (int i1 = start1; i1 < end1; i1++) {
                array[i0, i1] = value;
            }
        }
    }
    
    /// <summary>
    /// Fills the array using the specified element in the region defined by the start and end indexes.
    /// </summary>
    public static void FillRange<T>(this T[,]? array, T value, int start0, int end0, int start1, int end1) {
        ThrowHelper.ThrowIfNull(array, new ArgumentNullException(nameof(array)));
        
        ThrowHelper.ThrowIf(start0 < 0, new ArgumentOutOfRangeException(nameof(start0), NON_NEG_INDEX_MSG));
        ThrowHelper.ThrowIf(start1 < 0, new ArgumentOutOfRangeException(nameof(start0), NON_NEG_INDEX_MSG));
        ThrowHelper.ThrowIf(end0 < 0, new ArgumentOutOfRangeException(nameof(start0), NON_NEG_INDEX_MSG));
        ThrowHelper.ThrowIf(end1 < 0, new ArgumentOutOfRangeException(nameof(start0), NON_NEG_INDEX_MSG));

        int len0 = array.Len0();
        int len1 = array.Len1();
        
        ThrowHelper.ThrowIf(start0 >= len0, new ArgumentOutOfRangeException(nameof(start0), NON_NEG_INDEX_MSG));
        ThrowHelper.ThrowIf(start1 >= len1, new ArgumentOutOfRangeException(nameof(start0), NON_NEG_INDEX_MSG));
        ThrowHelper.ThrowIf(end0 >= len0, new ArgumentOutOfRangeException(nameof(start0), NON_NEG_INDEX_MSG));
        ThrowHelper.ThrowIf(end1 >= len1, new ArgumentOutOfRangeException(nameof(start0), NON_NEG_INDEX_MSG));

        for (int i0 = start0; i0 <= end0; i0++) {
            for (int i1 = start1; i1 < end1; i1++) {
                array[i0, i1] = value;
            }
        }
    }

    /// <summary>
    /// Fills the array using the specified element in the region defined by the start and end indexes.
    /// If any part of the subregion is out of bounds, the subregion will be set to not be out of bounds.
    /// </summary>
    public static void SafeFillRange<T>(this T[,]? array, T value, int start0, int end0, int start1, int end1) {
        ThrowHelper.ThrowIfNull(array, new ArgumentNullException(nameof(array)));
        
        start0 = start0.Clamp(0, array.Len0() - 1);
        end0 = end0.Clamp(0, array.Len0() - 1);
        start1 = start1.Clamp(0, array.Len1() - 1);
        end1 = end1.Clamp(0, array.Len1() - 1);

        for (int i0 = start0; i0 < end0; i0++) {
            for (int i1 = start1; i1 < end1; i1++) {
                array[i0, i1] = value;
            }
        }
    }

    private const string NON_NEG_SIZE_MSG = "The size of the array must be non-negative.";
    
    /// <summary>
    /// Resizes the array to the specified dimensions.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">one of the supplied dimensions is negative</exception>
    public static void Resize<T>([NotNull] ref T[,]? array, int newSize0, int newSize1) {
        ThrowHelper.ThrowIf(newSize0 < 0, new ArgumentOutOfRangeException(nameof(newSize0), NON_NEG_SIZE_MSG));
        ThrowHelper.ThrowIf(newSize1 < 0, new ArgumentOutOfRangeException(nameof(newSize1), NON_NEG_SIZE_MSG));
        
        var lArray = array;

        if (lArray is null) {
            array = new T[newSize0, newSize1];
            return;
        }

        // if the new size is the same as the old size, do nothing
        if (lArray.Len0() == newSize0 || lArray.Len1() == newSize1) {
            ThrowHelper.ThrowIfNull(array, new ArgumentNullException(nameof(array)));
            return;
        }
        
        // copy the actual array
        var newArray = new T[newSize0, newSize1];

        int d0Min = Math.Min(newSize0, lArray.Len0());
        int d1Min = Math.Min(newSize0, lArray.Len1());
        
        for (int i0 = 0; i0 < d0Min; i0++) {
            for (int i1 = 0; i1 < d1Min; i1++) {
                newArray[i0, i1] = lArray[i0, i1];
            }
        }
        
        // overwrite the old array
        array = newArray;
        ThrowHelper.ThrowIfNull(array, new ArgumentNullException(nameof(array)));
    }

    /// <summary>
    /// Clears the array.
    /// </summary>
    public static void Clear<T>([NotNull] T[,]? array) {
        ThrowHelper.ThrowIfNull(array, new ArgumentNullException(nameof(array)));

        for (int i0 = 0; i0 < array.Len0(); i0++) {
            for (int i1 = 0; i1 < array.Len1(); i1++) {
                array[i0, i1] = default!;
            }
        }
    }

    /// <summary>
    /// Clears the array in the specified region.
    /// </summary>
    public static void Clear<T>([NotNull] T[,]? array, int start0, int count0, int start1, int count1) {
        ThrowHelper.ThrowIfNull(array, new ArgumentNullException(nameof(array)));

        int end0 = start0 + count0;
        int end1 = start1 + count1;
        
        ThrowHelper.ThrowIf(start0 < 0, new ArgumentOutOfRangeException(nameof(start0), NON_NEG_INDEX_MSG));
        ThrowHelper.ThrowIf(start1 < 0, new ArgumentOutOfRangeException(nameof(start0), NON_NEG_INDEX_MSG));
        ThrowHelper.ThrowIf(end0 < 0, new ArgumentOutOfRangeException(nameof(start0), NON_NEG_INDEX_MSG));
        ThrowHelper.ThrowIf(end1 < 0, new ArgumentOutOfRangeException(nameof(start0), NON_NEG_INDEX_MSG));
        
        int len0 = array.Len0();
        int len1 = array.Len1();
        
        ThrowHelper.ThrowIf(start0 >= len0, new ArgumentOutOfRangeException(nameof(start0), NON_NEG_INDEX_MSG));
        ThrowHelper.ThrowIf(start1 >= len1, new ArgumentOutOfRangeException(nameof(start0), NON_NEG_INDEX_MSG));
        ThrowHelper.ThrowIf(end0 >= len0, new ArgumentOutOfRangeException(nameof(start0), NON_NEG_INDEX_MSG));
        ThrowHelper.ThrowIf(end1 >= len1, new ArgumentOutOfRangeException(nameof(start0), NON_NEG_INDEX_MSG));
        
        for (int i0 = start0; i0 <= end0; i0++) {
            for (int i1 = start1; i1 < end1; i1++) {
                array[i0, i1] = default!;
            }
        }
    }

    /// <summary>
    /// Clears the array in the specified region.
    /// </summary>
    public static void ClearRange<T>([NotNull] T[,]? array, int start0, int end0, int start1, int end1) {
        ThrowHelper.ThrowIfNull(array, new ArgumentNullException(nameof(array)));
        
        ThrowHelper.ThrowIf(start0 < 0, new ArgumentOutOfRangeException(nameof(start0), NON_NEG_INDEX_MSG));
        ThrowHelper.ThrowIf(start1 < 0, new ArgumentOutOfRangeException(nameof(start0), NON_NEG_INDEX_MSG));
        ThrowHelper.ThrowIf(end0 < 0, new ArgumentOutOfRangeException(nameof(start0), NON_NEG_INDEX_MSG));
        ThrowHelper.ThrowIf(end1 < 0, new ArgumentOutOfRangeException(nameof(start0), NON_NEG_INDEX_MSG));

        int len0 = array.Len0();
        int len1 = array.Len1();
        
        ThrowHelper.ThrowIf(start0 >= len0, new ArgumentOutOfRangeException(nameof(start0), NON_NEG_INDEX_MSG));
        ThrowHelper.ThrowIf(start1 >= len1, new ArgumentOutOfRangeException(nameof(start0), NON_NEG_INDEX_MSG));
        ThrowHelper.ThrowIf(end0 >= len0, new ArgumentOutOfRangeException(nameof(start0), NON_NEG_INDEX_MSG));
        ThrowHelper.ThrowIf(end1 >= len1, new ArgumentOutOfRangeException(nameof(start0), NON_NEG_INDEX_MSG));
        
        for (int i0 = start0; i0 <= end0; i0++) {
            for (int i1 = start1; i1 < end1; i1++) {
                array[i0, i1] = default!;
            }
        }
    }

    /// <summary>
    /// Converts the array into a jagged array.
    /// </summary>
    public static T[][] ToJagged<T>(this T[,]? array) {
        ThrowHelper.ThrowIfNull(array, new ArgumentNullException(nameof(array)));
        
        int s0 = array.GetLength(0);
        int s1 = array.GetLength(1);
        T[][] jagged = new T[s0][];
        
        for (int i0 = 0; i0 < s0; i0++) {
            jagged[i0] = new T[s1];
            for (int i1 = 0; i1 < s1; i1++) {
                jagged[i0][i1] = array[i0, i1];
            }
        }
        
        return jagged;
    }
    
#endregion

#region ARRAY_3D

    /// <summary>
    /// Returns the length of the first dimension of an array.
    /// </summary>
    public static int Len0<T>(this T[,,] array) => array.GetLength(0);
    
    /// <summary>
    /// Returns the length of the second dimension of an array.
    /// </summary>
    public static int Len1<T>(this T[,,] array) => array.GetLength(1);
    
    /// <summary>
    /// Returns the length of the third dimension of an array.
    /// </summary>
    public static int Len2<T>(this T[,,] array) => array.GetLength(2);
    
#endregion
}
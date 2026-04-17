//
// Metriks
//  Copyright (c) MIT License 2025, KryKom & ZlomenyMesic
//

namespace Metriks;

public static class ArrayManipulation {
    
    private const string INDEX_OUT_OF_BOUNDS = "The supplied index is out of bounds.";
    
#region COMPATIBILITY
    
    /// <summary>
    /// Returns whether a specified element is contained in the supplied array.
    /// </summary>
    [Pure]
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
    
    extension<T>(T[]? array) {
        
        /// <summary>
        /// Selects the elements in the specified range.
        /// </summary>
        [Pure]
        public T[] Range(int start, int end) {
            ThrowHelper.ThrowIfNull(array);
        
            ThrowHelper.ThrowIf(start < 0, new ArgumentOutOfRangeException(nameof(start), INDEX_OUT_OF_BOUNDS));
            ThrowHelper.ThrowIf(start >= array.Length, new ArgumentOutOfRangeException(nameof(start), INDEX_OUT_OF_BOUNDS));
            ThrowHelper.ThrowIf(end < 0, new ArgumentOutOfRangeException(nameof(end), INDEX_OUT_OF_BOUNDS));
            ThrowHelper.ThrowIf(end >= array.Length, new ArgumentOutOfRangeException(nameof(end), INDEX_OUT_OF_BOUNDS));
        
            List<T> list = new();

            for (int i = start; i < end; i++) {
                list.Add(array[i]);
            }
        
            return list.ToArray();
        }

        /// <summary>
        /// Selects the elements in the specified range clamped to the bounds. 
        /// </summary>
        [Pure]
        public T[] SafeRange(int start, int end) {
            ThrowHelper.ThrowIfNull(array);

            start = start.Clamp(0, array.Length - 1);
            end   = end.Clamp(0, array.Length - 1);
        
            List<T> list = new();

            for (int i = start; i < end; i++) {
                list.Add(array[i]);
            }
        
            return list.ToArray();
        }
    }
    
#endregion


    extension(Array) {
        public static void RevCpy(Array src, Array dst, int len) {
            for (int i = len - 1; i >= 0; i--) {
                dst.SetValue(src.GetValue(i), i);
            }
        }
    }
    
#region ARRAY_2D
    
    extension<T>(T[,]? array) {
        
        /// <summary>
        /// Fills the array using the supplied element.
        /// </summary>
        public void Fill(T value) {
            ThrowHelper.ThrowIfNull(array, new ArgumentNullException(nameof(array)));
        
            for (int i0 = 0; i0 < array.Len0; i0++) {
                for (int i1 = 0; i1 < array.Len1; i1++) {
                    array[i0, i1] = value;
                }
            }
        }

        /// <summary>
        /// Fills the array using the specified element in the region defined by the start indexes and counts.
        /// </summary>
        public void Fill(T value, int start0, int count0, int start1, int count1) {
            ThrowHelper.ThrowIfNull(array, new ArgumentNullException(nameof(array)));

            int end0 = start0 + count0;
            int end1 = start1 + count1;
        
            ThrowHelper.ThrowIf(start0 < 0, new ArgumentOutOfRangeException(nameof(start0), INDEX_OUT_OF_BOUNDS));
            ThrowHelper.ThrowIf(start1 < 0, new ArgumentOutOfRangeException(nameof(start0), INDEX_OUT_OF_BOUNDS));
            ThrowHelper.ThrowIf(end0 < 0, new ArgumentOutOfRangeException(nameof(start0), INDEX_OUT_OF_BOUNDS));
            ThrowHelper.ThrowIf(end1 < 0, new ArgumentOutOfRangeException(nameof(start0), INDEX_OUT_OF_BOUNDS));
        
            int len0 = array.Len0;
            int len1 = array.Len1;
        
            ThrowHelper.ThrowIf(start0 >= len0, new ArgumentOutOfRangeException(nameof(start0), INDEX_OUT_OF_BOUNDS));
            ThrowHelper.ThrowIf(start1 >= len1, new ArgumentOutOfRangeException(nameof(start0), INDEX_OUT_OF_BOUNDS));
            ThrowHelper.ThrowIf(end0 >= len0, new ArgumentOutOfRangeException(nameof(start0), INDEX_OUT_OF_BOUNDS));
            ThrowHelper.ThrowIf(end1 >= len1, new ArgumentOutOfRangeException(nameof(start0), INDEX_OUT_OF_BOUNDS));
        
            for (int i0 = start0; i0 <= end0; i0++) {
                for (int i1 = start1; i1 < end1; i1++) {
                    array[i0, i1] = value;
                }
            }
        }

        /// <summary>
        /// Fills the array using the specified element in the region defined by the start
        /// indexes and counts clamped to the bounds.
        /// </summary>
        public void SafeFill(T value, int start0, int count0, int start1, int count1) {
            ThrowHelper.ThrowIfNull(array, new ArgumentNullException(nameof(array)));
        
            start0   = start0.Clamp(0, array.Len0 - 1);
            int end0 = (start0 + count0).Clamp(0, array.Len0 - 1);
            start1   = start1.Clamp(0, array.Len1 - 1);
            int end1 = (start1 + count1).Clamp(0, array.Len1 - 1);

            for (int i0 = start0; i0 < end0; i0++) {
                for (int i1 = start1; i1 < end1; i1++) {
                    array[i0, i1] = value;
                }
            }
        }

        /// <summary>
        /// Fills the array using the specified element in the region defined by the start and end indexes.
        /// </summary>
        public void FillRange(T value, int start0, int end0, int start1, int end1) {
            ThrowHelper.ThrowIfNull(array, new ArgumentNullException(nameof(array)));
        
            ThrowHelper.ThrowIf(start0 < 0, new ArgumentOutOfRangeException(nameof(start0), INDEX_OUT_OF_BOUNDS));
            ThrowHelper.ThrowIf(start1 < 0, new ArgumentOutOfRangeException(nameof(start0), INDEX_OUT_OF_BOUNDS));
            ThrowHelper.ThrowIf(end0 < 0, new ArgumentOutOfRangeException(nameof(start0), INDEX_OUT_OF_BOUNDS));
            ThrowHelper.ThrowIf(end1 < 0, new ArgumentOutOfRangeException(nameof(start0), INDEX_OUT_OF_BOUNDS));

            int len0 = array.Len0;
            int len1 = array.Len1;
        
            ThrowHelper.ThrowIf(start0 >= len0, new ArgumentOutOfRangeException(nameof(start0), INDEX_OUT_OF_BOUNDS));
            ThrowHelper.ThrowIf(start1 >= len1, new ArgumentOutOfRangeException(nameof(start0), INDEX_OUT_OF_BOUNDS));
            ThrowHelper.ThrowIf(end0 >= len0, new ArgumentOutOfRangeException(nameof(start0), INDEX_OUT_OF_BOUNDS));
            ThrowHelper.ThrowIf(end1 >= len1, new ArgumentOutOfRangeException(nameof(start0), INDEX_OUT_OF_BOUNDS));

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
        public void SafeFillRange(T value, int start0, int end0, int start1, int end1) {
            ThrowHelper.ThrowIfNull(array, new ArgumentNullException(nameof(array)));
        
            start0 = start0.Clamp(0, array.Len0 - 1);
            end0   = end0.Clamp(0, array.Len0 - 1);
            start1 = start1.Clamp(0, array.Len1 - 1);
            end1   = end1.Clamp(0, array.Len1 - 1);

            for (int i0 = start0; i0 < end0; i0++) {
                for (int i1 = start1; i1 < end1; i1++) {
                    array[i0, i1] = value;
                }
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
        if (lArray.Len0 == newSize0 || lArray.Len1 == newSize1) {
            ThrowHelper.ThrowIfNull(array, new ArgumentNullException(nameof(array)));
            return;
        }
        
        // copy the actual array
        var newArray = new T[newSize0, newSize1];

        int d0Min = Math.Min(newSize0, lArray.Len0);
        int d1Min = Math.Min(newSize0, lArray.Len1);
        
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

        for (int i0 = 0; i0 < array.Len0; i0++) {
            for (int i1 = 0; i1 < array.Len1; i1++) {
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
        
        ThrowHelper.ThrowIf(start0 < 0, new ArgumentOutOfRangeException(nameof(start0), INDEX_OUT_OF_BOUNDS));
        ThrowHelper.ThrowIf(start1 < 0, new ArgumentOutOfRangeException(nameof(start0), INDEX_OUT_OF_BOUNDS));
        ThrowHelper.ThrowIf(end0 < 0, new ArgumentOutOfRangeException(nameof(start0), INDEX_OUT_OF_BOUNDS));
        ThrowHelper.ThrowIf(end1 < 0, new ArgumentOutOfRangeException(nameof(start0), INDEX_OUT_OF_BOUNDS));
        
        int len0 = array.Len0;
        int len1 = array.Len1;
        
        ThrowHelper.ThrowIf(start0 >= len0, new ArgumentOutOfRangeException(nameof(start0), INDEX_OUT_OF_BOUNDS));
        ThrowHelper.ThrowIf(start1 >= len1, new ArgumentOutOfRangeException(nameof(start0), INDEX_OUT_OF_BOUNDS));
        ThrowHelper.ThrowIf(end0 >= len0, new ArgumentOutOfRangeException(nameof(start0), INDEX_OUT_OF_BOUNDS));
        ThrowHelper.ThrowIf(end1 >= len1, new ArgumentOutOfRangeException(nameof(start0), INDEX_OUT_OF_BOUNDS));
        
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
        
        ThrowHelper.ThrowIf(start0 < 0, new ArgumentOutOfRangeException(nameof(start0), INDEX_OUT_OF_BOUNDS));
        ThrowHelper.ThrowIf(start1 < 0, new ArgumentOutOfRangeException(nameof(start0), INDEX_OUT_OF_BOUNDS));
        ThrowHelper.ThrowIf(end0 < 0, new ArgumentOutOfRangeException(nameof(start0), INDEX_OUT_OF_BOUNDS));
        ThrowHelper.ThrowIf(end1 < 0, new ArgumentOutOfRangeException(nameof(start0), INDEX_OUT_OF_BOUNDS));

        int len0 = array.Len0;
        int len1 = array.Len1;
        
        ThrowHelper.ThrowIf(start0 >= len0, new ArgumentOutOfRangeException(nameof(start0), INDEX_OUT_OF_BOUNDS));
        ThrowHelper.ThrowIf(start1 >= len1, new ArgumentOutOfRangeException(nameof(start0), INDEX_OUT_OF_BOUNDS));
        ThrowHelper.ThrowIf(end0 >= len0, new ArgumentOutOfRangeException(nameof(start0), INDEX_OUT_OF_BOUNDS));
        ThrowHelper.ThrowIf(end1 >= len1, new ArgumentOutOfRangeException(nameof(start0), INDEX_OUT_OF_BOUNDS));
        
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
}
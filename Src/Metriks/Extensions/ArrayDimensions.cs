// Metriks
// Copyright (c) KryKom & ZlomenyMesic 2026

using System.Runtime.CompilerServices;

namespace Metriks;

[UsedImplicitly]
public static class ArrayDimensions {
    
    extension<T>(T?[,] arr) {
        
        public int Len0 {
            [Pure]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => arr.GetLength(0);
        }
        
        public int Len1 {
            [Pure]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => arr.GetLength(1);
        }

        public long LongLen0 {
            [Pure]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => arr.GetLongLength(0);
        }

        public long LongLen1 {
            [Pure]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => arr.GetLongLength(1);
        }
        
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetLen0() => arr.GetLength(0);
        
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetLen1() => arr.GetLength(1);
        
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long GetLongLen0() => arr.GetLength(0);
        
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long GetLongLen1() => arr.GetLength(1);

        public Size2D Size {
            [Pure]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new(arr.GetLen0(), arr.GetLen1());
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Size2D GetSize() => new(arr.GetLen0(), arr.GetLen1());
    }
    
    extension<T>(T?[,,] arr) {
        
        public int Len0 {
            [Pure]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => arr.GetLength(0);
        }

        public int Len1 {
            [Pure]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => arr.GetLength(1);
        }

        public int Len2 {
            [Pure]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => arr.GetLength(2);
        }

        public long LongLen0 {
            [Pure]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => arr.GetLongLength(0);
        }

        public long LongLen1 {
            [Pure]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => arr.GetLongLength(1);
        }

        public long LongLen2 {
            [Pure]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => arr.GetLongLength(2);
        }
        
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetLen0() => arr.GetLength(0);
        
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetLen1() => arr.GetLength(1);
        
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetLen2() => arr.GetLength(2);
        
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long GetLongLen0() => arr.GetLength(0);
        
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long GetLongLen1() => arr.GetLength(1);
        
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long GetLongLen2() => arr.GetLength(2);
        
        public Size3D Size {
            [Pure]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new(arr.GetLen0(), arr.GetLen1(), arr.GetLen2());
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Size3D GetSize() => new(arr.GetLen0(), arr.GetLen1(), arr.GetLen2());
    }
    
    extension<T>(T?[,,,] arr) {
        
        public int Len0 {
            [Pure]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => arr.GetLength(0);
        }

        public int Len1 {
            [Pure]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => arr.GetLength(1);
        }

        public int Len2 {
            [Pure]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => arr.GetLength(2);
        }
        
        public int Len3 {
            [Pure]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => arr.GetLength(3);
        }

        public long LongLen0 {
            [Pure]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => arr.GetLongLength(0);
        }

        public long LongLen1 {
            [Pure]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => arr.GetLongLength(1);
        }

        public long LongLen2 {
            [Pure]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => arr.GetLongLength(2);
        }
        
        public long LongLen3 {
            [Pure]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => arr.GetLongLength(3);
        }
        
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetLen0() => arr.GetLength(0);
        
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetLen1() => arr.GetLength(1);
        
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetLen2() => arr.GetLength(2);
        
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetLen3() => arr.GetLength(3);
        
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long GetLongLen0() => arr.GetLength(0);
        
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long GetLongLen1() => arr.GetLength(1);
        
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long GetLongLen2() => arr.GetLength(2);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long GetLongLen3() => arr.GetLongLength(3);
        
        public Size4D Size {
            [Pure]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new(arr.GetLen0(), arr.GetLen1(), arr.GetLen2(), arr.GetLen3());
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Size4D GetSize() => new(arr.GetLen0(), arr.GetLen1(), arr.GetLen2(), arr.GetLen3());
    }

    extension(Array arr) {
        
        public int Len0 {
            [Pure]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => arr.GetLength(0);
        }

        public int Len1 {
            [Pure]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => arr.GetLength(1);
        }

        public int Len2 {
            [Pure]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => arr.GetLength(2);
        }
        
        public int Len3 {
            [Pure]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => arr.GetLength(3);
        }

        public long LongLen0 {
            [Pure]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => arr.GetLongLength(0);
        }

        public long LongLen1 {
            [Pure]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => arr.GetLongLength(1);
        }

        public long LongLen2 {
            [Pure]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => arr.GetLongLength(2);
        }
        
        public long LongLen3 {
            [Pure]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => arr.GetLongLength(3);
        }
        
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetLen0() => arr.GetLength(0);
        
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetLen1() => arr.GetLength(1);
        
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetLen2() => arr.GetLength(2);
        
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetLen3() => arr.GetLength(3);
        
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long GetLongLen0() => arr.GetLength(0);
        
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long GetLongLen1() => arr.GetLength(1);
        
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long GetLongLen2() => arr.GetLength(2);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long GetLongLen3() => arr.GetLongLength(3);
        
        public Size2D Size2D {
            [Pure]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new(arr.GetLen0(), arr.GetLen1());
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Size2D GetSize2D() => new(arr.GetLen0(), arr.GetLen1());
        
        public Size3D Size3D {
            [Pure]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new(arr.GetLen0(), arr.GetLen1(), arr.GetLen2());
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Size3D GetSize3D() => new(arr.GetLen0(), arr.GetLen1(), arr.GetLen2());
        
        public Size4D Size4D {
            [Pure]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new(arr.GetLen0(), arr.GetLen1(), arr.GetLen2(), arr.GetLen3());
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Size4D GetSize4D() => new(arr.GetLen0(), arr.GetLen1(), arr.GetLen2(), arr.GetLen3());
    }
}
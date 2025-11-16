using System.Runtime.CompilerServices;

namespace Metriks;

[UsedImplicitly]
public static class ArrayDimensions {

    #region 2D
    
    extension<T>(T[,] arr) {
        
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
            get => new(GetLen0(arr), GetLen1(arr));
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Size2D GetSize() => new(GetLen0(arr), GetLen1(arr));
    }

    #endregion
    
    
    extension<T>(T[,,] arr) {
        
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
            get => new(GetLen0(arr), GetLen1(arr), GetLen2(arr));
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Size3D GetSize() => new(GetLen0(arr), GetLen1(arr), GetLen2(arr));
    }
    
    extension<T>(T[,,,] arr) {
        
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
            get => new(GetLen0(arr), GetLen1(arr), GetLen2(arr), GetLen3(arr));
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Size4D GetSize() => new(GetLen0(arr), GetLen1(arr), GetLen2(arr), GetLen3(arr));
    }
}
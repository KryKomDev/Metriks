using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Metriks;

/// <summary>
/// Provides utility methods for manipulating and copying data in four-dimensional arrays.
/// </summary>
public static class Array4D {
    
    /// <summary>
    /// Copies a rectangular region of data from one four-dimensional array to another.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the arrays.</typeparam>
    /// <param name="source">The source four-dimensional array.</param>
    /// <param name="srcWOffset">The starting W index in the source array.</param>
    /// <param name="srcXOffset">The starting X index in the source array.</param>
    /// <param name="srcYOffset">The starting Y index in the source array.</param>
    /// <param name="srcZOffset">The starting Z index in the source array.</param>
    /// <param name="dest">The destination four-dimensional array.</param>
    /// <param name="dstWOffset">The starting W index in the destination array.</param>
    /// <param name="dstXOffset">The starting X index in the destination array.</param>
    /// <param name="dstYOffset">The starting Y index in the destination array.</param>
    /// <param name="dstZOffset">The starting Z index in the destination array.</param>
    /// <param name="wToCopy">The number of elements to copy along the W axis.</param>
    /// <param name="xToCopy">The number of elements to copy along the X axis.</param>
    /// <param name="yToCopy">The number of elements to copy along the Y axis.</param>
    /// <param name="zToCopy">The number of elements to copy along the Z axis.</param>
    public static void Copy<T>(
        T[,,,] source, int srcWOffset, int srcXOffset, int srcYOffset, int srcZOffset,
        T[,,,] dest,   int dstWOffset, int dstXOffset, int dstYOffset, int dstZOffset, 
        int wToCopy, int xToCopy, int yToCopy, int zToCopy) 
    {
        if (wToCopy <= 0 || xToCopy <= 0 || yToCopy <= 0 || zToCopy <= 0) return;

        int srcXSize = source.GetLength(1);
        int srcYSize = source.GetLength(2);
        int srcZSize = source.GetLength(3);
        int dstXSize = dest.GetLength(1);
        int dstYSize = dest.GetLength(2);
        int dstZSize = dest.GetLength(3);

        ref var srcRef = ref source[0, 0, 0, 0];
        ref var dstRef = ref dest  [0, 0, 0, 0];

        for (int w = 0; w < wToCopy; w++) {
            for (int x = 0; x < xToCopy; x++) {
                for (int y = 0; y < yToCopy; y++) {
                    ref var srcStart = ref Unsafe.Add(ref srcRef, (((srcWOffset + w) * srcXSize + (srcXOffset + x)) * srcYSize + (srcYOffset + y)) * srcZSize + srcZOffset);
                    ref var dstStart = ref Unsafe.Add(ref dstRef, (((dstWOffset + w) * dstXSize + (dstXOffset + x)) * dstYSize + (dstYOffset + y)) * dstZSize + dstZOffset);

                    var srcSpan = MemoryMarshal.CreateSpan(ref srcStart, zToCopy);
                    var dstSpan = MemoryMarshal.CreateSpan(ref dstStart, zToCopy);

                    srcSpan.CopyTo(dstSpan);
                }
            }
        }
    }

    /// <summary>
    /// Copies a rectangular region of data from one four-dimensional array to another, using structured definitions for offsets and dimensions.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the arrays.</typeparam>
    /// <param name="source">The source four-dimensional array.</param>
    /// <param name="srcOffset">The starting position in the source array, represented as a <see cref="Point4D"/>.</param>
    /// <param name="dest">The destination four-dimensional array.</param>
    /// <param name="dstOffset">The starting position in the destination array, represented as a <see cref="Point4D"/>.</param>
    /// <param name="sizeToCopy">The size of the region to copy, represented as a <see cref="Size4D"/>.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Copy<T>(
        T[,,,] source, Point4D srcOffset,
        T[,,,] dest,   Point4D dstOffset,
        Size4D sizeToCopy) 
    {
        Copy(
            source, srcOffset.W, srcOffset.X, srcOffset.Y, srcOffset.Z,
            dest,   dstOffset.W, dstOffset.X, dstOffset.Y, dstOffset.Z,
            sizeToCopy.W, sizeToCopy.X, sizeToCopy.Y, sizeToCopy.Z
        );
    }

    /// <summary>
    /// Copies a rectangular region of data from one four-dimensional array to another
    /// using an offset in the source and a destination offset.
    /// </summary>
    /// <typeparam name="T">The type of elements in the arrays.</typeparam>
    /// <param name="source">The source four-dimensional array from which data will be copied.</param>
    /// <param name="dest">The destination four-dimensional array to which data will be copied.</param>
    /// <param name="offset">The offset in the destination array that determines the starting point of the copy operation.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Copy<T>(T[,,,] source, T[,,,] dest, Point4D offset) =>
        Copy(source, Point4D.Zero, dest, offset, dest.Size);

    /// <summary>
    /// Fills a specified rectangular region of a four-dimensional array with a given value.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the array.</typeparam>
    /// <param name="array">The four-dimensional array to be filled.</param>
    /// <param name="item">The value to fill in the specified region of the array.</param>
    /// <param name="wStart">The starting W index of the region to be filled.</param>
    /// <param name="xStart">The starting X index of the region to be filled.</param>
    /// <param name="yStart">The starting Y index of the region to be filled.</param>
    /// <param name="zStart">The starting Z index of the region to be filled.</param>
    /// <param name="wCount">The number of elements to fill along the W axis.</param>
    /// <param name="xCount">The number of elements to fill along the X axis.</param>
    /// <param name="yCount">The number of elements to fill along the Y axis.</param>
    /// <param name="zCount">The number of elements to fill along the Z axis.</param>
    public static void Fill<T>(T[,,,] array, T item, int wStart, int xStart, int yStart, int zStart, int wCount, int xCount, int yCount, int zCount) {
        if (wCount <= 0 || xCount <= 0 || yCount <= 0 || zCount <= 0) return;

        int xSize = array.GetLength(1);
        int ySize = array.GetLength(2);
        int zSize = array.GetLength(3);
        var flatArray = MemoryMarshal.CreateSpan(ref array[0, 0, 0, 0], array.Length);

        for (int w = 0; w < wCount; w++) {
            for (int x = 0; x < xCount; x++) {
                for (int y = 0; y < yCount; y++) {
                    int offset = (((wStart + w) * xSize + (xStart + x)) * ySize + (yStart + y)) * zSize + zStart;

                    var rowSpan = flatArray.Slice(offset, zCount);
                    rowSpan.Fill(item);
                }
            }
        }
    }

    /// <summary>
    /// Fills a specified rectangular region of a four-dimensional array with a given value.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the array.</typeparam>
    /// <param name="array">The four-dimensional array to fill.</param>
    /// <param name="item">The value to fill the specified region with.</param>
    /// <param name="start">The offset of the rectangular region within the array.</param>
    /// <param name="size">The size of the rectangular region within the array.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Fill<T>(T[,,,] array, T item, Point4D start, Size4D size) =>
        Fill(array, item, start.W, start.X, start.Y, start.Z, size.W, size.X, size.Y, size.Z);

    /// <summary>
    /// Fills a specified rectangular region of a four-dimensional array with a given value.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the array.</typeparam>
    /// <param name="array">The four-dimensional array to fill.</param>
    /// <param name="item">The value to fill the specified region with.</param>
    /// <param name="area">The rectangular area within the array to be filled excluding the
    /// <see cref="Area4D.Higher"/> point and the corresponding row and column.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Fill<T>(T[,,,] array, T item, Area4D area) =>
        Fill(array, item, area.Lower, area.Size + Size4D.One);

    /// <summary>
    /// Fills a specified rectangular region of a four-dimensional array with a given value.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the array.</typeparam>
    /// <param name="array">The four-dimensional array to be filled.</param>
    /// <param name="itemFactory">A factory function producing the value to fill the specified region with.</param>
    /// <param name="wStart">The starting W index of the region to be filled.</param>
    /// <param name="xStart">The starting X index of the region to be filled.</param>
    /// <param name="yStart">The starting Y index of the region to be filled.</param>
    /// <param name="zStart">The starting Z index of the region to be filled.</param>
    /// <param name="wCount">The number of elements to fill along the W axis.</param>
    /// <param name="xCount">The number of elements to fill along the X axis.</param>
    /// <param name="yCount">The number of elements to fill along the Y axis.</param>
    /// <param name="zCount">The number of elements to fill along the Z axis.</param>
    public static void Fill<T>(T[,,,] array, Func<T> itemFactory, int wStart, int xStart, int yStart, int zStart, int wCount, int xCount, int yCount, int zCount) {
        if (wCount <= 0 || xCount <= 0 || yCount <= 0 || zCount <= 0) return;

        int xSize = array.GetLength(1);
        int ySize = array.GetLength(2);
        int zSize = array.GetLength(3);
        var flatArray = MemoryMarshal.CreateSpan(ref array[0, 0, 0, 0], array.Length);

        for (int w = 0; w < wCount; w++) {
            for (int x = 0; x < xCount; x++) {
                for (int y = 0; y < yCount; y++) {
                    int offset = (((wStart + w) * xSize + (xStart + x)) * ySize + (yStart + y)) * zSize + zStart;

                    var rowSpan = flatArray.Slice(offset, zCount);
                    rowSpan.Fill(itemFactory());
                }
            }
        }
    }

    /// <summary>
    /// Fills a specified rectangular region of a four-dimensional array with a given value.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the array.</typeparam>
    /// <param name="array">The four-dimensional array to fill.</param>
    /// <param name="itemFactory">A factory function producing the value to fill the specified region with.</param>
    /// <param name="start">The offset of the rectangular region within the array.</param>
    /// <param name="size">The size of the rectangular region within the array.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Fill<T>(T[,,,] array, Func<T> itemFactory, Point4D start, Size4D size) =>
        Fill(array, itemFactory, start.W, start.X, start.Y, start.Z, size.W, size.X, size.Y, size.Z);

    /// <summary>
    /// Fills a specified rectangular region of a four-dimensional array with a given value.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the array.</typeparam>
    /// <param name="array">The four-dimensional array to fill.</param>
    /// <param name="itemFactory">A factory function producing the value to fill the specified region with.</param>
    /// <param name="area">The rectangular area within the array to be filled excluding the
    /// <see cref="Area4D.Higher"/> point and the corresponding row and column.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Fill<T>(T[,,,] array, Func<T> itemFactory, Area4D area) =>
        Fill(array, itemFactory, area.Lower, area.Size + Size4D.One);

    /// <summary>
    /// Clears a rectangular region in the specified four-dimensional array by setting its elements to the default
    /// value of the type.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the array.</typeparam>
    /// <param name="array">The four-dimensional array in which the region will be cleared.</param>
    /// <param name="wOffset">The starting W index of the region to clear.</param>
    /// <param name="xOffset">The starting X index of the region to clear.</param>
    /// <param name="yOffset">The starting Y index of the region to clear.</param>
    /// <param name="zOffset">The starting Z index of the region to clear.</param>
    /// <param name="wCount">The number of elements to clear along the W axis.</param>
    /// <param name="xCount">The number of elements to clear along the X axis.</param>
    /// <param name="yCount">The number of elements to clear along the Y axis.</param>
    /// <param name="zCount">The number of elements to clear along the Z axis.</param>
    public static void Clear<T>(T[,,,] array, int wOffset, int xOffset, int yOffset, int zOffset, int wCount, int xCount, int yCount, int zCount) {
        if (wCount <= 0 || xCount <= 0 || yCount <= 0 || zCount <= 0) return;

        int xSize = array.GetLength(1);
        int ySize = array.GetLength(2);
        int zSize = array.GetLength(3);
        var flatArray = MemoryMarshal.CreateSpan(ref array[0, 0, 0, 0], array.Length);

        for (int w = 0; w < wCount; w++) {
            for (int x = 0; x < xCount; x++) {
                for (int y = 0; y < yCount; y++) {
                    int index = (((wOffset + w) * xSize + (xOffset + x)) * ySize + (yOffset + y)) * zSize + zOffset;

                    var rowSpan = flatArray.Slice(index, zCount);
                    rowSpan.Clear();
                }
            }
        }
    }

    /// <summary>
    /// Clears a rectangular region within the specified four-dimensional array by resetting its elements to their
    /// default values.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the array.</typeparam>
    /// <param name="array">The four-dimensional array to clear.</param>
    /// <param name="offset">The starting coordinates of the region to clear, represented as a <see cref="Point4D"/>.</param>
    /// <param name="size">The size of the region to clear, represented as a <see cref="Size4D"/>.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Clear<T>(T[,,,] array, Point4D offset, Size4D size) =>
        Clear(array, offset.W, offset.X, offset.Y, offset.Z, size.W, size.X, size.Y, size.Z);

    /// <summary>
    /// Clears a rectangular region of data from a four-dimensional array, setting the elements within the specified
    /// bounds to their default values.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the array.</typeparam>
    /// <param name="array">The four-dimensional array to clear.</param>
    /// <param name="area">The area to be cleared excluding the <see cref="Area4D.Higher"/> point and the corresponding
    /// row and column.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Clear<T>(T[,,,] array, Area4D area) =>
        Clear(array, area.Lower, area.Size + Size4D.One);
    
    /// <summary>
    /// Extracts a three-dimensional array slice from a four-dimensional array at a specific W coordinate.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the array.</typeparam>
    /// <param name="array">The four-dimensional source array.</param>
    /// <param name="w">The fixed W coordinate from which to slice the data.</param>
    /// <returns>A three-dimensional array containing the slice of the array at the specified W coordinate.</returns>
    public static T[,,] SliceAtW<T>(T[,,,] array, int w) {
        var len1 = array.Len1;
        var len2 = array.Len2;
        var len3 = array.Len3;
        var slice = new T[len1, len2, len3];
        var totalElements = len1 * len2 * len3;
        if (totalElements == 0) {
            if (w < 0 || w >= array.Len0) throw new IndexOutOfRangeException();
            return slice;
        }
        MemoryMarshal.CreateReadOnlySpan(ref array[w, 0, 0, 0], totalElements)
            .CopyTo(MemoryMarshal.CreateSpan(ref slice[0, 0, 0], totalElements));
        return slice;
    }

    /// <summary>
    /// Extracts a three-dimensional array slice from a four-dimensional array at a specific X coordinate.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the array.</typeparam>
    /// <param name="array">The four-dimensional source array.</param>
    /// <param name="x">The fixed X coordinate from which to slice the data.</param>
    /// <returns>A three-dimensional array containing the slice of the array at the specified X coordinate.</returns>
    public static T[,,] SliceAtX<T>(T[,,,] array, int x) {
        var len0 = array.Len0;
        var len2 = array.Len2;
        var len3 = array.Len3;
        var slice = new T[len0, len2, len3];
        var blockSize = len2 * len3;
        if (len0 == 0 || blockSize == 0) {
            if (x < 0 || x >= array.Len1) throw new IndexOutOfRangeException();
            return slice;
        }

        ref var dstStart = ref slice[0, 0, 0];
        
        for (int w = 0; w < len0; w++) {
            var srcSpan = MemoryMarshal.CreateReadOnlySpan(ref array[w, x, 0, 0], blockSize);
            var dstSpan = MemoryMarshal.CreateSpan(ref Unsafe.Add(ref dstStart, w * blockSize), blockSize);
            srcSpan.CopyTo(dstSpan);
        }

        return slice;
    }

    /// <summary>
    /// Extracts a three-dimensional array slice from a four-dimensional array at a specific Y coordinate.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the array.</typeparam>
    /// <param name="array">The four-dimensional source array.</param>
    /// <param name="y">The fixed Y coordinate from which to slice the data.</param>
    /// <returns>A three-dimensional array containing the slice of the array at the specified Y coordinate.</returns>
    public static T[,,] SliceAtY<T>(T[,,,] array, int y) {
        var len0 = array.Len0;
        var len1 = array.Len1;
        var len3 = array.Len3;
        var slice = new T[len0, len1, len3];
        if (len0 == 0 || len1 == 0 || len3 == 0) {
            if (y < 0 || y >= array.Len2) throw new IndexOutOfRangeException();
            return slice;
        }

        ref var dstStart = ref slice[0, 0, 0];
        var dstStrideW = len1 * len3;
        
        for (int w = 0; w < len0; w++) {
            ref var dstW = ref Unsafe.Add(ref dstStart, w * dstStrideW);
            
            for (int x = 0; x < len1; x++) {
                var srcSpan = MemoryMarshal.CreateReadOnlySpan(ref array[w, x, y, 0], len3);
                var dstSpan = MemoryMarshal.CreateSpan(ref Unsafe.Add(ref dstW, x * len3), len3);
                srcSpan.CopyTo(dstSpan);
            }
        }

        return slice;
    }

    /// <summary>
    /// Extracts a three-dimensional array slice from a four-dimensional array at a specific Z coordinate.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the array.</typeparam>
    /// <param name="array">The four-dimensional source array.</param>
    /// <param name="z">The fixed Z coordinate from which to slice the data.</param>
    /// <returns>A three-dimensional array containing the slice of the array at the specified Z coordinate.</returns>
    public static T[,,] SliceAtZ<T>(T[,,,] array, int z) {
        var len0 = array.Len0;
        var len1 = array.Len1;
        var len2 = array.Len2;
        var slice = new T[len0, len1, len2];
        if (len0 == 0 || len1 == 0 || len2 == 0) {
            if (z < 0 || z >= array.Len3) throw new IndexOutOfRangeException();
            return slice;
        }

        ref var srcStart = ref array[0, 0, 0, z];
        ref var dstStart = ref slice[0, 0, 0];
        
        var strideSrcY = array.Len3;
        var strideSrcX = array.Len2 * array.Len3;
        var strideSrcW = array.Len1 * array.Len2 * array.Len3;
        
        var strideDstX = len2;
        var strideDstW = len1 * len2;
        
        for (int w = 0; w < len0; w++) {
            ref var srcW = ref Unsafe.Add(ref srcStart, w * strideSrcW);
            ref var dstW = ref Unsafe.Add(ref dstStart, w * strideDstW);
            
            for (int x = 0; x < len1; x++) {
                ref var srcRow = ref Unsafe.Add(ref srcW, x * strideSrcX);
                ref var dstRow = ref Unsafe.Add(ref dstW, x * strideDstX);
                
                for (int y = 0; y < len2; y++) {
                    Unsafe.Add(ref dstRow, y) = Unsafe.Add(ref srcRow, y * strideSrcY);
                }
            }
        }
        
        return slice;
    }
    
    /// <summary>
    /// Flattens a four-dimensional array into a one-dimensional array.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the array.</typeparam>
    /// <param name="array">The four-dimensional array to be flattened.</param>
    /// <returns>A one-dimensional array containing all elements of the input array in row-major order.</returns>
    public static T[] Flatten<T>(T[,,,] array) {
        if (array.Length == 0) return Array.Empty<T>();
        var flat = new T[array.Length];
        MemoryMarshal.CreateReadOnlySpan(ref array[0, 0, 0, 0], array.Length).CopyTo(flat);
        return flat;
    }
}

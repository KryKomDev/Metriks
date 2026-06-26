using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Metriks;

/// <summary>
/// Provides utility methods for manipulating and copying data in three-dimensional arrays.
/// </summary>
public static class Array3D {
    
    /// <summary>
    /// Copies a rectangular region of data from one three-dimensional array to another.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the arrays.</typeparam>
    /// <param name="source">The source three-dimensional array.</param>
    /// <param name="srcXOffset">The starting X index in the source array.</param>
    /// <param name="srcYOffset">The starting Y index in the source array.</param>
    /// <param name="srcZOffset">The starting Z index in the source array.</param>
    /// <param name="dest">The destination three-dimensional array.</param>
    /// <param name="dstXOffset">The starting X index in the destination array.</param>
    /// <param name="dstYOffset">The starting Y index in the destination array.</param>
    /// <param name="dstZOffset">The starting Z index in the destination array.</param>
    /// <param name="xToCopy">The number of elements to copy along the X axis.</param>
    /// <param name="yToCopy">The number of elements to copy along the Y axis.</param>
    /// <param name="zToCopy">The number of elements to copy along the Z axis.</param>
    public static void Copy<T>(
        T[,,] source, int srcXOffset, int srcYOffset, int srcZOffset,
        T[,,] dest,   int dstXOffset, int dstYOffset, int dstZOffset, 
        int xToCopy, int yToCopy, int zToCopy) 
    {
        if (xToCopy <= 0 || yToCopy <= 0 || zToCopy <= 0) return;

        int srcYSize = source.GetLength(1);
        int srcZSize = source.GetLength(2);
        int dstYSize = dest.GetLength(1);
        int dstZSize = dest.GetLength(2);

        ref var srcRef = ref source[0, 0, 0];
        ref var dstRef = ref dest  [0, 0, 0];

        for (int x = 0; x < xToCopy; x++) {
            for (int y = 0; y < yToCopy; y++) {
                ref var srcStart = ref Unsafe.Add(ref srcRef, ((srcXOffset + x) * srcYSize + (srcYOffset + y)) * srcZSize + srcZOffset);
                ref var dstStart = ref Unsafe.Add(ref dstRef, ((dstXOffset + x) * dstYSize + (dstYOffset + y)) * dstZSize + dstZOffset);

                var srcSpan = MemoryMarshal.CreateSpan(ref srcStart, zToCopy);
                var dstSpan = MemoryMarshal.CreateSpan(ref dstStart, zToCopy);

                srcSpan.CopyTo(dstSpan);
            }
        }
    }

    /// <summary>
    /// Copies a rectangular region of data from one three-dimensional array to another, using structured definitions for offsets and dimensions.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the arrays.</typeparam>
    /// <param name="source">The source three-dimensional array.</param>
    /// <param name="srcOffset">The starting position in the source array, represented as a <see cref="Point3D"/>.</param>
    /// <param name="dest">The destination three-dimensional array.</param>
    /// <param name="dstOffset">The starting position in the destination array, represented as a <see cref="Point3D"/>.</param>
    /// <param name="sizeToCopy">The size of the region to copy, represented as a <see cref="Size3D"/>.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Copy<T>(
        T[,,] source, Point3D srcOffset,
        T[,,] dest,   Point3D dstOffset,
        Size3D sizeToCopy) 
    {
        Copy(
            source, srcOffset.X, srcOffset.Y, srcOffset.Z,
            dest,   dstOffset.X, dstOffset.Y, dstOffset.Z,
            sizeToCopy.X, sizeToCopy.Y, sizeToCopy.Z
        );
    }
    
    /// <summary>
    /// Copies a rectangular region of data from one three-dimensional array to another
    /// using an offset in the source and a destination offset.
    /// </summary>
    /// <typeparam name="T">The type of elements in the arrays.</typeparam>
    /// <param name="source">The source three-dimensional array from which data will be copied.</param>
    /// <param name="dest">The destination three-dimensional array to which data will be copied.</param>
    /// <param name="offset">The offset in the destination array that determines the starting point of the copy operation.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Copy<T>(T[,,] source, T[,,] dest, Point3D offset) =>
        Copy(source, Point3D.Zero, dest, offset, dest.Size);

    /// <summary>
    /// Fills a specified rectangular region of a three-dimensional array with a given value.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the array.</typeparam>
    /// <param name="array">The three-dimensional array to be filled.</param>
    /// <param name="item">The value to fill in the specified region of the array.</param>
    /// <param name="xStart">The starting X index of the region to be filled.</param>
    /// <param name="yStart">The starting Y index of the region to be filled.</param>
    /// <param name="zStart">The starting Z index of the region to be filled.</param>
    /// <param name="xCount">The number of elements to fill along the X axis.</param>
    /// <param name="yCount">The number of elements to fill along the Y axis.</param>
    /// <param name="zCount">The number of elements to fill along the Z axis.</param>
    public static void Fill<T>(T[,,] array, T item, int xStart, int yStart, int zStart, int xCount, int yCount, int zCount) {
        if (xCount <= 0 || yCount <= 0 || zCount <= 0) return;

        int ySize = array.GetLength(1);
        int zSize = array.GetLength(2);
        var flatArray = MemoryMarshal.CreateSpan(ref array[0, 0, 0], array.Length);

        for (int x = 0; x < xCount; x++) {
            for (int y = 0; y < yCount; y++) {
                int offset = ((xStart + x) * ySize + (yStart + y)) * zSize + zStart;

                var rowSpan = flatArray.Slice(offset, zCount);
                rowSpan.Fill(item);
            }
        }
    }

    /// <summary>
    /// Fills a specified rectangular region of a three-dimensional array with a given value.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the array.</typeparam>
    /// <param name="array">The three-dimensional array to fill.</param>
    /// <param name="item">The value to fill the specified region with.</param>
    /// <param name="start">The offset of the rectangular region within the array.</param>
    /// <param name="size">The size of the rectangular region within the array.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Fill<T>(T[,,] array, T item, Point3D start, Size3D size) =>
        Fill(array, item, start.X, start.Y, start.Z, size.X, size.Y, size.Z);

    /// <summary>
    /// Fills a specified rectangular region of a three-dimensional array with a given value.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the array.</typeparam>
    /// <param name="array">The three-dimensional array to fill.</param>
    /// <param name="item">The value to fill the specified region with.</param>
    /// <param name="area">The rectangular area within the array to be filled excluding the
    /// <see cref="Area3D.Higher"/> point and the corresponding row and column.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Fill<T>(T[,,] array, T item, Area3D area) =>
        Fill(array, item, area.Lower, area.Size + Size3D.One);

    /// <summary>
    /// Fills a specified rectangular region of a three-dimensional array with a given value.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the array.</typeparam>
    /// <param name="array">The three-dimensional array to be filled.</param>
    /// <param name="itemFactory">A factory function producing the value to fill the specified region with.</param>
    /// <param name="xStart">The starting X index of the region to be filled.</param>
    /// <param name="yStart">The starting Y index of the region to be filled.</param>
    /// <param name="zStart">The starting Z index of the region to be filled.</param>
    /// <param name="xCount">The number of elements to fill along the X axis.</param>
    /// <param name="yCount">The number of elements to fill along the Y axis.</param>
    /// <param name="zCount">The number of elements to fill along the Z axis.</param>
    public static void Fill<T>(T[,,] array, Func<T> itemFactory, int xStart, int yStart, int zStart, int xCount, int yCount, int zCount) {
        if (xCount <= 0 || yCount <= 0 || zCount <= 0) return;

        int ySize = array.GetLength(1);
        int zSize = array.GetLength(2);
        var flatArray = MemoryMarshal.CreateSpan(ref array[0, 0, 0], array.Length);

        for (int x = 0; x < xCount; x++) {
            for (int y = 0; y < yCount; y++) {
                int offset = ((xStart + x) * ySize + (yStart + y)) * zSize + zStart;

                var rowSpan = flatArray.Slice(offset, zCount);
                rowSpan.Fill(itemFactory());
            }
        }
    }

    /// <summary>
    /// Fills a specified rectangular region of a three-dimensional array with a given value.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the array.</typeparam>
    /// <param name="array">The three-dimensional array to fill.</param>
    /// <param name="itemFactory">A factory function producing the value to fill the specified region with.</param>
    /// <param name="start">The offset of the rectangular region within the array.</param>
    /// <param name="size">The size of the rectangular region within the array.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Fill<T>(T[,,] array, Func<T> itemFactory, Point3D start, Size3D size) =>
        Fill(array, itemFactory, start.X, start.Y, start.Z, size.X, size.Y, size.Z);

    /// <summary>
    /// Fills a specified rectangular region of a three-dimensional array with a given value.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the array.</typeparam>
    /// <param name="array">The three-dimensional array to fill.</param>
    /// <param name="itemFactory">A factory function producing the value to fill the specified region with.</param>
    /// <param name="area">The rectangular area within the array to be filled excluding the
    /// <see cref="Area3D.Higher"/> point and the corresponding row and column.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Fill<T>(T[,,] array, Func<T> itemFactory, Area3D area) =>
        Fill(array, itemFactory, area.Lower, area.Size + Size3D.One);

    /// <summary>
    /// Clears a rectangular region in the specified three-dimensional array by setting its elements to the default
    /// value of the type.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the array.</typeparam>
    /// <param name="array">The three-dimensional array in which the region will be cleared.</param>
    /// <param name="xOffset">The starting X index of the region to clear.</param>
    /// <param name="yOffset">The starting Y index of the region to clear.</param>
    /// <param name="zOffset">The starting Z index of the region to clear.</param>
    /// <param name="xCount">The number of elements to clear along the X axis.</param>
    /// <param name="yCount">The number of elements to clear along the Y axis.</param>
    /// <param name="zCount">The number of elements to clear along the Z axis.</param>
    public static void Clear<T>(T[,,] array, int xOffset, int yOffset, int zOffset, int xCount, int yCount, int zCount) {
        if (xCount <= 0 || yCount <= 0 || zCount <= 0) return;

        int ySize = array.GetLength(1);
        int zSize = array.GetLength(2);
        var flatArray = MemoryMarshal.CreateSpan(ref array[0, 0, 0], array.Length);

        for (int x = 0; x < xCount; x++) {
            for (int y = 0; y < yCount; y++) {
                int index = ((xOffset + x) * ySize + (yOffset + y)) * zSize + zOffset;

                var rowSpan = flatArray.Slice(index, zCount);
                rowSpan.Clear();
            }
        }
    }

    /// <summary>
    /// Clears a rectangular region within the specified three-dimensional array by resetting its elements to their
    /// default values.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the array.</typeparam>
    /// <param name="array">The three-dimensional array to clear.</param>
    /// <param name="offset">The starting coordinates of the region to clear, represented as a <see cref="Point3D"/>.</param>
    /// <param name="size">The size of the region to clear, represented as a <see cref="Size3D"/>.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Clear<T>(T[,,] array, Point3D offset, Size3D size) =>
        Clear(array, offset.X, offset.Y, offset.Z, size.X, size.Y, size.Z);

    /// <summary>
    /// Clears a rectangular region of data from a three-dimensional array, setting the elements within the specified
    /// bounds to their default values.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the array.</typeparam>
    /// <param name="array">The three-dimensional array to clear.</param>
    /// <param name="area">The area to be cleared excluding the <see cref="Area3D.Higher"/> point and the corresponding
    /// row and column.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Clear<T>(T[,,] array, Area3D area) =>
        Clear(array, area.Lower, area.Size + Size3D.One);
    
    /// <summary>
    /// Extracts a two-dimensional array slice from a three-dimensional array at a specific X coordinate.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the array.</typeparam>
    /// <param name="array">The three-dimensional source array.</param>
    /// <param name="x">The fixed X coordinate from which to slice the data.</param>
    /// <returns>A two-dimensional array containing the slice of the array at the specified X coordinate.</returns>
    public static T[,] SliceAtX<T>(T[,,] array, int x) {
        var len1 = array.Len1;
        var len2 = array.Len2;
        var slice = new T[len1, len2];
        var totalElements = len1 * len2;
        if (totalElements == 0) {
            if (x < 0 || x >= array.Len0) throw new IndexOutOfRangeException();
            return slice;
        }
        MemoryMarshal.CreateReadOnlySpan(ref array[x, 0, 0], totalElements)
            .CopyTo(MemoryMarshal.CreateSpan(ref slice[0, 0], totalElements));
        return slice;
    }

    /// <summary>
    /// Extracts a two-dimensional array slice from a three-dimensional array at a specific Y coordinate.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the array.</typeparam>
    /// <param name="array">The three-dimensional source array.</param>
    /// <param name="y">The fixed Y coordinate from which to slice the data.</param>
    /// <returns>A two-dimensional array containing the slice of the array at the specified Y coordinate.</returns>
    public static T[,] SliceAtY<T>(T[,,] array, int y) {
        var len0 = array.Len0;
        var len2 = array.Len2;
        var slice = new T[len0, len2];
        if (len0 == 0 || len2 == 0) {
            if (y < 0 || y >= array.Len1) throw new IndexOutOfRangeException();
            return slice;
        }
        
        for (int x = 0; x < len0; x++) {
            var srcSpan = MemoryMarshal.CreateReadOnlySpan(ref array[x, y, 0], len2);
            var dstSpan = MemoryMarshal.CreateSpan(ref slice[x, 0], len2);
            srcSpan.CopyTo(dstSpan);
        }
        
        return slice;
    }

    /// <summary>
    /// Extracts a two-dimensional array slice from a three-dimensional array at a specific Z coordinate.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the array.</typeparam>
    /// <param name="array">The three-dimensional source array.</param>
    /// <param name="z">The fixed Z coordinate from which to slice the data.</param>
    /// <returns>A two-dimensional array containing the slice of the array at the specified Z coordinate.</returns>
    public static T[,] SliceAtZ<T>(T[,,] array, int z) {
        var len0 = array.Len0;
        var len1 = array.Len1;
        var slice = new T[len0, len1];
        if (len0 == 0 || len1 == 0) {
            if (z < 0 || z >= array.Len2) throw new IndexOutOfRangeException();
            return slice;
        }

        ref var srcStart = ref array[0, 0, z];
        ref var dstStart = ref slice[0, 0];
        
        var strideSrcY = array.Len2;
        var strideSrcX = array.Len1 * array.Len2;
        var strideDstX = len1;
        
        for (int x = 0; x < len0; x++) {
            ref var srcRow = ref Unsafe.Add(ref srcStart, x * strideSrcX);
            ref var dstRow = ref Unsafe.Add(ref dstStart, x * strideDstX);
            
            for (int y = 0; y < len1; y++) {
                Unsafe.Add(ref dstRow, y) = Unsafe.Add(ref srcRow, y * strideSrcY);
            }
        }
        
        return slice;
    }
    
    /// <summary>
    /// Flattens a three-dimensional array into a one-dimensional array.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the array.</typeparam>
    /// <param name="array">The three-dimensional array to be flattened.</param>
    /// <returns>A one-dimensional array containing all elements of the input array in row-major order.</returns>
    public static T[] Flatten<T>(T[,,] array) {
        if (array.Length == 0) return Array.Empty<T>();
        var flat = new T[array.Length];
        MemoryMarshal.CreateReadOnlySpan(ref array[0, 0, 0], array.Length).CopyTo(flat);
        return flat;
    }
}

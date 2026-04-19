using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Metriks;

/// <summary>
/// Provides utility methods for manipulating and copying data in two-dimensional arrays.
/// </summary>
public static class Array2D {
    
    /// <summary>
    /// Copies a rectangular region of data from one two-dimensional array to another.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the arrays.</typeparam>
    /// <param name="source">The source two-dimensional array.</param>
    /// <param name="srcXOffset">The starting column index in the source array.</param>
    /// <param name="srcYOffset">The starting row index in the source array.</param>
    /// <param name="dest">The destination two-dimensional array.</param>
    /// <param name="dstXOffset">The starting column index in the destination array.</param>
    /// <param name="dstYOffset">The starting row index in the destination array.</param>
    /// <param name="xToCopy">The number of columns to copy.</param>
    /// <param name="yToCopy">The number of rows to copy.</param>
    public static void Copy<T>(
        T[,] source, int srcXOffset, int srcYOffset,
        T[,] dest,   int dstXOffset, int dstYOffset, 
        int xToCopy, int yToCopy) 
    {
        // Guard against empty copies to prevent IndexOutOfRangeException on [0,0]
        if (yToCopy <= 0 || xToCopy <= 0) return;

        int srcCols = source.GetLength(1);
        int dstCols = dest  .GetLength(1);

        // Get a reference directly to the first element using C# ref returns.
        // This gives us a ref T safely without needing .NET 5 APIs.
        ref var srcRef = ref source[0, 0];
        ref var dstRef = ref dest  [0, 0];

        for (int i = 0; i < yToCopy; i++) {
            
            // Advance references to the exact offset for the current row
            ref var srcStart = ref Unsafe.Add(ref srcRef, (srcYOffset + i) * srcCols + srcXOffset);
            ref var dstStart = ref Unsafe.Add(ref dstRef, (dstYOffset + i) * dstCols + dstXOffset);

            // Create spans wrapping that row's data and copy
            var srcSpan = MemoryMarshal.CreateSpan(ref srcStart, xToCopy);
            var dstSpan = MemoryMarshal.CreateSpan(ref dstStart, xToCopy);

            srcSpan.CopyTo(dstSpan);
        }
    }

    /// <summary>
    /// Copies a rectangular region of data from one two-dimensional array to another, using structured definitions for offsets and dimensions.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the arrays.</typeparam>
    /// <param name="source">The source two-dimensional array.</param>
    /// <param name="srcOffset">The starting position in the source array, represented as a <see cref="Point2D"/>.</param>
    /// <param name="dest">The destination two-dimensional array.</param>
    /// <param name="dstOffset">The starting position in the destination array, represented as a <see cref="Point2D"/>.</param>
    /// <param name="sizeToCopy">The size of the region to copy, represented as a <see cref="Size2D"/>.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Copy<T>(
        T[,] source, Point2D srcOffset,
        T[,] dest,   Point2D dstOffset,
        Size2D sizeToCopy) 
    {
        Copy(
            source, srcOffset.X, srcOffset.Y,   
            dest,   dstOffset.X, dstOffset.Y, 
            sizeToCopy.X, sizeToCopy.Y
        );
    }

    /// <summary>
    /// Copies a rectangular region of data from one two-dimensional array to another
    /// using an offset in the source and a destination offset.
    /// </summary>
    /// <typeparam name="T">The type of elements in the arrays.</typeparam>
    /// <param name="source">The source two-dimensional array from which data will be copied.</param>
    /// <param name="dest">The destination two-dimensional array to which data will be copied.</param>
    /// <param name="offset">The offset in the destination array that determines the starting point of the copy operation.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Copy<T>(T[,] source, T[,] dest, Point2D offset) =>
        Copy(source, Point2D.Zero, dest, offset, dest.Size);

    /// <summary>
    /// Fills a specified rectangular region of a two-dimensional array with a given value.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the array.</typeparam>
    /// <param name="array">The two-dimensional array to be filled.</param>
    /// <param name="item">The value to fill in the specified region of the array.</param>
    /// <param name="xStart">The starting column index of the region to be filled.</param>
    /// <param name="yStart">The starting row index of the region to be filled.</param>
    /// <param name="xCount">The number of columns to fill in the region.</param>
    /// <param name="yCount">The number of rows to fill in the region.</param>
    public static void Fill<T>(T[,] array, T item, int xStart, int yStart, int xCount, int yCount) {
        if (xCount <= 0 || yCount <= 0) return;

        int cols = array.GetLength(1);
        var flatArray = MemoryMarshal.CreateSpan(ref array[0, 0], array.Length);

        for (int i = 0; i < yCount; i++) {
            int offset = (yStart + i) * cols + xStart;

            var rowSpan = flatArray.Slice(offset, xCount);
            rowSpan.Fill(item);
        }
    }

    /// <summary>
    /// Fills a specified rectangular region of a two-dimensional array with a given value.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the array.</typeparam>
    /// <param name="array">The two-dimensional array to fill.</param>
    /// <param name="item">The value to fill the specified region with.</param>
    /// <param name="start">The offset of the rectangular region within the array.</param>
    /// <param name="size">The size of the rectangular region within the array.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Fill<T>(T[,] array, T item, Point2D start, Size2D size) =>
        Fill(array, item, start.X, start.Y, size.X, size.Y);

    /// <summary>
    /// Fills a specified rectangular region of a two-dimensional array with a given value.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the array.</typeparam>
    /// <param name="array">The two-dimensional array to fill.</param>
    /// <param name="item">The value to fill the specified region with.</param>
    /// <param name="area">The rectangular area within the array to be filled excluding the
    /// <see cref="Area2D.Higher"/> point and the corresponding row and column.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Fill<T>(T[,] array, T item, Area2D area) =>
        Fill(array, item, area.Lower, area.Size + Size2D.One);
    
    /// <summary>
    /// Fills a specified rectangular region of a two-dimensional array with a given value.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the array.</typeparam>
    /// <param name="array">The two-dimensional array to be filled.</param>
    /// <param name="itemFactory">A factory function producing the value to fill the specified region with.</param>
    /// <param name="xStart">The starting column index of the region to be filled.</param>
    /// <param name="yStart">The starting row index of the region to be filled.</param>
    /// <param name="xCount">The number of columns to fill in the region.</param>
    /// <param name="yCount">The number of rows to fill in the region.</param>
    public static void Fill<T>(T[,] array, Func<T> itemFactory, int xStart, int yStart, int xCount, int yCount) {
        if (xCount <= 0 || yCount <= 0) return;

        int cols = array.GetLength(1);
        var flatArray = MemoryMarshal.CreateSpan(ref array[0, 0], array.Length);

        for (int i = 0; i < yCount; i++) {
            int offset = (yStart + i) * cols + xStart;

            var rowSpan = flatArray.Slice(offset, xCount);
            rowSpan.Fill(itemFactory());
        }
    }

    /// <summary>
    /// Fills a specified rectangular region of a two-dimensional array with a given value.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the array.</typeparam>
    /// <param name="array">The two-dimensional array to fill.</param>
    /// <param name="itemFactory">A factory function producing the value to fill the specified region with.</param>
    /// <param name="start">The offset of the rectangular region within the array.</param>
    /// <param name="size">The size of the rectangular region within the array.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Fill<T>(T[,] array, Func<T> itemFactory, Point2D start, Size2D size) =>
        Fill(array, itemFactory, start.X, start.Y, size.X, size.Y);

    /// <summary>
    /// Fills a specified rectangular region of a two-dimensional array with a given value.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the array.</typeparam>
    /// <param name="array">The two-dimensional array to fill.</param>
    /// <param name="itemFactory">A factory function producing the value to fill the specified region with.</param>
    /// <param name="area">The rectangular area within the array to be filled excluding the
    /// <see cref="Area2D.Higher"/> point and the corresponding row and column.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Fill<T>(T[,] array, Func<T> itemFactory, Area2D area) =>
        Fill(array, itemFactory, area.Lower, area.Size + Size2D.One);

    /// <summary>
    /// Clears a rectangular region in the specified two-dimensional array by setting its elements to the default
    /// value of the type.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the array.</typeparam>
    /// <param name="array">The two-dimensional array in which the region will be cleared.</param>
    /// <param name="xOffset">The starting column index of the region to clear.</param>
    /// <param name="yOffset">The starting row index of the region to clear.</param>
    /// <param name="xCount">The number of columns to clear in the region.</param>
    /// <param name="yCount">The number of rows to clear in the region.</param>
    public static void Clear<T>(T[,] array, int xOffset, int yOffset, int xCount, int yCount) {
        if (xCount <= 0 || yCount <= 0) return;

        int cols = array.GetLength(1);
        var flatArray = MemoryMarshal.CreateSpan(ref array[0, 0], array.Length);

        for (int i = 0; i < yCount; i++) {
            int index = (yOffset + i) * cols + xOffset;

            var rowSpan = flatArray.Slice(index, xCount);
            rowSpan.Clear();
        }
    }

    /// <summary>
    /// Clears a rectangular region within the specified two-dimensional array by resetting its elements to their
    /// default values.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the array.</typeparam>
    /// <param name="array">The two-dimensional array to clear.</param>
    /// <param name="offset">The starting coordinates of the region to clear, represented as a <see cref="Point2D"/>.</param>
    /// <param name="size">The size of the region to clear, represented as a <see cref="Size2D"/>.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Clear<T>(T[,] array, Point2D offset, Size2D size) =>
        Clear(array, offset.X, offset.Y, size.X, size.Y);

    /// <summary>
    /// Clears a rectangular region of data from a two-dimensional array, setting the elements within the specified
    /// bounds to their default values.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the array.</typeparam>
    /// <param name="array">The two-dimensional array to clear.</param>
    /// <param name="area">The area to be cleared excluding the <see cref="Area2D.Higher"/> point and the corresponding
    /// row and column.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Clear<T>(T[,] array, Area2D area) =>
        Clear(array, area.Lower, area.Size + Size2D.One);
}
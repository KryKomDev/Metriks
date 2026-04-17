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
    /// <param name="srcColOffset">The starting column index in the source array.</param>
    /// <param name="srcRowOffset">The starting row index in the source array.</param>
    /// <param name="dest">The destination two-dimensional array.</param>
    /// <param name="dstColOffset">The starting column index in the destination array.</param>
    /// <param name="dstRowOffset">The starting row index in the destination array.</param>
    /// <param name="colsToCopy">The number of columns to copy.</param>
    /// <param name="rowsToCopy">The number of rows to copy.</param>
    public static void Copy<T>(
        T[,] source, int srcColOffset, int srcRowOffset,
        T[,] dest,   int dstColOffset, int dstRowOffset, 
        int colsToCopy, int rowsToCopy) 
    {
        // Guard against empty copies to prevent IndexOutOfRangeException on [0,0]
        if (rowsToCopy <= 0 || colsToCopy <= 0) return;

        int srcCols = source.GetLength(1);
        int dstCols = dest  .GetLength(1);

        // Get a reference directly to the first element using C# ref returns.
        // This gives us a ref T safely without needing .NET 5 APIs.
        ref var srcRef = ref source[0, 0];
        ref var dstRef = ref dest  [0, 0];

        for (int i = 0; i < rowsToCopy; i++) {
            
            // Advance references to the exact offset for the current row
            ref var srcStart = ref Unsafe.Add(ref srcRef, (srcRowOffset + i) * srcCols + srcColOffset);
            ref var dstStart = ref Unsafe.Add(ref dstRef, (dstRowOffset + i) * dstCols + dstColOffset);

            // Create spans wrapping that row's data and copy
            var srcSpan = MemoryMarshal.CreateSpan(ref srcStart, colsToCopy);
            var dstSpan = MemoryMarshal.CreateSpan(ref dstStart, colsToCopy);

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
}
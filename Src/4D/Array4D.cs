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
}

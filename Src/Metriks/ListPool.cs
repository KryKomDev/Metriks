// Metriks
// Copyright (c) KryKom & ZlomenyMesic 2026

using System.Collections.Concurrent;

namespace Metriks;

/// <summary>
///     Provides a pool for renting and returning <see cref="List2D{T}"/> instances to reduce allocations.
/// </summary>
/// <typeparam name="T">The type of elements in the 2D list.</typeparam>
public static class ListPool2D<T> {
    private static readonly ConcurrentQueue<List2D<T>> POOL = new();

    /// <summary>
    ///     Rents a <see cref="List2D{T}"/> from the pool with at least the specified capacity.
    /// </summary>
    /// <param name="xCapacity">The requested capacity along the X-axis.</param>
    /// <param name="yCapacity">The requested capacity along the Y-axis.</param>
    /// <returns>A rented and reset <see cref="List2D{T}"/> instance.</returns>
    public static List2D<T> Rent(int xCapacity, int yCapacity) {
        ArgumentOutOfRangeException.ThrowIfNegative(xCapacity);
        ArgumentOutOfRangeException.ThrowIfNegative(yCapacity);

        if (!POOL.TryDequeue(out var list))
            return new List2D<T>(xCapacity, yCapacity);

        list.Reset(xCapacity, yCapacity);
        return list;

    }

    /// <summary>
    ///     Returns a <see cref="List2D{T}"/> to the pool.
    /// </summary>
    /// <param name="list">The list to return.</param>
    public static void Return(List2D<T> list) {
        if (list == null) return;

        list.Reset(0, 0);
        POOL.Enqueue(list);
    }
}

/// <summary>
///     Provides a pool for renting and returning <see cref="List3D{T}"/> instances to reduce allocations.
/// </summary>
/// <typeparam name="T">The type of elements in the 3D list.</typeparam>
public static class ListPool3D<T> {
    private static readonly ConcurrentQueue<List3D<T>> POOL = new();

    /// <summary>
    ///     Rents a <see cref="List3D{T}"/> from the pool with at least the specified capacity.
    /// </summary>
    /// <param name="xCapacity">The requested capacity along the X-axis.</param>
    /// <param name="yCapacity">The requested capacity along the Y-axis.</param>
    /// <param name="zCapacity">The requested capacity along the Z-axis.</param>
    /// <returns>A rented and reset <see cref="List3D{T}"/> instance.</returns>
    public static List3D<T> Rent(int xCapacity, int yCapacity, int zCapacity) {
        ArgumentOutOfRangeException.ThrowIfNegative(xCapacity);
        ArgumentOutOfRangeException.ThrowIfNegative(yCapacity);
        ArgumentOutOfRangeException.ThrowIfNegative(zCapacity);

        if (!POOL.TryDequeue(out var list))
            return new List3D<T>(xCapacity, yCapacity, zCapacity);

        list.Reset(xCapacity, yCapacity, zCapacity);
        return list;

    }

    /// <summary>
    ///     Returns a <see cref="List3D{T}"/> to the pool.
    /// </summary>
    /// <param name="list">The list to return.</param>
    public static void Return(List3D<T> list) {
        if (list == null) return;

        list.Reset(0, 0, 0);
        POOL.Enqueue(list);
    }
}

/// <summary>
///     Provides a pool for renting and returning <see cref="List4D{T}"/> instances to reduce allocations.
/// </summary>
/// <typeparam name="T">The type of elements in the 4D list.</typeparam>
public static class ListPool4D<T> {
    private static readonly ConcurrentQueue<List4D<T>> POOL = new();

    /// <summary>
    ///     Rents a <see cref="List4D{T}"/> from the pool with at least the specified capacity.
    /// </summary>
    /// <param name="wCapacity">The requested capacity along the W-axis.</param>
    /// <param name="xCapacity">The requested capacity along the X-axis.</param>
    /// <param name="yCapacity">The requested capacity along the Y-axis.</param>
    /// <param name="zCapacity">The requested capacity along the Z-axis.</param>
    /// <returns>A rented and reset <see cref="List4D{T}"/> instance.</returns>
    public static List4D<T> Rent(int wCapacity, int xCapacity, int yCapacity, int zCapacity) {
        ArgumentOutOfRangeException.ThrowIfNegative(wCapacity);
        ArgumentOutOfRangeException.ThrowIfNegative(xCapacity);
        ArgumentOutOfRangeException.ThrowIfNegative(yCapacity);
        ArgumentOutOfRangeException.ThrowIfNegative(zCapacity);

        if (!POOL.TryDequeue(out var list))
            return new List4D<T>(wCapacity, xCapacity, yCapacity, zCapacity);

        list.Reset(wCapacity, xCapacity, yCapacity, zCapacity);
        return list;

    }

    /// <summary>
    ///     Returns a <see cref="List4D{T}"/> to the pool.
    /// </summary>
    /// <param name="list">The list to return.</param>
    public static void Return(List4D<T> list) {
        if (list == null) return;

        list.Reset(0, 0, 0, 0);
        POOL.Enqueue(list);
    }
}

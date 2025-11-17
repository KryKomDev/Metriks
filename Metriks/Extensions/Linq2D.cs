// Metriks
// Copyright (c) KryKom & ZlomenyMesic 2025

namespace Metriks;

public static class Linq2D {
    
    /// <summary>
    /// Projects each sub-collection of elements in a two-dimensional enumerable into a new form.
    /// </summary>
    /// <typeparam name="T">The type of elements in the sub-collections.</typeparam>
    /// <typeparam name="TResult">The type of the value returned by the selector function.</typeparam>
    /// <param name="list">The two-dimensional enumerable to invoke the projection on.</param>
    /// <param name="selector">A transform function to apply to each sub-collection.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> whose elements are the result of invoking the projection
    /// function on each sub-collection.</returns>
    public static IEnumerable<TResult> Select<T, TResult>(
        this IEnumerable2D<T> list,
        Func<IEnumerable<T>, TResult> selector) 
    {
        foreach (var l in list) yield return selector(l);
    }
}
using System.Runtime.CompilerServices;

namespace Metriks;

public static class Numeric {
    
    /// <summary>
    /// Clamps the specified <see cref="IComparable{T}"/> between the maximum and minimum values.
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Clamp<T>(this T value, T min, T max) where T : IComparable<T> => 
        value.CompareTo(min) < 0 ? min : 
        value.CompareTo(max) > 0 ? max : value;


}
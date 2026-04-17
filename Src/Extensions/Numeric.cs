using System.Runtime.CompilerServices;

namespace Metriks;

public static class Numeric {
    
    /// <summary>
    /// Clamps the specified <see cref="IComparable{T}"/> between the maximum and minimum values.
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Clamp<T>(this T value, T min, T max) where T : IComparable<T> 
        => value.CompareTo(min) < 0 
            ? min
            : value.CompareTo(max) > 0 
                ? max 
                : value;

    extension(int value) {
        
        /// <summary>
        /// Determines whether the current numeric value is within the specified range, inclusive of the range's boundaries.
        /// If the range's boundaries are from the end of the range, the range is considered to be open on that side
        /// (value is excluded). <br/>
        /// <b>WARNING:</b> This works only for ranges with non-negative start and end values
        /// (from-end indices are detected by checking if the index value is negative).
        /// </summary>
        /// <param name="range">The range to check the numeric value against.</param>
        /// <returns>True if the numeric value lies within the range; otherwise, false.</returns>
        /// <example>
        /// <code>
        /// var t1 = 5.Belongs(1..10);    // true
        /// var t2 = 11.Belongs(1..10);   // false
        /// var t3 = 10.Belongs(1..10);   // true
        /// var t4 = 10.Belongs(10..20);  // true
        /// var t5 = 10.Belongs(10..10);  // true
        /// var t6 = 10.Belongs(^10..20); // false
        /// var t7 = 10.Belongs(1..^10);  // false
        /// </code>
        /// </example>
        public bool Belongs(Range range) {
            return value.Belongs(
                range.Start.Value,
                range.End  .Value, 
                range.Start.IsFromEnd,
                range.End  .IsFromEnd
            );
        }

        /// <summary>
        /// Determines whether the current numeric value is within the specified range,
        /// inclusive of both the start and end boundaries.
        /// </summary>
        /// <param name="start">The starting boundary of the range.</param>
        /// <param name="end">The ending boundary of the range.</param>
        /// <returns>True if the numeric value lies within the range; otherwise, false.</returns>
        public bool Belongs(int start, int end) {
            return start <= value && value <= end;
        }

        /// <summary>
        /// Determines whether the current numeric value is within the specified range,
        /// inclusive or exclusive of the range's boundaries based on the specified parameters.
        /// </summary>
        /// <param name="start">The inclusive or exclusive start value of the range.</param>
        /// <param name="end">The inclusive or exclusive end value of the range.</param>
        /// <param name="leftOpen">Indicates whether the start of the range is open (exclusive).
        /// If true, the start value is excluded.</param>
        /// <param name="rightOpen">Indicates whether the end of the range is open (exclusive).
        /// If true, the end value is excluded.</param>
        /// <returns>True if the numeric value lies within the range according to the specified
        /// inclusivity/exclusivity; otherwise, false.</returns>
        public bool Belongs(int start, int end, bool leftOpen, bool rightOpen) {
            return start + (leftOpen ? 1 : 0) <= value && value <= end - (rightOpen ? 1 : 0);
        }
    }

    extension(int) {
        public static bool operator ==(int left, Range right) => left.Belongs(right);
        public static bool operator !=(int left, Range right) => !left.Belongs(right);
    }
}
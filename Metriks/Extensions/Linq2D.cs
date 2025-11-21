// Metriks
// Copyright (c) KryKom & ZlomenyMesic 2025

namespace Metriks;

public static class Linq2D {
    
    /// <param name="list">The two-dimensional enumerable to invoke the projection on.</param>
    /// <typeparam name="T">The type of elements in the sub-collections.</typeparam>
    extension<T>(IEnumerable2D<T> list) {
        
        /// <summary>
        /// Projects each sub-collection of elements in a two-dimensional enumerable into a new form.
        /// </summary>
        /// <typeparam name="TResult">The type of the value returned by the selector function.</typeparam>
        /// <param name="selector">A transform function to apply to each sub-collection.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> whose elements are the result of invoking the projection
        /// function on each sub-collection.</returns>
        public IEnumerable<TResult> Select<TResult>(Func<IEnumerable<T>, TResult> selector) {
            foreach (var l in list) yield return selector(l);
        }

        /// <summary>
        /// Retrieves the sub-collection of elements at the specified horizontal index in a two-dimensional enumerable.
        /// </summary>
        /// <typeparam name="T">The type of elements in the sub-collections.</typeparam>
        /// <param name="x">The zero-based horizontal index of the sub-collection to retrieve.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> representing the sub-collection at the specified index.</returns>
        /// <exception cref="IndexOutOfRangeException">Thrown if the specified index is outside the bounds of the enumerable.</exception>
        public IEnumerable<T> GetAtX(int x) {
            int skipped = 0;

            foreach (var e in list) {
                if (skipped == x)
                    return e; // TODO - is this correct?
                
                skipped++;
            }
            
            throw new IndexOutOfRangeException();
        }

        /// <summary>
        /// Retrieves the element at the specified index within each sub-collection of a two-dimensional enumerable.
        /// </summary>
        /// <typeparam name="T">The type of elements in the sub-collections.</typeparam>
        /// <param name="y">The zero-based index of the element to retrieve from each sub-collection.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> containing the elements at the specified index from each sub-collection.</returns>
        /// <exception cref="IndexOutOfRangeException">
        /// Thrown when the specified index is out of range for any sub-collection.
        /// </exception>
        public IEnumerable<T> GetAtY(int y) {
            foreach (var e in list) {
                int  skipped = 0;
                T    val     = default!;
                bool success = false;
                
                foreach (var t in e) {
                    if (skipped == y) {
                        success = true;
                        val = t;
                        break; // TODO - is this correct?
                    }
                    
                    skipped++;
                }

                if (success)
                    yield return val;
                else
                    throw new IndexOutOfRangeException();
            }
        }
    }

    extension<T>(List2D<T> list) {
        
        /// <summary>
        /// Determines whether all elements in the specified column satisfy the given predicate.
        /// </summary>
        /// <param name="x">The zero-based index of the column to check.</param>
        /// <param name="predicate">The predicate to evaluate for each element in the column.</param>
        /// <returns>
        /// True if all elements in the column satisfy the predicate; otherwise, false.
        /// </returns>
        public bool AllAtX(int x, Predicate<T> predicate) {
            for (int y = 0; y < list.YSize; y++) {
                if (!predicate(list.Items[x][y])) return false;
            }
        
            return true;
        }

        /// <summary>
        /// Determines whether all elements in the specified row of the 2D list satisfy a given condition.
        /// </summary>
        /// <param name="y">The zero-based index of the row to evaluate.</param>
        /// <param name="predicate">The condition to evaluate against each element in the row.</param>
        /// <returns>
        /// True if all elements in the specified row satisfy the condition; otherwise, false.
        /// </returns>
        /// <exception cref="IndexOutOfRangeException">
        /// Thrown when the specified row index is less than 0 or greater than or equal to the current YSize.
        /// </exception>
        public bool AllAtY(int y, Predicate<T> predicate) {
            for (int x = 0; x < list.XSize; x++) {
                if (!predicate(list.Items[x][y])) return false;
            }
        
            return true;
        }
    }

    extension<T>(Plane<T> plane) {
        
        /// <summary>
        /// Determines whether all elements in the specified column, adjusted for the origin offset, satisfy the given
        /// predicate.
        /// </summary>
        /// <param name="x">The x-coordinate (adjusted for the origin offset) of the column to check.</param>
        /// <param name="predicate">The predicate to evaluate for each element in the column.</param>
        /// <returns>
        /// True if all elements in the specified column satisfy the predicate; otherwise, false.
        /// </returns>
        public bool AllAtX(int x, Predicate<T> predicate) {
            for (int y = 0; y < plane.YSize; y++) {
                if (!predicate(plane.Items[x + plane.XOriginOffset][y])) return false;
            }
        
            return true;
        }

        /// <summary>
        /// Determines whether all elements in the specified row of the 2D list, with consideration to the origin offset,
        /// satisfy a given condition.
        /// </summary>
        /// <param name="y">The y-coordinate of the row to evaluate.</param>
        /// <param name="predicate">The condition to evaluate against each element in the row.</param>
        /// <returns>
        /// True if all elements in the specified row satisfy the condition; otherwise, false.
        /// </returns>
        /// <exception cref="IndexOutOfRangeException">
        /// Thrown when the specified row index is less than 0 or greater than or equal to the current YSize,
        /// considering the origin offset.
        /// </exception>
        public bool AllAtY(int y, Predicate<T> predicate) {
            for (int x = 0; x < plane.XSize; x++) {
                if (!predicate(plane.Items[x][y + plane.YOriginOffset])) return false;
            }
        
            return true;
        }

    }
    
}
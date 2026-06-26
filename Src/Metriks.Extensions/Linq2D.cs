namespace Metriks;

public static class Linq2DExtensions {
    extension<T>(IEnumerable2D<T> list) {
        
        /// <summary>
        /// Flattens the two-dimensional enumerable into a one-dimensional sequence.
        /// </summary>
        public IEnumerable<T> Flatten() {
            foreach (var row in list) {
                foreach (var item in row) {
                    yield return item;
                }
            }
        }

        /// <summary>
        /// Determines whether the two-dimensional enumerable contains any elements.
        /// </summary>
        public bool Any() => Flatten(list).Any();

        /// <summary>
        /// Determines whether any element of the two-dimensional enumerable satisfies a condition.
        /// </summary>
        public bool Any(Func<T, bool> predicate) => Flatten(list).Any(predicate);

        /// <summary>
        /// Determines whether all elements of the two-dimensional enumerable satisfy a condition.
        /// </summary>
        public bool All(Func<T, bool> predicate) => Flatten(list).All(predicate);

        /// <summary>
        /// Returns the number of elements in the two-dimensional enumerable that satisfy a condition.
        /// </summary>
        public int Count(Func<T, bool> predicate) => Flatten(list).Count(predicate);

        /// <summary>
        /// Returns the first element of the two-dimensional enumerable.
        /// </summary>
        public T First() => Flatten(list).First();

        /// <summary>
        /// Returns the first element of the two-dimensional enumerable that satisfies a condition.
        /// </summary>
        public T First(Func<T, bool> predicate) => Flatten(list).First(predicate);

        /// <summary>
        /// Returns the first element of the two-dimensional enumerable, or a default value if the sequence contains no elements.
        /// </summary>
        public T? FirstOrDefault() => Flatten(list).FirstOrDefault();

        /// <summary>
        /// Returns the first element of the two-dimensional enumerable that satisfies a condition or a default value if no such element is found.
        /// </summary>
        public T? FirstOrDefault(Func<T, bool> predicate) => Flatten(list).FirstOrDefault(predicate);

        /// <summary>
        /// Returns the last element of the two-dimensional enumerable.
        /// </summary>
        public T Last() => Flatten(list).Last();

        /// <summary>
        /// Returns the last element of the two-dimensional enumerable that satisfies a condition.
        /// </summary>
        public T Last(Func<T, bool> predicate) => Flatten(list).Last(predicate);

        /// <summary>
        /// Returns the last element of the two-dimensional enumerable, or a default value if the sequence contains no elements.
        /// </summary>
        public T? LastOrDefault() => Flatten(list).LastOrDefault();

        /// <summary>
        /// Returns the last element of the two-dimensional enumerable that satisfies a condition or a default value if no such element is found.
        /// </summary>
        public T? LastOrDefault(Func<T, bool> predicate) => Flatten(list).LastOrDefault(predicate);

        /// <summary>
        /// Returns the only element of the two-dimensional enumerable, and throws an exception if there is not exactly one element in the sequence.
        /// </summary>
        public T Single() => Flatten(list).Single();

        /// <summary>
        /// Returns the only element of the two-dimensional enumerable that satisfies a specified condition, and throws an exception if more than one such element exists.
        /// </summary>
        public T Single(Func<T, bool> predicate) => Flatten(list).Single(predicate);

        /// <summary>
        /// Returns the only element of the two-dimensional enumerable, or a default value if the sequence is empty; this method throws an exception if there is more than one element in the sequence.
        /// </summary>
        public T? SingleOrDefault() => Flatten(list).SingleOrDefault();

        /// <summary>
        /// Returns the only element of the two-dimensional enumerable that satisfies a specified condition or a default value if no such element exists; this method throws an exception if more than one element satisfies the condition.
        /// </summary>
        public T? SingleOrDefault(Func<T, bool> predicate) => Flatten(list).SingleOrDefault(predicate);
    }

    extension<T>(List2D<T> list) {
        /// <summary>
        /// Projects each element of a two-dimensional list into a new form, returning a new two-dimensional list.
        /// </summary>
        public List2D<TResult> Select<TResult>(Func<T, TResult> selector) {
            if (selector is null)
                throw new ArgumentNullException(nameof(selector));

            var result = new List2D<TResult>(list.XSize, list.YSize);
            result.Resize(list.XSize, list.YSize);

            for (int x = 0; x < list.XSize; x++) {
                for (int y = 0; y < list.YSize; y++) {
                    result[x, y] = selector(list[x, y]);
                }
            }

            return result;
        }

        /// <summary>
        /// Determines whether any element in the specified column satisfies the given predicate.
        /// </summary>
        public bool AnyAtX(int x, Predicate<T> predicate) {
            for (int y = 0; y < list.YSize; y++) {
                if (predicate(list[x, y]))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Determines whether any element in the specified row satisfies the given predicate.
        /// </summary>
        public bool AnyAtY(int y, Predicate<T> predicate) {
            for (int x = 0; x < list.XSize; x++) {
                if (predicate(list[x, y]))
                    return true;
            }

            return false;
        }
    }

    extension<T>(Space2D<T> space) {
        /// <summary>
        /// Projects each element of a two-dimensional spatial grid into a new form, returning a new Space2D.
        /// </summary>
        public Space2D<TResult> Select<TResult>(Func<T, TResult> selector) {
            if (selector is null)
                throw new ArgumentNullException(nameof(selector));

            var result = new Space2D<TResult>(space.XSize, space.YSize);
            result.Resize(space.XSize, space.YSize);
            result.MoveOrigin(space.XOriginOffset, space.YOriginOffset);

            for (int x = space.XStart; x <= space.XEnd; x++) {
                for (int y = space.YStart; y <= space.YEnd; y++) {
                    result[x, y] = selector(space[x, y]);
                }
            }

            return result;
        }

        /// <summary>
        /// Determines whether any element in the specified column (adjusted for origin offset) satisfies the given predicate.
        /// </summary>
        public bool AnyAtX(int x, Predicate<T> predicate) {
            for (int y = space.YStart; y <= space.YEnd; y++) {
                if (predicate(space[x, y]))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Determines whether any element in the specified row (adjusted for origin offset) satisfies the given predicate.
        /// </summary>
        public bool AnyAtY(int y, Predicate<T> predicate) {
            for (int x = space.XStart; x <= space.XEnd; x++) {
                if (predicate(space[x, y]))
                    return true;
            }

            return false;
        }
    }
}
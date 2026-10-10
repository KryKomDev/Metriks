namespace Metriks;

public static class Linq3DExtensions {

    extension<T>(IEnumerable3D<T> list) {

        /// <summary>
        ///     Flattens the three-dimensional enumerable into a one-dimensional sequence.
        /// </summary>
        public IEnumerable<T> Flatten() {
            foreach (var plane in list)
            foreach (var row in plane)
            foreach (var item in row)
                yield return item;
        }

        /// <summary>
        ///     Determines whether the three-dimensional enumerable contains any elements.
        /// </summary>
        public bool Any() => list.Flatten().Any();

        /// <summary>
        ///     Determines whether any element of the three-dimensional enumerable satisfies a condition.
        /// </summary>
        public bool Any(Func<T, bool> predicate) => list.Flatten().Any(predicate);

        /// <summary>
        ///     Determines whether all elements of the three-dimensional enumerable satisfy a condition.
        /// </summary>
        public bool All(Func<T, bool> predicate) => list.Flatten().All(predicate);

        /// <summary>
        ///     Returns the number of elements in the three-dimensional enumerable that satisfy a condition.
        /// </summary>
        public int Count(Func<T, bool> predicate) => list.Flatten().Count(predicate);

        /// <summary>
        ///     Returns the first element of the three-dimensional enumerable.
        /// </summary>
        public T First() => list.Flatten().First();

        /// <summary>
        ///     Returns the first element of the three-dimensional enumerable that satisfies a condition.
        /// </summary>
        public T First(Func<T, bool> predicate) => list.Flatten().First(predicate);

        /// <summary>
        ///     Returns the first element of the three-dimensional enumerable, or a default value if the sequence contains no
        ///     elements.
        /// </summary>
        public T? FirstOrDefault() => list.Flatten().FirstOrDefault();

        /// <summary>
        ///     Returns the first element of the three-dimensional enumerable that satisfies a condition or a default value if no
        ///     such element is found.
        /// </summary>
        public T? FirstOrDefault(Func<T, bool> predicate) => list.Flatten().FirstOrDefault(predicate);

        /// <summary>
        ///     Returns the last element of the three-dimensional enumerable.
        /// </summary>
        public T Last() => list.Flatten().Last();

        /// <summary>
        ///     Returns the last element of the three-dimensional enumerable that satisfies a condition.
        /// </summary>
        public T Last(Func<T, bool> predicate) => list.Flatten().Last(predicate);

        /// <summary>
        ///     Returns the last element of the three-dimensional enumerable, or a default value if the sequence contains no
        ///     elements.
        /// </summary>
        public T? LastOrDefault() => list.Flatten().LastOrDefault();

        /// <summary>
        ///     Returns the last element of the three-dimensional enumerable that satisfies a condition or a default value if no
        ///     such element is found.
        /// </summary>
        public T? LastOrDefault(Func<T, bool> predicate) => list.Flatten().LastOrDefault(predicate);

        /// <summary>
        ///     Returns the only element of the three-dimensional enumerable, and throws an exception if there is not exactly one
        ///     element in the sequence.
        /// </summary>
        public T Single() => list.Flatten().Single();

        /// <summary>
        ///     Returns the only element of the three-dimensional enumerable that satisfies a specified condition, and throws an
        ///     exception if more than one such element exists.
        /// </summary>
        public T Single(Func<T, bool> predicate) => list.Flatten().Single(predicate);

        /// <summary>
        ///     Returns the only element of the three-dimensional enumerable, or a default value if the sequence is empty; this
        ///     method throws an exception if there is more than one element in the sequence.
        /// </summary>
        public T? SingleOrDefault() => list.Flatten().SingleOrDefault();

        /// <summary>
        ///     Returns the only element of the three-dimensional enumerable that satisfies a specified condition or a default
        ///     value if no such element exists; this method throws an exception if more than one element satisfies the condition.
        /// </summary>
        public T? SingleOrDefault(Func<T, bool> predicate) => list.Flatten().SingleOrDefault(predicate);
    }

    extension<T>(List3D<T> list) {

        /// <summary>
        ///     Projects each element of a three-dimensional list into a new form, returning a new three-dimensional list.
        /// </summary>
        public List3D<TResult> Select<TResult>(Func<T, TResult> selector) {
            if (selector is null)
                throw new ArgumentNullException(nameof(selector));

            var result = new List3D<TResult>(list.XSize, list.YSize, list.ZSize);
            result.Resize(list.XSize, list.YSize, list.ZSize);

            for (var x = 0; x < list.XSize; x++)
            for (var y = 0; y < list.YSize; y++)
            for (var z = 0; z < list.ZSize; z++)
                result[x, y, z] = selector(list[x, y, z]);

            return result;
        }

        /// <summary>
        ///     Determines whether all elements at the specified X index satisfy the given predicate.
        /// </summary>
        public bool AllAtX(int x, Predicate<T> predicate) {
            for (var y = 0; y < list.YSize; y++)
            for (var z = 0; z < list.ZSize; z++)
                if (!predicate(list[x, y, z]))
                    return false;

            return true;
        }

        /// <summary>
        ///     Determines whether all elements at the specified Y index satisfy the given predicate.
        /// </summary>
        public bool AllAtY(int y, Predicate<T> predicate) {
            for (var x = 0; x < list.XSize; x++)
            for (var z = 0; z < list.ZSize; z++)
                if (!predicate(list[x, y, z]))
                    return false;

            return true;
        }

        /// <summary>
        ///     Determines whether all elements at the specified Z index satisfy the given predicate.
        /// </summary>
        public bool AllAtZ(int z, Predicate<T> predicate) {
            for (var x = 0; x < list.XSize; x++)
            for (var y = 0; y < list.YSize; y++)
                if (!predicate(list[x, y, z]))
                    return false;

            return true;
        }

        /// <summary>
        ///     Determines whether any element at the specified X index satisfies the given predicate.
        /// </summary>
        public bool AnyAtX(int x, Predicate<T> predicate) {
            for (var y = 0; y < list.YSize; y++)
            for (var z = 0; z < list.ZSize; z++)
                if (predicate(list[x, y, z]))
                    return true;

            return false;
        }

        /// <summary>
        ///     Determines whether any element at the specified Y index satisfies the given predicate.
        /// </summary>
        public bool AnyAtY(int y, Predicate<T> predicate) {
            for (var x = 0; x < list.XSize; x++)
            for (var z = 0; z < list.ZSize; z++)
                if (predicate(list[x, y, z]))
                    return true;

            return false;
        }

        /// <summary>
        ///     Determines whether any element at the specified Z index satisfies the given predicate.
        /// </summary>
        public bool AnyAtZ(int z, Predicate<T> predicate) {
            for (var x = 0; x < list.XSize; x++)
            for (var y = 0; y < list.YSize; y++)
                if (predicate(list[x, y, z]))
                    return true;

            return false;
        }
    }

    extension<T>(Space3D<T> space) {

        /// <summary>
        ///     Projects each element of a three-dimensional spatial grid into a new form, returning a new Space3D.
        /// </summary>
        public Space3D<TResult> Select<TResult>(Func<T, TResult> selector) {
            if (selector is null)
                throw new ArgumentNullException(nameof(selector));

            var result = new Space3D<TResult>(space.XSize, space.YSize, space.ZSize);
            result.Resize(space.XSize, space.YSize, space.ZSize);
            result.MoveOrigin(space.XOriginOffset, space.YOriginOffset, space.ZOriginOffset);

            for (var x = space.XStart; x <= space.XEnd; x++)
            for (var y = space.YStart; y <= space.YEnd; y++)
            for (var z = space.ZStart; z <= space.ZEnd; z++)
                result[x, y, z] = selector(space[x, y, z]);

            return result;
        }

        /// <summary>
        ///     Determines whether all elements at the specified X index (adjusted for offset) satisfy the given predicate.
        /// </summary>
        public bool AllAtX(int x, Predicate<T> predicate) {
            for (var y = space.YStart; y <= space.YEnd; y++)
            for (var z = space.ZStart; z <= space.ZEnd; z++)
                if (!predicate(space[x, y, z]))
                    return false;

            return true;
        }

        /// <summary>
        ///     Determines whether all elements at the specified Y index (adjusted for offset) satisfy the given predicate.
        /// </summary>
        public bool AllAtY(int y, Predicate<T> predicate) {
            for (var x = space.XStart; x <= space.XEnd; x++)
            for (var z = space.ZStart; z <= space.ZEnd; z++)
                if (!predicate(space[x, y, z]))
                    return false;

            return true;
        }

        /// <summary>
        ///     Determines whether all elements at the specified Z index (adjusted for offset) satisfy the given predicate.
        /// </summary>
        public bool AllAtZ(int z, Predicate<T> predicate) {
            for (var x = space.XStart; x <= space.XEnd; x++)
            for (var y = space.YStart; y <= space.YEnd; y++)
                if (!predicate(space[x, y, z]))
                    return false;

            return true;
        }

        /// <summary>
        ///     Determines whether any element at the specified X index (adjusted for offset) satisfies the given predicate.
        /// </summary>
        public bool AnyAtX(int x, Predicate<T> predicate) {
            for (var y = space.YStart; y <= space.YEnd; y++)
            for (var z = space.ZStart; z <= space.ZEnd; z++)
                if (predicate(space[x, y, z]))
                    return true;

            return false;
        }

        /// <summary>
        ///     Determines whether any element at the specified Y index (adjusted for offset) satisfies the given predicate.
        /// </summary>
        public bool AnyAtY(int y, Predicate<T> predicate) {
            for (var x = space.XStart; x <= space.XEnd; x++)
            for (var z = space.ZStart; z <= space.ZEnd; z++)
                if (predicate(space[x, y, z]))
                    return true;

            return false;
        }

        /// <summary>
        ///     Determines whether any element at the specified Z index (adjusted for offset) satisfies the given predicate.
        /// </summary>
        public bool AnyAtZ(int z, Predicate<T> predicate) {
            for (var x = space.XStart; x <= space.XEnd; x++)
            for (var y = space.YStart; y <= space.YEnd; y++)
                if (predicate(space[x, y, z]))
                    return true;

            return false;
        }
    }
}
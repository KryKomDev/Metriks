namespace Metriks;

public static class Linq4DExtensions {

    /// <summary>
    ///     Projects each element of a four-dimensional list into a new form, returning a new four-dimensional list.
    /// </summary>
    public static List4D<TResult> Select<T, TResult>(this List4D<T> list, Func<T, TResult> selector) {
        if (list is null)
            throw new ArgumentNullException(nameof(list));

        if (selector is null)
            throw new ArgumentNullException(nameof(selector));

        var result = new List4D<TResult>(list.WSize, list.XSize, list.YSize, list.ZSize);
        result.Resize(list.WSize, list.XSize, list.YSize, list.ZSize);

        for (var w = 0; w < list.WSize; w++)
        for (var x = 0; x < list.XSize; x++)
        for (var y = 0; y < list.YSize; y++)
        for (var z = 0; z < list.ZSize; z++)
            result[w, x, y, z] = selector(list[w, x, y, z]);

        return result;
    }

    /// <summary>
    ///     Determines whether all elements at the specified W index satisfy the given predicate.
    /// </summary>
    public static bool AllAtW<T>(this List4D<T> list, int w, Predicate<T> predicate) {
        if (list is null)
            throw new ArgumentNullException(nameof(list));

        for (var x = 0; x < list.XSize; x++)
        for (var y = 0; y < list.YSize; y++)
        for (var z = 0; z < list.ZSize; z++)
            if (!predicate(list[w, x, y, z]))
                return false;

        return true;
    }

    /// <summary>
    ///     Determines whether all elements at the specified X index satisfy the given predicate.
    /// </summary>
    public static bool AllAtX<T>(this List4D<T> list, int x, Predicate<T> predicate) {
        if (list is null)
            throw new ArgumentNullException(nameof(list));

        for (var w = 0; w < list.WSize; w++)
        for (var y = 0; y < list.YSize; y++)
        for (var z = 0; z < list.ZSize; z++)
            if (!predicate(list[w, x, y, z]))
                return false;

        return true;
    }

    /// <summary>
    ///     Determines whether all elements at the specified Y index satisfy the given predicate.
    /// </summary>
    public static bool AllAtY<T>(this List4D<T> list, int y, Predicate<T> predicate) {
        if (list is null)
            throw new ArgumentNullException(nameof(list));

        for (var w = 0; w < list.WSize; w++)
        for (var x = 0; x < list.XSize; x++)
        for (var z = 0; z < list.ZSize; z++)
            if (!predicate(list[w, x, y, z]))
                return false;

        return true;
    }

    /// <summary>
    ///     Determines whether all elements at the specified Z index satisfy the given predicate.
    /// </summary>
    public static bool AllAtZ<T>(this List4D<T> list, int z, Predicate<T> predicate) {
        if (list is null)
            throw new ArgumentNullException(nameof(list));

        for (var w = 0; w < list.WSize; w++)
        for (var x = 0; x < list.XSize; x++)
        for (var y = 0; y < list.YSize; y++)
            if (!predicate(list[w, x, y, z]))
                return false;

        return true;
    }

    /// <summary>
    ///     Determines whether any element at the specified W index satisfies the given predicate.
    /// </summary>
    public static bool AnyAtW<T>(this List4D<T> list, int w, Predicate<T> predicate) {
        if (list is null)
            throw new ArgumentNullException(nameof(list));

        for (var x = 0; x < list.XSize; x++)
        for (var y = 0; y < list.YSize; y++)
        for (var z = 0; z < list.ZSize; z++)
            if (predicate(list[w, x, y, z]))
                return true;

        return false;
    }

    /// <summary>
    ///     Determines whether any element at the specified X index satisfies the given predicate.
    /// </summary>
    public static bool AnyAtX<T>(this List4D<T> list, int x, Predicate<T> predicate) {
        if (list is null)
            throw new ArgumentNullException(nameof(list));

        for (var w = 0; w < list.WSize; w++)
        for (var y = 0; y < list.YSize; y++)
        for (var z = 0; z < list.ZSize; z++)
            if (predicate(list[w, x, y, z]))
                return true;

        return false;
    }

    /// <summary>
    ///     Determines whether any element at the specified Y index satisfies the given predicate.
    /// </summary>
    public static bool AnyAtY<T>(this List4D<T> list, int y, Predicate<T> predicate) {
        if (list is null)
            throw new ArgumentNullException(nameof(list));

        for (var w = 0; w < list.WSize; w++)
        for (var x = 0; x < list.XSize; x++)
        for (var z = 0; z < list.ZSize; z++)
            if (predicate(list[w, x, y, z]))
                return true;

        return false;
    }

    /// <summary>
    ///     Determines whether any element at the specified Z index satisfies the given predicate.
    /// </summary>
    public static bool AnyAtZ<T>(this List4D<T> list, int z, Predicate<T> predicate) {
        if (list is null)
            throw new ArgumentNullException(nameof(list));

        for (var w = 0; w < list.WSize; w++)
        for (var x = 0; x < list.XSize; x++)
        for (var y = 0; y < list.YSize; y++)
            if (predicate(list[w, x, y, z]))
                return true;

        return false;
    }

    extension<T>(IEnumerable4D<T> list) {
        /// <summary>
        ///     Flattens the four-dimensional enumerable into a one-dimensional sequence.
        /// </summary>
        public IEnumerable<T> Flatten() {
            if (list is null)
                throw new ArgumentNullException(nameof(list));

            foreach (var cube in list)
            foreach (var plane in cube)
            foreach (var row in plane)
            foreach (var item in row)
                yield return item;
        }

        /// <summary>
        ///     Determines whether the four-dimensional enumerable contains any elements.
        /// </summary>
        public bool Any() => list.Flatten().Any();

        /// <summary>
        ///     Determines whether any element of the four-dimensional enumerable satisfies a condition.
        /// </summary>
        public bool Any(Func<T, bool> predicate) => list.Flatten().Any(predicate);

        /// <summary>
        ///     Determines whether all elements of the four-dimensional enumerable satisfy a condition.
        /// </summary>
        public bool All(Func<T, bool> predicate) => list.Flatten().All(predicate);

        /// <summary>
        ///     Returns the number of elements in the four-dimensional enumerable that satisfy a condition.
        /// </summary>
        public int Count(Func<T, bool> predicate) => list.Flatten().Count(predicate);

        /// <summary>
        ///     Returns the first element of the four-dimensional enumerable.
        /// </summary>
        public T First() => list.Flatten().First();

        /// <summary>
        ///     Returns the first element of the four-dimensional enumerable that satisfies a condition.
        /// </summary>
        public T First(Func<T, bool> predicate) => list.Flatten().First(predicate);

        /// <summary>
        ///     Returns the first element of the four-dimensional enumerable, or a default value if the sequence contains no
        ///     elements.
        /// </summary>
        public T? FirstOrDefault() => list.Flatten().FirstOrDefault();

        /// <summary>
        ///     Returns the first element of the four-dimensional enumerable that satisfies a condition or a default value if no
        ///     such element is found.
        /// </summary>
        public T? FirstOrDefault(Func<T, bool> predicate) => list.Flatten().FirstOrDefault(predicate);

        /// <summary>
        ///     Returns the last element of the four-dimensional enumerable.
        /// </summary>
        public T Last() => list.Flatten().Last();

        /// <summary>
        ///     Returns the last element of the four-dimensional enumerable that satisfies a condition.
        /// </summary>
        public T Last(Func<T, bool> predicate) => list.Flatten().Last(predicate);

        /// <summary>
        ///     Returns the last element of the four-dimensional enumerable, or a default value if the sequence contains no
        ///     elements.
        /// </summary>
        public T? LastOrDefault() => list.Flatten().LastOrDefault();

        /// <summary>
        ///     Returns the last element of the four-dimensional enumerable that satisfies a condition or a default value if no
        ///     such element is found.
        /// </summary>
        public T? LastOrDefault(Func<T, bool> predicate) => list.Flatten().LastOrDefault(predicate);

        /// <summary>
        ///     Returns the only element of the four-dimensional enumerable, and throws an exception if there is not exactly one
        ///     element in the sequence.
        /// </summary>
        public T Single() => list.Flatten().Single();

        /// <summary>
        ///     Returns the only element of the four-dimensional enumerable that satisfies a specified condition, and throws an
        ///     exception if more than one such element exists.
        /// </summary>
        public T Single(Func<T, bool> predicate) => list.Flatten().Single(predicate);

        /// <summary>
        ///     Returns the only element of the four-dimensional enumerable, or a default value if the sequence is empty; this
        ///     method throws an exception if there is more than one element in the sequence.
        /// </summary>
        public T? SingleOrDefault() => list.Flatten().SingleOrDefault();

        /// <summary>
        ///     Returns the only element of the four-dimensional enumerable that satisfies a specified condition or a default value
        ///     if no such element exists; this method throws an exception if more than one element satisfies the condition.
        /// </summary>
        public T? SingleOrDefault(Func<T, bool> predicate) => list.Flatten().SingleOrDefault(predicate);
    }
}
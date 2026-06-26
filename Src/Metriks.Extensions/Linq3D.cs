using System;
using System.Collections.Generic;
using System.Linq;

namespace Metriks;

public static class Linq3DExtensions {

    extension<T>(IEnumerable3D<T> list) {

        /// <summary>
        /// Flattens the three-dimensional enumerable into a one-dimensional sequence.
        /// </summary>
        public IEnumerable<T> Flatten() {
            foreach (var plane in list) {
                foreach (var row in plane) {
                    foreach (var item in row) {
                        yield return item;
                    }
                }
            }
        }

        /// <summary>
        /// Determines whether the three-dimensional enumerable contains any elements.
        /// </summary>
        public bool Any() => Flatten(list).Any();

        /// <summary>
        /// Determines whether any element of the three-dimensional enumerable satisfies a condition.
        /// </summary>
        public bool Any(Func<T, bool> predicate) => Flatten(list).Any(predicate);

        /// <summary>
        /// Determines whether all elements of the three-dimensional enumerable satisfy a condition.
        /// </summary>
        public bool All(Func<T, bool> predicate) => Flatten(list).All(predicate);

        /// <summary>
        /// Returns the number of elements in the three-dimensional enumerable that satisfy a condition.
        /// </summary>
        public int Count(Func<T, bool> predicate) => Flatten(list).Count(predicate);

        /// <summary>
        /// Returns the first element of the three-dimensional enumerable.
        /// </summary>
        public T First() => Flatten(list).First();

        /// <summary>
        /// Returns the first element of the three-dimensional enumerable that satisfies a condition.
        /// </summary>
        public T First(Func<T, bool> predicate) => Flatten(list).First(predicate);

        /// <summary>
        /// Returns the first element of the three-dimensional enumerable, or a default value if the sequence contains no elements.
        /// </summary>
        public T? FirstOrDefault() => Flatten(list).FirstOrDefault();

        /// <summary>
        /// Returns the first element of the three-dimensional enumerable that satisfies a condition or a default value if no such element is found.
        /// </summary>
        public T? FirstOrDefault(Func<T, bool> predicate) => Flatten(list).FirstOrDefault(predicate);

        /// <summary>
        /// Returns the last element of the three-dimensional enumerable.
        /// </summary>
        public T Last() => Flatten(list).Last();

        /// <summary>
        /// Returns the last element of the three-dimensional enumerable that satisfies a condition.
        /// </summary>
        public T Last(Func<T, bool> predicate) => Flatten(list).Last(predicate);

        /// <summary>
        /// Returns the last element of the three-dimensional enumerable, or a default value if the sequence contains no elements.
        /// </summary>
        public T? LastOrDefault() => Flatten(list).LastOrDefault();

        /// <summary>
        /// Returns the last element of the three-dimensional enumerable that satisfies a condition or a default value if no such element is found.
        /// </summary>
        public T? LastOrDefault(Func<T, bool> predicate) => Flatten(list).LastOrDefault(predicate);

        /// <summary>
        /// Returns the only element of the three-dimensional enumerable, and throws an exception if there is not exactly one element in the sequence.
        /// </summary>
        public T Single() => Flatten(list).Single();

        /// <summary>
        /// Returns the only element of the three-dimensional enumerable that satisfies a specified condition, and throws an exception if more than one such element exists.
        /// </summary>
        public T Single(Func<T, bool> predicate) => Flatten(list).Single(predicate);

        /// <summary>
        /// Returns the only element of the three-dimensional enumerable, or a default value if the sequence is empty; this method throws an exception if there is more than one element in the sequence.
        /// </summary>
        public T? SingleOrDefault() => Flatten(list).SingleOrDefault();

        /// <summary>
        /// Returns the only element of the three-dimensional enumerable that satisfies a specified condition or a default value if no such element exists; this method throws an exception if more than one element satisfies the condition.
        /// </summary>
        public T? SingleOrDefault(Func<T, bool> predicate) => Flatten(list).SingleOrDefault(predicate);
    }

    extension<T>(List3D<T> list) {

        /// <summary>
        /// Projects each element of a three-dimensional list into a new form, returning a new three-dimensional list.
        /// </summary>
        public List3D<TResult> Select<TResult>(Func<T, TResult> selector) {
            if (selector is null) throw new ArgumentNullException(nameof(selector));
            var result = new List3D<TResult>(list.XSize, list.YSize, list.ZSize);
            result.Resize(list.XSize, list.YSize, list.ZSize);
            for (int x = 0; x < list.XSize; x++) {
                for (int y = 0; y < list.YSize; y++) {
                    for (int z = 0; z < list.ZSize; z++) {
                        result[x, y, z] = selector(list[x, y, z]);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Determines whether all elements at the specified X index satisfy the given predicate.
        /// </summary>
        public bool AllAtX(int x, Predicate<T> predicate) {
            for (int y = 0; y < list.YSize; y++) {
                for (int z = 0; z < list.ZSize; z++) {
                    if (!predicate(list[x, y, z])) return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Determines whether all elements at the specified Y index satisfy the given predicate.
        /// </summary>
        public bool AllAtY(int y, Predicate<T> predicate) {
            for (int x = 0; x < list.XSize; x++) {
                for (int z = 0; z < list.ZSize; z++) {
                    if (!predicate(list[x, y, z])) return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Determines whether all elements at the specified Z index satisfy the given predicate.
        /// </summary>
        public bool AllAtZ(int z, Predicate<T> predicate) {
            for (int x = 0; x < list.XSize; x++) {
                for (int y = 0; y < list.YSize; y++) {
                    if (!predicate(list[x, y, z])) return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Determines whether any element at the specified X index satisfies the given predicate.
        /// </summary>
        public bool AnyAtX(int x, Predicate<T> predicate) {
            for (int y = 0; y < list.YSize; y++) {
                for (int z = 0; z < list.ZSize; z++) {
                    if (predicate(list[x, y, z])) return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Determines whether any element at the specified Y index satisfies the given predicate.
        /// </summary>
        public bool AnyAtY(int y, Predicate<T> predicate) {
            for (int x = 0; x < list.XSize; x++) {
                for (int z = 0; z < list.ZSize; z++) {
                    if (predicate(list[x, y, z])) return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Determines whether any element at the specified Z index satisfies the given predicate.
        /// </summary>
        public bool AnyAtZ(int z, Predicate<T> predicate) {
            for (int x = 0; x < list.XSize; x++) {
                for (int y = 0; y < list.YSize; y++) {
                    if (predicate(list[x, y, z])) return true;
                }
            }
            return false;
        }
    }

    extension<T>(Space3D<T> space) {

        /// <summary>
        /// Projects each element of a three-dimensional spatial grid into a new form, returning a new Space3D.
        /// </summary>
        public Space3D<TResult> Select<TResult>(Func<T, TResult> selector) {
            if (selector is null) throw new ArgumentNullException(nameof(selector));
            var result = new Space3D<TResult>(space.XSize, space.YSize, space.ZSize);
            result.Resize(space.XSize, space.YSize, space.ZSize);
            result.MoveOrigin(space.XOriginOffset, space.YOriginOffset, space.ZOriginOffset);
            for (int x = space.XStart; x <= space.XEnd; x++) {
                for (int y = space.YStart; y <= space.YEnd; y++) {
                    for (int z = space.ZStart; z <= space.ZEnd; z++) {
                        result[x, y, z] = selector(space[x, y, z]);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Determines whether all elements at the specified X index (adjusted for offset) satisfy the given predicate.
        /// </summary>
        public bool AllAtX(int x, Predicate<T> predicate) {
            for (int y = space.YStart; y <= space.YEnd; y++) {
                for (int z = space.ZStart; z <= space.ZEnd; z++) {
                    if (!predicate(space[x, y, z])) return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Determines whether all elements at the specified Y index (adjusted for offset) satisfy the given predicate.
        /// </summary>
        public bool AllAtY(int y, Predicate<T> predicate) {
            for (int x = space.XStart; x <= space.XEnd; x++) {
                for (int z = space.ZStart; z <= space.ZEnd; z++) {
                    if (!predicate(space[x, y, z])) return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Determines whether all elements at the specified Z index (adjusted for offset) satisfy the given predicate.
        /// </summary>
        public bool AllAtZ(int z, Predicate<T> predicate) {
            for (int x = space.XStart; x <= space.XEnd; x++) {
                for (int y = space.YStart; y <= space.YEnd; y++) {
                    if (!predicate(space[x, y, z])) return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Determines whether any element at the specified X index (adjusted for offset) satisfies the given predicate.
        /// </summary>
        public bool AnyAtX(int x, Predicate<T> predicate) {
            for (int y = space.YStart; y <= space.YEnd; y++) {
                for (int z = space.ZStart; z <= space.ZEnd; z++) {
                    if (predicate(space[x, y, z])) return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Determines whether any element at the specified Y index (adjusted for offset) satisfies the given predicate.
        /// </summary>
        public bool AnyAtY(int y, Predicate<T> predicate) {
            for (int x = space.XStart; x <= space.XEnd; x++) {
                for (int z = space.ZStart; z <= space.ZEnd; z++) {
                    if (predicate(space[x, y, z])) return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Determines whether any element at the specified Z index (adjusted for offset) satisfies the given predicate.
        /// </summary>
        public bool AnyAtZ(int z, Predicate<T> predicate) {
            for (int x = space.XStart; x <= space.XEnd; x++) {
                for (int y = space.YStart; y <= space.YEnd; y++) {
                    if (predicate(space[x, y, z])) return true;
                }
            }
            return false;
        }
    }
}
using System.Runtime.InteropServices;

namespace Metriks;

internal static class MetriksHelpers {

    /// <summary>
    ///     Retrieves a reference to the first element of the given array. This method is optimized
    ///     for scenarios where
    ///     direct access to the array's memory is required.
    /// </summary>
    /// <typeparam name="T">The type of the elements contained in the array.</typeparam>
    /// <param name="array">The array from which to get a reference to its data.</param>
    /// <returns>A reference to the first element of the provided array.</returns>
    internal static ref T GetArrayData<T>(T[] array) {
        #if NET5_0_OR_GREATER
        return ref MemoryMarshal.GetArrayDataReference(array);
        #else
        return ref MemoryMarshal.GetReference(new Span<T>(array));
        #endif
    }
}
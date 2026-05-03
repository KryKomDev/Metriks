# Array Utilities & Extensions

Metriks provides powerful static utility classes and extension methods for working with standard .NET multidimensional arrays (`T[,]`, `T[,,]`, etc.).

## Optimized Copying

The `Array2D`, `Array3D`, and `Array4D` static classes provide high-performance copy operations that use Spans and `Unsafe` memory access internally.

```csharp
using Metriks;

int[,] source = { { 1, 2, 3 }, { 4, 5, 6 } };
int[,] destination = new int[5, 5];

// Copy a 2x2 region from source to destination
Array2D.Copy(
    source, 0, 0,      // source, startX, startY
    destination, 1, 1, // destination, startX, startY
    2, 2               // width, height to copy
);
```

## Array Extensions

Metriks adds many useful extension methods to standard arrays to make them feel more like first-class objects.

### Size and Length
Access dimensions via properties instead of calling `GetLength()`.

```csharp
int[,] array = new int[10, 20];

int width = array.Len0;  // equivalent to array.GetLength(0)
int height = array.Len1; // equivalent to array.GetLength(1)

Size2D size = array.Size; // returns a Size2D(10, 20)

// For 4D arrays (T[,,,]):
// Len0 -> W, Len1 -> X, Len2 -> Y, Len3 -> Z
```

### Filling
Easily fill entire arrays or specific regions.

```csharp
array.Fill(42); // Fills entire array with 42

// Fill a specific region
array.Fill(value: -1, start0: 2, count0: 3, start1: 2, count1: 3);

// Safe fill (clamped to array bounds)
array.SafeFill(0, -5, 100, -5, 100);
```

### Conversions
```csharp
// Convert a multidimensional array to a jagged array (T[][])
int[][] jagged = array.ToJagged();
```

### Clearing
```csharp
// Clear a region of the array
ArrayManipulation.Clear(array, 0, 5, 0, 5);
```

## Summary of Utility Classes

- `Array2D`: Methods for `T[,]`
- `Array3D`: Methods for `T[,,]`
- `Array4D`: Methods for `T[,,,]`
- `ArrayManipulation`: General array extensions and range operations.
- `ArrayDimensions`: Extension properties for lengths and sizes.

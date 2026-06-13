# Metriks

## About

Metriks is a high-performance C# library providing dynamic multidimensional arrays (lists).
It bridges the gap between fixed-size multidimensional arrays (`T[,]`, `T[,,]`) and the
flexibility of `List<T>`, allowing you to grow, shrink, and resize your multidimensional
data structures easily.

## Features

- **Specialized Implementations:** Optimized `List2D<T>`, `List3D<T>`, and `List4D<T>` for common dimensions.
- **Dynamic Resizing:** Easily `Expand`, `Shrink`, or `Resize` your collections while preserving data.
- **Familiar API:** Implements standard collection interfaces and specialized multidimensional contracts (`IList2D<T>`, `IList3D<T>`, `IList4D<T>`).
- **Modern C# Support:** Full support for `Index` and `Range` syntax on modern .NET targets.
- **Rich Extensions:** Includes LINQ-like operations, flattening, and utility extensions for multidimensional data.
- **Spatial Grid Support:** `Space2D<T>` provides coordinates relative to movable origins (negative coordinates supported).
- **Broad Compatibility:** Targets .NET Standard 2.1, .NET 5.0, .NET 8.0, and .NET 10.0.

## Installation

Install via NuGet:

```bash
dotnet add package Metriks
```

## Quick Start

### 2D List

```csharp
using Metriks;

// Create a 2D list with initial size 0x0 (default capacity 4x4)
var list = new List2D<int>();

// Expand the list size to 3x3 and fill with values
list.Expand(3, 3);
list[0, 0] = 1;
list[1, 1] = 2;
list[2, 2] = 3;

// Access using standard indexers
int value = list[1, 1]; // 2

// Resize the list while keeping existing data
list.Resize(5, 5);

// Use modern C# indexers (on supported frameworks)
var last = list[^1, ^1];
```

### 3D List

Note: Constructor parameters define initial capacity, not size. Use `Resize` or `Expand` to change size before indexing elements.

```csharp
var world = new List3D<string>();
world.Resize(10, 10, 10);
world[5, 2, 7] = "Block";
Console.WriteLine(world.Size); // "10x10x10"
```

### Shifted Spatial Grid (Space2D)

`Space2D<T>` maps coordinate boundaries relative to an origin offset. This allows you to work with negative coordinates seamlessly.

```csharp
var space = new Space2D<int>(new int[,] {
    { 10, 20 },
    { 30, 40 }
});

// Move the origin offset by (1, 1)
space.MoveOrigin(1, 1);

// The original elements are now shifted:
// Element originally at [0,0] (value 10) is now at [-1, -1]
int shiftedValue = space[-1, -1]; // 10
Console.WriteLine($"Current bounds: X[{space.XStart} to {space.XEnd}], Y[{space.YStart} to {space.YEnd}]");
```

### Slicing and Flattening

Metriks offers optimized slicing along axes and flattening operations:

```csharp
var list3D = new List3D<int>(new int[,,] {
    { { 1, 2 }, { 3, 4 } },
    { { 5, 6 }, { 7, 8 } }
});

// Extract a 2D slice from a 3D list at X index 1
IEnumerable2D<int> slice2D = list3D.GetAtX(1);
slice2D.Write(); // Prints a formatted 2D layer representation

// Flatten a 2D array to a 1D array
int[,] array2D = { { 1, 2 }, { 3, 4 } };
int[] flat = Array2D.Flatten(array2D); // [1, 2, 3, 4]
```

## Resizing Behavior

Metriks provides several ways to change the size of your collections:

- **Expand:** Increases the size of the list. New elements are initialized with the default
  value or a provided factory/default.
- **Shrink:** Decreases the size of the list, removing elements outside the new bounds.
- **Resize:** A general-purpose method to set the exact size, either expanding or shrinking as needed.

For detailed information on how data is preserved during resizing, see the
[Expand](https://krykomdev.github.io/Metriks/docs/Expand-Methods.html), 
[Shrink](https://krykomdev.github.io/Metriks/docs/Shrink-Methods.html), and 
[Resize](https://krykomdev.github.io/Metriks/docs/Resize-Methods.html) documentation.

## Performance

Metriks is designed with performance in mind:
- Uses jagged arrays internally for efficient memory management and access.
- Aggressive inlining for hot paths.
- Employs span-based memory copying (`Span<T>`) for bulk data transfers and resize operations.
- Specialized classes for 2D, 3D, and 4D to avoid the overhead of generic N-dimensional
  indexing where possible.

## License

This project is licensed under the MIT License – see the 
[LICENSE](https://github.com/KryKomDev/Metriks/blob/master/LICENSE) file for details.

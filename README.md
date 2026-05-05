<h1 align="center">Metriks</h1>

<p align="center">A multidimensional dynamic array library for C#.</p>

<div align="center">
    <p>
        <img src="https://img.shields.io/github/license/KryKomDev/Metriks?style=for-the-badge&amp;labelColor=%235F6473&amp;color=%23F2A0A0" alt="GitHub License" />
        <a href="https://www.nuget.org/packages/Metriks"><img src="https://img.shields.io/nuget/v/Metriks?color=F0CA95&amp;style=for-the-badge&amp;labelColor=5F6473" alt="NuGet" /></a>
        <img src="https://img.shields.io/nuget/dt/Metriks?color=E3ED8A&amp;style=for-the-badge&amp;labelColor=5F6473" alt="NuGet Downloads" />
        <img src="https://img.shields.io/github/actions/workflow/status/KryKomDev/Metriks/test.yml?style=for-the-badge&amp;labelColor=%235F6473&amp;color=%2395EC7D" alt="GitHub Actions Workflow Status" />
        <img src="https://img.shields.io/badge/.NET-Standard2.1-7ACFDC?style=for-the-badge&amp;labelColor=5F6473" alt=".NET Standard" />
    </p>
</div>

Metriks is a high-performance C# library providing dynamic multidimensional arrays (lists). 
It bridges the gap between fixed-size multidimensional arrays (`T[,]`, `T[,,]`) and the 
flexibility of `List<T>`, allowing you to grow, shrink, and resize your multidimensional 
data structures easily.

## Features

- **Specialized Implementations:** Optimized `List2D<T>`, `List3D<T>`, and `List4D<T>` for common dimensions.
- **Dynamic Resizing:** Easily `Expand`, `Shrink`, or `Resize` your collections while preserving data.
- **Familiar API:** Implements `IEnumerable`, `ICollection`, and specialized multidimensional interfaces.
- **Modern C# Support:** Full support for `Index` and `Range` syntax on modern .NET targets.
- **Rich Extensions:** Includes LINQ-like operations and utility extensions for multidimensional data.
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

// Expand the list to 3x3 and fill with values
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

```csharp
var world = new List3D<string>(10, 10, 10);
world[5, 2, 7] = "Block";
Console.WriteLine(world.Size); // 10, 10, 10
```

## Resizing Behavior

Metriks provides several ways to change the size of your collections:

- **Expand:** Increases the size of the list. New elements are initialized with the default
  value or a provided one.
- **Shrink:** Decreases the size of the list, removing elements outside the new bounds.
- **Resize:** A general-purpose method to set the exact size, either expanding or shrinking as needed.

For detailed information on how data is preserved during resizing, see the 
[Expand](Docs/Docs/Expand-Methods.md), [Shrink](Docs/Docs/Shrink-Methods.md), and [Resize](Docs/Docs/Resize-Methods.md) documentation.

## Performance

Metriks is designed with performance in mind:
- Uses jagged arrays internally for efficient memory management and access.
- Aggressive inlining for hot paths.
- Specialized classes for 2D, 3D, and 4D to avoid the overhead of generic N-dimensional 
  indexing where possible.

## License

This project is licensed under the MIT License – see the [LICENSE](LICENSE) file for details.

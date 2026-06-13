# Using 2D Spaces

`Space2D<T>` is a specialized `List2D<T>` that supports a coordinate system with an origin offset. This allows you to use negative indices and define where `(0, 0)` is located within your data.

## Why use Space2D?

In many applications (like games or simulations), a grid might grow in all directions. `Space2D` makes this easy by decoupling the data storage from the coordinate system.

## Basic Usage

```csharp
using Metriks;

var space = new Space2D<string>();
space.Resize(3, 3); // Internal size is 3x3

// By default, origin is at (0, 0)
space[0, 0] = "Center";
space[1, 1] = "Top-Right";

// You can shift the origin
// Moving origin by (1, 1) means the previous (1, 1) is now (0, 0)
// and the previous (0, 0) is now (-1, -1)
// This is done internally by adjusting the origin offset.
```

## Coordinate System

`Space2D` provides properties to find the bounds of your coordinate system:

- `XStart`, `YStart`: The minimum valid indices (inclusive).
- `XEnd`, `YEnd`: The maximum valid indices (inclusive).

```csharp
// If origin offset is (1, 1) and size is 3x3:
// XStart = -1, XEnd = 1
// YStart = -1, YEnd = 1
```

## Origin Management

When you insert or remove rows/columns, `Space2D` intelligently updates the origin offset:

- If you insert a column at a position *before or at* the current origin, the origin shifts to 
  maintain its logical position relative to the data.
- If you insert *after* the origin, the origin stays put.

```csharp
space.InsertAtX(-1); // Inserts a new column at the beginning and shifts origin
```

## Uncoordinated Access

Sometimes you want to access the underlying data without the coordinate offset (as if it were a 
normal `List2D`).

```csharp
// These ignore the origin offset
space.UncoordinatedSet(0, 0, "Top-Left-most data");
var val = space.UncoordinatedGet(0, 0);
```

For a deep dive into how origin shifts during manipulation, see the 
[Insert Methods](../Docs/Insert-Methods.md) and [Remove Methods](../Docs/Remove-Methods.md) 
article.

# Helper Structs: Point, Size, and Area

Metriks provides lightweight `readonly record struct` types to represent positions, dimensions, and regions in 2D, 3D, and 4D space.

## Point (`Point2D`, `Point3D`, `Point4D`)

Represents a position in space.

```csharp
var p1 = new Point2D(10, 20);
var p2 = new Point2D(5, 5);

// Arithmetic operations
var p3 = p1 + p2; // (15, 25)
var p4 = p1 * 2;  // (20, 40) - if supported, otherwise manually

// Constants
var zero = Point2D.Zero;
var one = Point2D.One;

// Deconstruction
var (x, y) = p1;

// 4D Example (W, X, Y, Z)
var p4d = new Point4D(1, 2, 3, 4);
var (w, px, py, pz) = p4d;
```

## Size (`Size2D`, `Size3D`, `Size4D`)

Represents dimensions or extents. Similar to `Point` but logically used for sizes.

```csharp
var size = new Size2D(100, 200);
int width = size.X;
int height = size.Y;

// Convert Point to Size
Size2D sFromP = p1.ToSize();
```

## Area (`Area2D`, `Area3D`, `Area4D`)

Represents a region defined by a lower and higher bound, or a point and a size.

```csharp
var lower = new Point2D(0, 0);
var higher = new Point2D(10, 10);

// Define by bounds
var area = new Area2D(lower, higher);

// Define by point and size
var area2 = new Area2D(lower, new Size2D(10, 10));

// Properties
Point2D min = area.Lower;
Point2D max = area.Higher;
Size2D s = area.Size;

// Shifting an area
var shiftedArea = area + new Point2D(5, 5);
```

> [!TIP]
> Using these structs makes your code more readable and reduces the number of parameters in your methods.
> Most Metriks classes have properties and methods that accept or return these structs (e.g., `list.Size`, `space.OriginOffset`).

```csharp
// Instead of this:
void ProcessRegion(int x, int y, int width, int height) { ... }

// Do this:
void ProcessRegion(Area2D area) { ... }
```

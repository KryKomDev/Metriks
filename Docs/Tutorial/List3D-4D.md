# Higher Dimensional Lists (3D & 4D)

Metriks provides `List3D<T>` and `List4D<T>` for working with three and four dimensions. They follow the same patterns as `List2D<T>`.

## List3D Usage

### Initialization
```csharp
using Metriks;

// Empty list
var list3d = new List3D<int>();

// With specific capacities (X, Y, Z)
var list3dCap = new List3D<int>(10, 10, 10);

// From 3D array
int[,,] array3d = new int[2, 2, 2];
var listFromArr = new List3D<int>(array3d);
```

### Basic Operations
```csharp
list3d[0, 1, 2] = 100;
int xSize = list3d.XSize;
int ySize = list3d.YSize;
int zSize = list3d.ZSize;
```

### Adding/Inserting/Removing Slices
Instead of rows and columns, you work with X, Y, and Z planes.
```csharp
list3d.AddZ(); // Adds a new plane along the Z axis
list3d.InsertAtY(2); // Inserts a new plane at Y index 2
list3d.ShrinkX(); // Removes the last plane along the X axis
list3d.RemoveAtX(0); // Removes the first plane along the X axis
```

## List4D Usage

### Initialization
```csharp
// Constructor takes (W, X, Y, Z) capacities
var list4d = new List4D<int>(5, 5, 5, 5);
```

### Basic Operations
```csharp
// Indexing is [w, x, y, z]
list4d[0, 1, 2, 3] = 500;
int wSize = list4d.WSize;
int xSize = list4d.XSize;
int ySize = list4d.YSize;
int zSize = list4d.ZSize;
```

### Adding/Inserting/Removing
```csharp
list4d.AddW();
list4d.InsertAtW(1);
list4d.ShrinkW();
```

## Summary of Dimensions

| Class | Indices | Size Properties |
| :--- | :--- | :--- |
| `List2D<T>` | `[x, y]` | `XSize`, `YSize` |
| `List3D<T>` | `[x, y, z]` | `XSize`, `YSize`, `ZSize` |
| `List4D<T>` | `[w, x, y, z]` | `WSize`, `XSize`, `YSize`, `ZSize` |

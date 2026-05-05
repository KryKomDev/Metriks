# Class Structure

Metriks is a .NET library designed for efficient management of multidimensional data. It provides dynamic lists, coordinate-mapped spaces, and utility methods for 2D, 3D, and 4D structures.

## Core Concepts

The library is organized around three main pillars for each dimension:

### 1. Dynamic Lists (`List2D`, `List3D`, `List4D`)

These are the multidimensional equivalent of `System.Collections.Generic.List<T>`. They support:

- Dynamic resizing and capacity management.
- Adding or removing entire rows, columns, or slices.
- Standard indexing (zero-based).

### 2. Spaces (`Space2D`, `Space3D`)

Spaces extend Dynamic Lists by adding a coordinate system with an origin offset.

- Supports negative indices.
- Useful for grids where the "center" or "start" might not be at `(0, 0)`.
- Automatically manages origin shifts during insertions or removals.

### 3. Utility Classes (`Array2D`, `Array3D`, `Array4D`)

Static classes providing high-performance operations on standard .NET multidimensional arrays (`T[,]`, `T[,,]`, etc.), such as:
- 
- Optimized copying using Spans and `Unsafe` memory access.
- Transformations and manipulations.

## Helper Structs

To make working with dimensions easier, Metriks provides several lightweight structs:

- **Point**: Represents a position (`Point2D`, `Point3D`, `Point4D`).
- **Size**: Represents dimensions/extents (`Size2D`, `Size3D`, `Size4D`).
- **Area**: Represents a region defined by a point and a size (`Area2D`, `Area3D`, `Area4D`).

## Namespace Organization

All core types reside in the `Metriks` namespace. The internal implementation is physically separated into directories by dimension (`2D/`, `3D/`, `4D/`), but they share the same namespace for ease of use.

| Feature | 2D | 3D | 4D |
| :--- | :---: | :---: | :---: |
| Dynamic List | ✅ | ✅ | ✅ |
| Space | ✅ | ✅ | ❌ |
| Utilities | ✅ | ✅ | ✅ |
| Point/Size/Area | ✅ | ✅ | ✅ |
| Extensions | ✅ | ✅ | ✅ |

# Shrink Methods

Shrinking allows you to reduce the size of a collection. This can be done by specifying new dimensions or by removing the last element of a specific dimension.

## Explicit Shrinking

The `Shrink` method reduces the collection to the specified size. It throws an `ArgumentOutOfRangeException` if any of the new dimensions are larger than the current ones.

```csharp
var list = new List2D<int>(10, 10);
list.Shrink(5, 5); // Reduces to 5x5, freeing memory
```

## Quick Shrinking

The `Shrink{Dim}` methods remove the last "row", "column", or "plane" of the collection, reducing its size in that dimension by exactly one.

### 2D List Quick Shrink

- `ShrinkX()`: Removes the last column.
- `ShrinkY()`: Removes the last row.

```csharp
var list = new List2D<int>(3, 3);
list.ShrinkX(); // Now 2x3
list.ShrinkY(); // Now 2x2
```

### 3D and 4D Quick Shrink

- `List3D`: `ShrinkX()`, `ShrinkY()`, `ShrinkZ()`.
- `List4D`: `ShrinkW()`, `ShrinkX()`, `ShrinkY()`, `ShrinkZ()`.

## Shrinking vs. Resizing

While `Resize` can also be used to shrink a collection, the `Shrink` and `Shrink{Dim}` methods are more specialized:

- `Shrink` explicitly signals the intent to reduce size and will fail if you accidentally try to grow the collection.
- `Shrink{Dim}` is a convenient way to pop the last element of a dimension without needing to know the current size.

## Effects on Memory

> [!NOTE]
> Calling `Shrink(x, y)` (or `Resize`) usually triggers a reallocation of the underlying storage to match the new size, which can help reduce memory usage after large deletions.
> In contrast, `ShrinkX()` and `ShrinkY()` typically just decrement the size counter and null out references (for reference types) to allow for garbage collection of the removed elements.

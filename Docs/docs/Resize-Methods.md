# Resize Methods

Resizing allows you to change the dimensions of a collection to any non-negative size. Unlike `Expand`, `Resize` can both increase and decrease the size of the collection.

## Resizing Lists

The `Resize` method is available on `List2D`, `List3D`, and `List4D`.

### Growing with Resize

If the new dimensions are larger than the current ones, `Resize` behaves like `Expand`, filling the new space with a default value or using a factory.

```csharp
var list = new List2D<int>(2, 2);
list.Resize(4, 4, 10); // Grows to 4x4, new elements are 10
```

### Shrinking with Resize

If the new dimensions are smaller, the elements outside the new bounds are removed.

```csharp
var list = new List2D<int>(4, 4);
list.Resize(2, 2); // Shrinks to 2x2, elements at indices >= 2 are lost
```

If we have a 3x3 list:

y\x| 0 | 1 | 2
---|---|---|---
 0 | a | b | c
 1 | d | e | f
 2 | g | h | i

Calling `Resize(2, 2)` results in:

y\x| 0 | 1 
---|---|---
 0 | a | b 
 1 | d | e 

## Resizing Spaces

Resizing a `Space2D` or `Space3D` changes its bounds while maintaining its origin.

```csharp
var space = new Space2D<int>(10, 10);
space.Resize(5, 5); // Reduces the coordinate bounds
```

> [!NOTE]
> Resizing a space might cut off elements if they fall outside the new coordinate range relative to the origin.

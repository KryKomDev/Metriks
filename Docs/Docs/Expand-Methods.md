# Expand Methods

Expanding a collection increases its size in one or more dimensions. Unlike `Resize`, `Expand` only allows increasing the size; attempting to reduce any dimension will result in an `ArgumentOutOfRangeException`.

## Expanding Lists

You can expand a `List2D`, `List3D`, or `List4D` using the `Expand` method.

### 2D List Expansion

```csharp
var list = new List2D<int>(2, 2); // 2x2 list
list.Expand(4, 4, -1);            // Expands to 4x4, filling new elements with -1
```

If we have a 2x2 list:

y\x| 0 | 1 
---|---|---
 0 | a | b 
 1 | c | d 

Calling `Expand(3, 3, 'x')` results in:

y\x| 0 | 1 | 2
---|---|---|---
 0 | a | b | x
 1 | c | d | x
 2 | x | x | x

### Default Value Factory

For reference types or when you need a new instance for each element, you can use a factory function:

```csharp
var list = new List2D<MyClass>(2, 2);
list.Expand(4, 4, () => new MyClass());
```

## Expanding Spaces

Expanding a `Space2D` or `Space3D` works similarly to lists, but it preserves the origin's relative position.

### Expansion in Spaces

When you expand a space, the capacity of the underlying storage is increased, and the coordinate system is extended.

```csharp
var space = new Space2D<int>(2, 2);
space.Expand(5, 5); // Increases the bounds of the space
```

> [!TIP]
> Methods like `Place` have a `resize` parameter (true by default) that automatically calls `Expand` if the object being placed exceeds the current bounds.

```csharp
var list = new List2D<int>(2, 2);
var other = new List2D<int>(3, 3);
list.Place(other, new Point2D(1, 1)); // list will automatically expand to 4x4
```

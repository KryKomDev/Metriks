# Using 2D Lists

`List2D<T>` is a dynamic two-dimensional array that behaves similarly to `List<T>`. It allows you to grow or shrink dimensions independently.

## Initialization

You can create a `List2D<T>` with an initial capacity or from an existing 2D array.

```csharp
using Metriks;

// Create an empty list with default capacity
var list = new List2D<int>();

// Create a list with specific initial capacity (X, Y)
var listWithCapacity = new List2D<int>(10, 10);

// Create from an existing 2D array
int[,] array = { { 1, 2 }, { 3, 4 } };
var listFromArray = new List2D<int>(array);
```

## Basic Operations

### Indexing
Access elements using the `[x, y]` indexer. It is zero-based.

```csharp
list[0, 0] = 42;
int value = list[0, 0];
```

### Getting Size
Use `XSize`, `YSize`, or the `Size` property (which returns a `Size2D` struct).

```csharp
int width = list.XSize;
int height = list.YSize;
Size2D size = list.Size;
```

## Manipulating Dimensions

Unlike standard arrays, `List2D` allows you to add or remove rows and columns.

### Adding
`AddX()` adds a new column at the end, and `AddY()` adds a new row at the end.

```csharp
list.AddX(); // Increases XSize by 1
list.AddY(); // Increases YSize by 1
```

### Inserting
`InsertAtX(index)` and `InsertAtY(index)` allow you to insert at a specific position. Existing elements are shifted to make room.

```csharp
list.InsertAtX(1); // Inserts a new column at index 1
```

### Removing
`RemoveX()` and `RemoveY()` remove from the end. `RemoveAtX(index)` and `RemoveAtY(index)` remove at a specific position.

```csharp
list.RemoveAtY(0); // Removes the first row
```

## Resizing

You can manually control the size and capacity.

- **Resize(x, y)**: Sets the list to the exact size, copying old data where it fits.
- **Expand(x, y)**: Ensures the list is at least the specified size. Throws if the new size is smaller than current.
- **Shrink(x, y)**: Reduces the list to the specified size. Throws if the new size is larger than current.

```csharp
list.Resize(5, 5);
```

For more details on insertion/removal behavior, see the [Insert / Remove Behaviour](../Articles/Insert-Remove-Behaviour.md) article.

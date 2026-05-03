# LINQ & Querying Extensions

Metriks provides LINQ-like extension methods for querying multidimensional structures, allowing you to perform operations on specific dimensions easily.

## Querying by Dimension

You can retrieve entire rows or columns as enumerables.

```csharp
using Metriks;

var list = new List2D<int>(new[,] { { 1, 2 }, { 3, 4 } });

// Get all elements in the first column (X=0)
IEnumerable<int> column0 = list.GetAtX(0);

// Get all elements in the second row (Y=1)
IEnumerable<int> row1 = list.GetAtY(1);
```

## Validation & Predicates

Use `AllAtX` or `AllAtY` to verify if elements in a specific row or column meet a condition.

```csharp
// Check if all elements in the first column are positive
bool allPositive = list.AllAtX(0, val => val > 0);

// Check if any element in the second row is even
// (Note: Currently only AllAtX/AllAtY are implemented in Linq2D)
```

## Projection

You can project sub-collections using `Select`.

```csharp
// Calculate the sum of each column
IEnumerable<int> columnSums = list.Select(column => column.Sum());
```

## Numeric Extensions

Metriks also includes numeric utilities like `Clamp`.

```csharp
int value = 150;
int clamped = value.Clamp(0, 100); // 100
```

## Important Note

The `Linq2D` extensions are designed to work with `IEnumerable2D<T>`, `List2D<T>`, and `Space2D<T>`. When using them with `Space2D`, the indices are automatically adjusted for the origin offset.

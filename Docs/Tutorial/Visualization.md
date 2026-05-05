# Visualization & Debugging

Metriks provides built-in methods to help you visualize your multidimensional data in the console.

## Formatting as Strings

You can convert any `IEnumerable2D<T>` (including `List2D`, `Space2D`, and standard arrays) into a formatted string.

```csharp
using Metriks;

int[,] array = { { 1, 2 }, { 3, 4 } };
string output = array.ToCollectionString();
Console.WriteLine(output);
```

**Output:**
```text
[
   [ 1, 2 ],
   [ 3, 4 ]
]
```

## Printing to Console

For quick debugging, use the `.Write()` extension method.

```csharp
array.Write(); // Prints the formatted collection directly to Console
```

## Space2D Table View

`Space2D<T>` has a specialized `WriteTable()` method that prints a labeled grid, which is extremely useful for verifying coordinate mappings and origin offsets.

```csharp
var space = new Space2D<int>(3, 3);
space.Resize(3, 3);
// ... fill space ...

space.WriteTable();
```

This will output a grid with X and Y indices as headers, helping you see exactly where your data is relative to the origin.

## ToString Support

The helper structs like `Point2D`, `Size2D`, and `Area2D` all have clean `ToString()` implementations that respect current culture settings.

```csharp
var point = new Point2D(5, -2);
Console.WriteLine(point); // Output: (5, -2) or [5; -2] depending on culture
```

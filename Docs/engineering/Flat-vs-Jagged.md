# Flat vs Jagged

## Jagged

### Pros

- fast and simple indexing
- fast and simple copying/moving (faster for more major dimensions)

### Cons

- no Span<T> interop
- no easy access to the underlying memory
- GC pressure

## Flat

### Pros

- no GC pressure
- easy access to the underlying memory
- Span<T> interop

### Cons

- slower indexing (see [Optimizations](#indexing))
- slower copying/moving (same speed for all dimensions)

### Optimizations

#### Indexing

```c#
public ref T this[int x, int y] {
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    get {
        ref T space = ref MemoryMarshal.GetArrayDataReference(_items);
        var offset = (IntPtr)((nint)x * (nint)_xSize + y);
        return ref Unsafe.Add(ref space, offset);
    }
}
```

#### Resizing

```c#
public void Resize(int newX, int newY) {
    T[] newItems = ArrayPool<T>.Shared.Rent(newX * newY);

    int yToCopy = Math.Min(_ySize, newY);
    int xToCopy = Math.Min(_xSize, newX);

    for (int r = 0; r < yToCopy; r++) {
        var oldA = _items  .AsSpan(r * _xSize, xToCopy);
        var newA = newItems.AsSpan(r * newX,   xToCopy);
        oldA.CopyTo(newA);
    }

    ArrayPool<T>.Shared.Return(_items);
    _items = newItems;
    _xSize = newX;
    _ySize = newY;
}
```
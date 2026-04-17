// Metriks
// Copyright (c) KryKom & ZlomenyMesic 2025

using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Metriks;

public class Space2D<T> : List2D<T>, IEnumerable2D {
    
    private int _xOriginOffset = 0;
    private int _yOriginOffset = 0;
    
    public int XStart => -_xOriginOffset;
    public int YStart => -_yOriginOffset;
    public int XEnd => XSize - 1 - _xOriginOffset;
    public int YEnd => YSize - 1 - _yOriginOffset;
    
    public new T this[int x, int y] {
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => base[x + _xOriginOffset, y + _yOriginOffset];
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => base[x + _xOriginOffset, y + _yOriginOffset] = value;
    }

    public T this[int x, int y, bool disableCoordinates] {
        
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => disableCoordinates ? base[x, y] : this[x, y];
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set {
            if (disableCoordinates)
                base[x, y] = value;
            else
                this[x, y] = value;
        }
    }

    #if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
    
    public new T this[Index x, Index y] {
        
        [Pure]
        get {
            int xo = x.GetOffset(XSize) + (x.IsFromEnd ? 0 : _xOriginOffset);
            int yo = y.GetOffset(YSize) + (y.IsFromEnd ? 0 : _yOriginOffset);
            if (xo < 0 || xo >= XSize) throw new IndexOutOfRangeException("Index 'x' is out of range.");
            if (yo < 0 || yo >= YSize) throw new IndexOutOfRangeException("Index 'y' is out of range.");
            return UnsafeGet(xo, yo);
        }
        
        set {
            int xo = x.GetOffset(XSize) + (x.IsFromEnd ? 0 : _xOriginOffset);
            int yo = y.GetOffset(YSize) + (y.IsFromEnd ? 0 : _yOriginOffset);
            if (xo < 0 || xo >= XSize) throw new IndexOutOfRangeException("Index 'x' is out of range.");
            if (yo < 0 || yo >= YSize) throw new IndexOutOfRangeException("Index 'y' is out of range.");
            UnsafeSet(xo, yo, value);
        }
    }
    
    #endif

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T UncoordinatedGet(int x, int y) => base[x, y];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void UncoordinatedSet(int x, int y, T value) => base[x, y] = value;

    public int XOriginOffset {
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _xOriginOffset;
    }

    public int YOriginOffset {
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _yOriginOffset;
    }
    
    public Point2D OriginOffset {
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => new(_xOriginOffset, _yOriginOffset);
    }

    public Space2D(int xCapacity, int yCapacity) : base(xCapacity, yCapacity) { }
    public Space2D(int capacity) : base(capacity) { }
    public Space2D(T[,] arr) : base(arr) { }
    public Space2D() { }

    public new void InsertAtX(int x) {
        base.InsertAtX(Math.Max(0, x + _xOriginOffset));
        if (x <= _xOriginOffset) _xOriginOffset++;
    }

    public new void InsertAtY(int y) {
        base.InsertAtY(Math.Max(0, y + _yOriginOffset));
        if (y <= _yOriginOffset) _yOriginOffset++;
    }

    public new void RemoveAtX(int x) {
        base.RemoveAtX(x + _xOriginOffset);
        if (x < _xOriginOffset) _xOriginOffset--;
    }

    public new void RemoveAtY(int y) {
        base.RemoveAtY(y + _yOriginOffset);
        if (y < _yOriginOffset) _yOriginOffset--;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public new IEnumerable<T> GetAtX(int x) => base.GetAtX(x + _xOriginOffset);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public new IEnumerable<T> GetAtY(int y) => base.GetAtY(y + _yOriginOffset);

    public void Place(T[,] matrix, Point2D? offsetPoint = null) {
        var offset = offsetPoint ?? Point2D.Zero;
        base.Place(matrix, new Point2D(offset.X + _xOriginOffset, offset.Y + _yOriginOffset));
        if (offset.X < -_xOriginOffset) _xOriginOffset = -offset.X;
        if (offset.Y < -_yOriginOffset) _yOriginOffset = -offset.Y;
    }

    public new void Clear() {
        base.Clear();
        _xOriginOffset = 0;
        _yOriginOffset = 0;
    }

    public void MoveOrigin(int xOffset, int yOffset) {
        _xOriginOffset += xOffset;
        _yOriginOffset += yOffset;
    }

    IEnumerable IEnumerable2D.GetAtX(int x) => GetAtX(x);
    IEnumerable IEnumerable2D.GetAtY(int y) => GetAtY(y);

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    IEnumerator IEnumerable2D.GetEnumerator() => GetEnumerator();
}
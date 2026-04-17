// Metriks
// Copyright (c) KryKom & ZlomenyMesic 2025

using System.Runtime.CompilerServices;

namespace Metriks;

public class Space3D<T> : List3D<T> {
    
    private int _xOriginOffset = 0;
    private int _yOriginOffset = 0;
    private int _zOriginOffset = 0;
    
    public int XStart => -_xOriginOffset;
    public int YStart => -_yOriginOffset;
    public int ZStart => -_zOriginOffset;
    public int XEnd => XSize - 1 - _xOriginOffset;
    public int YEnd => YSize - 1 - _yOriginOffset;
    public int ZEnd => ZSize - 1 - _zOriginOffset;
    
    public new T this[int x, int y, int z] {
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => base[x + _xOriginOffset, y + _yOriginOffset, z + _zOriginOffset];
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => base[x + _xOriginOffset, y + _yOriginOffset, z + _zOriginOffset] = value;
    }

    public T this[int x, int y, int z, bool disableCoordinates] {
        
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => disableCoordinates ? base[x, y, z] : this[x, y, z];
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set {
            if (disableCoordinates)
                base[x, y, z] = value;
            else
                this[x, y, z] = value;
        }
    }

    #if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
    
    public new T this[Index x, Index y, Index z] {
        
        [Pure]
        get {
            int xo = x.GetOffset(XSize) + (x.IsFromEnd ? 0 : _xOriginOffset);
            int yo = y.GetOffset(YSize) + (y.IsFromEnd ? 0 : _yOriginOffset);
            int zo = z.GetOffset(ZSize) + (z.IsFromEnd ? 0 : _zOriginOffset);
            if (xo < 0 || xo >= XSize) throw new IndexOutOfRangeException("Index 'x' is out of range.");
            if (yo < 0 || yo >= YSize) throw new IndexOutOfRangeException("Index 'y' is out of range.");
            if (zo < 0 || zo >= ZSize) throw new IndexOutOfRangeException("Index 'z' is out of range.");
            return UnsafeGet(xo, yo, zo);
        }
        
        set {
            int xo = x.GetOffset(XSize) + (x.IsFromEnd ? 0 : _xOriginOffset);
            int yo = y.GetOffset(YSize) + (y.IsFromEnd ? 0 : _yOriginOffset);
            int zo = z.GetOffset(ZSize) + (z.IsFromEnd ? 0 : _zOriginOffset);
            if (xo < 0 || xo >= XSize) throw new IndexOutOfRangeException("Index 'x' is out of range.");
            if (yo < 0 || yo >= YSize) throw new IndexOutOfRangeException("Index 'y' is out of range.");
            if (zo < 0 || zo >= ZSize) throw new IndexOutOfRangeException("Index 'z' is out of range.");
            UnsafeSet(xo, yo, zo, value);
        }
    }
    
    #endif

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T UncoordinatedGet(int x, int y, int z) => base[x, y, z];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void UncoordinatedSet(int x, int y, int z, T value) => base[x, y, z] = value;

    public int XOriginOffset => _xOriginOffset;
    public int YOriginOffset => _yOriginOffset;
    public int ZOriginOffset => _zOriginOffset;
    
    public Point3D OriginOffset => new(_xOriginOffset, _yOriginOffset, _zOriginOffset);

    public Space3D(int xCapacity, int yCapacity, int zCapacity) : base(xCapacity, yCapacity, zCapacity) { }
    public Space3D(T[,,] arr) : base(arr) { }
    public Space3D() { }

    public new void InsertAtX(int x) {
        base.InsertAtX(Math.Max(0, x + _xOriginOffset));
        if (x <= _xOriginOffset) _xOriginOffset++;
    }

    public new void InsertAtY(int y) {
        base.InsertAtY(Math.Max(0, y + _yOriginOffset));
        if (y <= _yOriginOffset) _yOriginOffset++;
    }

    public new void InsertAtZ(int z) {
        base.InsertAtZ(Math.Max(0, z + _zOriginOffset));
        if (z <= _zOriginOffset) _zOriginOffset++;
    }

    public new void RemoveAtX(int x) {
        base.RemoveAtX(x + _xOriginOffset);
        if (x < _xOriginOffset) _xOriginOffset--;
    }

    public new void RemoveAtY(int y) {
        base.RemoveAtY(y + _yOriginOffset);
        if (y < _yOriginOffset) _yOriginOffset--;
    }

    public new void RemoveAtZ(int z) {
        base.RemoveAtZ(z + _zOriginOffset);
        if (z < _zOriginOffset) _zOriginOffset--;
    }

    public void Place(T[,,] matrix, Point3D? offsetPoint = null) {
        var offset = offsetPoint ?? Point3D.Zero;
        base.Place(matrix, new Point3D(offset.X + _xOriginOffset, offset.Y + _yOriginOffset, offset.Z + _zOriginOffset));
        if (offset.X < -_xOriginOffset) _xOriginOffset = -offset.X;
        if (offset.Y < -_yOriginOffset) _yOriginOffset = -offset.Y;
        if (offset.Z < -_zOriginOffset) _zOriginOffset = -offset.Z;
    }

    public new void Clear() {
        base.Clear();
        _xOriginOffset = 0;
        _yOriginOffset = 0;
        _zOriginOffset = 0;
    }

    public void MoveOrigin(int xOffset, int yOffset, int zOffset) {
        _xOriginOffset += xOffset;
        _yOriginOffset += yOffset;
        _zOriginOffset += zOffset;
    }
}
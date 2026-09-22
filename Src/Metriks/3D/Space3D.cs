// Metriks
// Copyright (c) KryKom & ZlomenyMesic 2026

using System.Runtime.CompilerServices;

namespace Metriks;

public class Space3D<T> : List3D<T> {

    public Space3D(int   xCapacity, int yCapacity, int zCapacity) : base(xCapacity, yCapacity, zCapacity) { }
    public Space3D(T[,,] arr) : base(arr) { }
    public Space3D() { }
    public Space3D(List3D<T> other) : base(other) { }
    public Space3D(IReadOnlyList3D<T> other) : base(other) { }
    public Space3D(Space3D<T> other) : base(other) {
        ArgumentNullException.ThrowIfNull(other);
        XOriginOffset = other.XOriginOffset;
        YOriginOffset = other.YOriginOffset;
        ZOriginOffset = other.ZOriginOffset;
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public new Space3D<T> Clone() => new(this);

    public int XStart => -XOriginOffset;
    public int YStart => -YOriginOffset;
    public int ZStart => -ZOriginOffset;
    public int XEnd   => XSize - 1 - XOriginOffset;
    public int YEnd   => YSize - 1 - YOriginOffset;
    public int ZEnd   => ZSize - 1 - ZOriginOffset;

    public new T this[int x, int y, int z] {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => base[x + XOriginOffset, y + YOriginOffset, z + ZOriginOffset];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => base[x + XOriginOffset, y + YOriginOffset, z + ZOriginOffset] = value;
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
            var xo = x.GetOffset(XSize) + (x.IsFromEnd ? 0 : XOriginOffset);
            var yo = y.GetOffset(YSize) + (y.IsFromEnd ? 0 : YOriginOffset);
            var zo = z.GetOffset(ZSize) + (z.IsFromEnd ? 0 : ZOriginOffset);

            if (xo < 0 || xo >= XSize)
                throw new IndexOutOfRangeException("Index 'x' is out of range.");

            if (yo < 0 || yo >= YSize)
                throw new IndexOutOfRangeException("Index 'y' is out of range.");

            if (zo < 0 || zo >= ZSize)
                throw new IndexOutOfRangeException("Index 'z' is out of range.");

            return UnsafeGet(xo, yo, zo);
        }

        set {
            var xo = x.GetOffset(XSize) + (x.IsFromEnd ? 0 : XOriginOffset);
            var yo = y.GetOffset(YSize) + (y.IsFromEnd ? 0 : YOriginOffset);
            var zo = z.GetOffset(ZSize) + (z.IsFromEnd ? 0 : ZOriginOffset);

            if (xo < 0 || xo >= XSize)
                throw new IndexOutOfRangeException("Index 'x' is out of range.");

            if (yo < 0 || yo >= YSize)
                throw new IndexOutOfRangeException("Index 'y' is out of range.");

            if (zo < 0 || zo >= ZSize)
                throw new IndexOutOfRangeException("Index 'z' is out of range.");

            UnsafeSet(xo, yo, zo, value);
        }
    }

    #endif

    public int XOriginOffset { get; private set; } = 0;

    public int YOriginOffset { get; private set; } = 0;

    public int ZOriginOffset { get; private set; } = 0;

    public Point3D OriginOffset => new(XOriginOffset, YOriginOffset, ZOriginOffset);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T UncoordinatedGet(int x, int y, int z) => base[x, y, z];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void UncoordinatedSet(int x, int y, int z, T value) => base[x, y, z] = value;

    public new void InsertAtX(int x) {
        base.InsertAtX(Math.Max(0, x + XOriginOffset));

        if (x <= XOriginOffset)
            XOriginOffset++;
    }

    public new void InsertAtY(int y) {
        base.InsertAtY(Math.Max(0, y + YOriginOffset));

        if (y <= YOriginOffset)
            YOriginOffset++;
    }

    public new void InsertAtZ(int z) {
        base.InsertAtZ(Math.Max(0, z + ZOriginOffset));

        if (z <= ZOriginOffset)
            ZOriginOffset++;
    }

    public new void RemoveAtX(int x) {
        base.RemoveAtX(x + XOriginOffset);

        if (x < XOriginOffset)
            XOriginOffset--;
    }

    public new void RemoveAtY(int y) {
        base.RemoveAtY(y + YOriginOffset);

        if (y < YOriginOffset)
            YOriginOffset--;
    }

    public new void RemoveAtZ(int z) {
        base.RemoveAtZ(z + ZOriginOffset);

        if (z < ZOriginOffset)
            ZOriginOffset--;
    }

    public void Place(T[,,] matrix, Point3D? offsetPoint = null) {
        var offset = offsetPoint ?? Point3D.Zero;
        base.Place(matrix, new Point3D(offset.X + XOriginOffset, offset.Y + YOriginOffset, offset.Z + ZOriginOffset));

        if (offset.X < -XOriginOffset)
            XOriginOffset = -offset.X;

        if (offset.Y < -YOriginOffset)
            YOriginOffset = -offset.Y;

        if (offset.Z < -ZOriginOffset)
            ZOriginOffset = -offset.Z;
    }

    public new void Clear() {
        base.Clear();
        XOriginOffset = 0;
        YOriginOffset = 0;
        ZOriginOffset = 0;
    }

    public void MoveOrigin(int xOffset, int yOffset, int zOffset) {
        XOriginOffset += xOffset;
        YOriginOffset += yOffset;
        ZOriginOffset += zOffset;
    }
}
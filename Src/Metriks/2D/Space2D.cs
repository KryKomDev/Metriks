// Metriks
// Copyright (c) KryKom & ZlomenyMesic 2026

using System.Collections;
using System.Runtime.CompilerServices;

namespace Metriks;

public class Space2D<T> : List2D<T>, IEnumerable2D {

    public Space2D(int  xCapacity, int yCapacity) : base(xCapacity, yCapacity) { }
    public Space2D(int  capacity) : base(capacity) { }
    public Space2D(T[,] arr) : base(arr) { }
    public Space2D() { }
    public Space2D(List2D<T> other) : base(other) { }
    public Space2D(IReadOnlyList2D<T> other) : base(other) { }
    public Space2D(Space2D<T> other) : base(other) {
        ArgumentNullException.ThrowIfNull(other);
        XOriginOffset = other.XOriginOffset;
        YOriginOffset = other.YOriginOffset;
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public new Space2D<T> Clone() => new(this);

    public int XStart => -XOriginOffset;
    public int YStart => -YOriginOffset;
    public int XEnd   => XSize - 1 - XOriginOffset;
    public int YEnd   => YSize - 1 - YOriginOffset;

    public new T this[int x, int y] {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => base[x + XOriginOffset, y + YOriginOffset];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => base[x + XOriginOffset, y + YOriginOffset] = value;
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
            var xo = x.GetOffset(XSize) + (x.IsFromEnd ? 0 : XOriginOffset);
            var yo = y.GetOffset(YSize) + (y.IsFromEnd ? 0 : YOriginOffset);

            if (xo < 0 || xo >= XSize)
                throw new IndexOutOfRangeException("Index 'x' is out of range.");

            if (yo < 0 || yo >= YSize)
                throw new IndexOutOfRangeException("Index 'y' is out of range.");

            return UnsafeGet(xo, yo);
        }

        set {
            var xo = x.GetOffset(XSize) + (x.IsFromEnd ? 0 : XOriginOffset);
            var yo = y.GetOffset(YSize) + (y.IsFromEnd ? 0 : YOriginOffset);

            if (xo < 0 || xo >= XSize)
                throw new IndexOutOfRangeException("Index 'x' is out of range.");

            if (yo < 0 || yo >= YSize)
                throw new IndexOutOfRangeException("Index 'y' is out of range.");

            UnsafeSet(xo, yo, value);
        }
    }

    #endif

    public int XOriginOffset {
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get;
        private set;
    } = 0;

    public int YOriginOffset {
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get;
        private set;
    } = 0;

    public Point2D OriginOffset {
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => new(XOriginOffset, YOriginOffset);
    }

    IEnumerable IEnumerable2D.GetAtX(int x) => GetAtX(x);
    IEnumerable IEnumerable2D.GetAtY(int y) => GetAtY(y);

    IEnumerator IEnumerable.  GetEnumerator() => GetEnumerator();
    IEnumerator IEnumerable2D.GetEnumerator() => GetEnumerator();

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T UncoordinatedGet(int x, int y) => base[x, y];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void UncoordinatedSet(int x, int y, T value) => base[x, y] = value;

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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public new IEnumerable<T> GetAtX(int x) => base.GetAtX(x + XOriginOffset);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public new IEnumerable<T> GetAtY(int y) => base.GetAtY(y + YOriginOffset);

    public void Place(T[,] matrix, Point2D? offsetPoint = null) {
        var offset = offsetPoint ?? Point2D.Zero;
        base.Place(matrix, new Point2D(offset.X + XOriginOffset, offset.Y + YOriginOffset));

        if (offset.X < -XOriginOffset)
            XOriginOffset = -offset.X;

        if (offset.Y < -YOriginOffset)
            YOriginOffset = -offset.Y;
    }

    public new void Clear() {
        base.Clear();
        XOriginOffset = 0;
        YOriginOffset = 0;
    }

    public void MoveOrigin(int xOffset, int yOffset) {
        XOriginOffset += xOffset;
        YOriginOffset += yOffset;
    }
}
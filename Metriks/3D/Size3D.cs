// Metriks
// Copyright (c) KryKom & ZlomenyMesic 2025

namespace Metriks;

public readonly struct Size3D {
    public int X { get; }
    public int Y { get; }
    public int Z { get; }
    
    public Size3D(int x, int y, int z) {
        X = x;
        Y = y;
        Z = z;
    }
}
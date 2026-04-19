namespace Metriks.Tests;

public class Array3DTests {
    [Fact]
    public void Copy_FullArray_CopiesAllElements() {
        // Arrange
        int[,,] source = new int[2, 2, 2];
        int val = 0;
        for (int x = 0; x < 2; x++)
            for (int y = 0; y < 2; y++)
                for (int z = 0; z < 2; z++)
                    source[x, y, z] = ++val;

        int[,,] destination = new int[2, 2, 2];

        // Act
        Array3D.Copy(source, 0, 0, 0, destination, 0, 0, 0, 2, 2, 2);

        // Assert
        Assert.Equal(source, destination);
    }

    [Fact]
    public void Copy_SubRegion_CopiesCorrectElements() {
        // Arrange
        int[,,] source = new int[3, 3, 3];
        for (int x = 0; x < 3; x++)
            for (int y = 0; y < 3; y++)
                for (int z = 0; z < 3; z++)
                    source[x, y, z] = x * 100 + y * 10 + z;

        int[,,] destination = new int[2, 2, 2];

        // Act
        // Copy 2x2x2 from (1,1,1)
        Array3D.Copy(source, 1, 1, 1, destination, 0, 0, 0, 2, 2, 2);

        // Assert
        int[,,] expected = new int[2, 2, 2];
        for (int x = 0; x < 2; x++)
            for (int y = 0; y < 2; y++)
                for (int z = 0; z < 2; z++)
                    expected[x, y, z] = (x + 1) * 100 + (y + 1) * 10 + (z + 1);

        Assert.Equal(expected, destination);
    }

    [Fact]
    public void Copy_PointAndSizeOverload_CopiesCorrectElements() {
        // Arrange
        int[,,] source = new int[3, 3, 3];
        source[1, 1, 1] = 42;
        int[,,] destination = new int[1, 1, 1];

        // Act
        Array3D.Copy(
            source, new Point3D(1, 1, 1),
            destination, new Point3D(0, 0, 0),
            new Size3D(1, 1, 1)
        );

        // Assert
        Assert.Equal(42, destination[0, 0, 0]);
    }

    [Fact]
    public void Fill_SubRegion_FillsCorrectElements() {
        // Arrange
        int[,,] array = new int[3, 3, 3];

        // Act
        Array3D.Fill(array, 42, 1, 1, 1, 1, 1, 1);

        // Assert
        Assert.Equal(42, array[1, 1, 1]);
        Assert.Equal(0, array[0, 0, 0]);
        Assert.Equal(0, array[2, 2, 2]);
    }

    [Fact]
    public void Fill_AreaOverload_FillsCorrectElements() {
        // Arrange
        int[,,] array = new int[3, 3, 3];
        Area3D area = new Area3D(new Point3D(1, 1, 1), new Size3D(1, 1, 1));

        // Act
        Array3D.Fill(array, 42, area);

        // Assert
        // Area3D(lower, size) => lower to lower + size
        // Array2D.Fill(array, item, area) calls Fill(array, item, area.Lower, area.Size + Size2D.One)
        // Wait, let me check Area2D.Fill implementation in Array2D.cs again.
        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        // public static void Fill<T>(T[,] array, T item, Area2D area) =>
        //     Fill(array, item, area.Lower, area.Size + Size2D.One);
        // If Area3D(lower, size) has Lower=(1,1,1) and Size=(1,1,1), then area.Size + One is (2,2,2).
        // So it fills 2x2x2 region from (1,1,1) to (2,2,2) inclusive?
        // Let's check Area3D definition.
        // Area3D(Point3D lower, Size3D s) { (_lx, _ly, _lz) = lower; _hx = lower.X + s.X; ... }
        // Size => Math.Abs(_lx - _hx) => s.X.
        // So Area3D.Size IS the size passed in constructor.
        // If Array2D.Fill(array, item, area) uses area.Size + One, it means it includes the 'Higher' point.
        // My implementation in Array3D.cs:
        // public static void Fill<T>(T[,,] array, T item, Area3D area) => Fill(array, item, area.Lower, area.Size + Size3D.One);
        // So for Area3D((1,1,1), (1,1,1)), size to fill is (2,2,2).
        // Elements filled: (1,1,1), (1,1,2), (1,2,1), (1,2,2), (2,1,1), (2,1,2), (2,2,1), (2,2,2).
        
        Assert.Equal(42, array[1, 1, 1]);
        Assert.Equal(42, array[2, 2, 2]);
        Assert.Equal(0, array[0, 0, 0]);
    }

    [Fact]
    public void Fill_Factory_FillsCorrectElements() {
        // Arrange
        int[,,] array = new int[2, 2, 2];
        int val = 0;

        // Act
        Array3D.Fill(array, () => ++val, 0, 0, 0, 2, 2, 2);

        // Assert
        // The factory is called once for each ROW (innermost dimension), because that's how it is implemented in Array2D.
        // Wait, let's check Array2D.Fill(array, factory, ...)
        // for (int i = 0; i < yCount; i++) { ... rowSpan.Fill(itemFactory()); }
        // Yes, it's called once per row.
        // In Array3D, it's called for each (x, y) pair.
        // So for 2x2x2, it should be called 4 times.
        Assert.Equal(1, array[0, 0, 0]);
        Assert.Equal(1, array[0, 0, 1]);
        Assert.Equal(2, array[0, 1, 0]);
        Assert.Equal(4, array[1, 1, 1]);
    }

    [Fact]
    public void Clear_SubRegion_ClearsCorrectElements() {
        // Arrange
        int[,,] array = new int[3, 3, 3];
        Array3D.Fill(array, 42, 0, 0, 0, 3, 3, 3);

        // Act
        Array3D.Clear(array, 1, 1, 1, 1, 1, 1);

        // Assert
        Assert.Equal(0, array[1, 1, 1]);
        Assert.Equal(42, array[0, 0, 0]);
        Assert.Equal(42, array[2, 2, 2]);
    }

    [Fact]
    public void Copy_NullSource_ThrowsNullReferenceException() {
        // Arrange
        int[,,] source = null!;
        int[,,] destination = new int[1, 1, 1];

        // Act & Assert
        Assert.Throws<NullReferenceException>(() => Array3D.Copy(source, 0, 0, 0, destination, 0, 0, 0, 1, 1, 1));
    }

    [Fact]
    public void Copy_ZeroCount_DoesNothing() {
        // Arrange
        int[,,] source = { { { 1 } } };
        int[,,] destination = { { { 0 } } };

        // Act
        Array3D.Copy(source, 0, 0, 0, destination, 0, 0, 0, 0, 1, 1);

        // Assert
        Assert.Equal(0, destination[0, 0, 0]);
    }

    [Fact]
    public void Fill_NegativeCount_DoesNothing() {
        // Arrange
        int[,,] array = new int[1, 1, 1];

        // Act
        Array3D.Fill(array, 42, 0, 0, 0, -1, 1, 1);

        // Assert
        Assert.Equal(0, array[0, 0, 0]);
    }
}

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
}

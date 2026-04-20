namespace Metriks.Tests;

public class Array4DTests {
    [Fact]
    public void Copy_FullArray_CopiesAllElements() {
        // Arrange
        int[,,,] source = new int[2, 2, 2, 2];
        int val = 0;
        for (int w = 0; w < 2; w++)
            for (int x = 0; x < 2; x++)
                for (int y = 0; y < 2; y++)
                    for (int z = 0; z < 2; z++)
                        source[w, x, y, z] = ++val;

        int[,,,] destination = new int[2, 2, 2, 2];

        // Act
        Array4D.Copy(source, 0, 0, 0, 0, destination, 0, 0, 0, 0, 2, 2, 2, 2);

        // Assert
        Assert.Equal(source, destination);
    }

    [Fact]
    public void Copy_SubRegion_CopiesCorrectElements() {
        // Arrange
        int[,,,] source = new int[3, 3, 3, 3];
        source[1, 1, 1, 1] = 1234;

        int[,,,] destination = new int[1, 1, 1, 1];

        // Act
        Array4D.Copy(source, 1, 1, 1, 1, destination, 0, 0, 0, 0, 1, 1, 1, 1);

        // Assert
        Assert.Equal(1234, destination[0, 0, 0, 0]);
    }

    [Fact]
    public void Copy_PointAndSizeOverload_CopiesCorrectElements() {
        // Arrange
        int[,,,] source = new int[3, 3, 3, 3];
        source[1, 1, 1, 1] = 42;
        int[,,,] destination = new int[1, 1, 1, 1];

        // Act
        Array4D.Copy(
            source, new Point4D(1, 1, 1, 1),
            destination, new Point4D(0, 0, 0, 0),
            new Size4D(1, 1, 1, 1)
        );

        // Assert
        Assert.Equal(42, destination[0, 0, 0, 0]);
    }

    [Fact]
    public void Copy_Shorthand_CopiesCorrectElements() {
        // Arrange
        int[,,,] source = new int[2, 2, 2, 2];
        source[0, 0, 0, 0] = 42;
        int[,,,] destination = new int[3, 3, 3, 3];

        // Act
        Array4D.Copy(source, destination, new Point4D(1, 1, 1, 1));

        // Assert
        Assert.Equal(42, destination[1, 1, 1, 1]);
    }

    [Fact]
    public void Fill_SubRegion_FillsCorrectElements() {
        // Arrange
        int[,,,] array = new int[3, 3, 3, 3];

        // Act
        Array4D.Fill(array, 42, 1, 1, 1, 1, 1, 1, 1, 1);

        // Assert
        Assert.Equal(42, array[1, 1, 1, 1]);
        Assert.Equal(0, array[0, 0, 0, 0]);
    }

    [Fact]
    public void Clear_SubRegion_ClearsCorrectElements() {
        // Arrange
        int[,,,] array = new int[3, 3, 3, 3];
        Array4D.Fill(array, 42, 0, 0, 0, 0, 3, 3, 3, 3);

        // Act
        Array4D.Clear(array, 1, 1, 1, 1, 1, 1, 1, 1);

        // Assert
        Assert.Equal(0, array[1, 1, 1, 1]);
        Assert.Equal(42, array[0, 0, 0, 0]);
    }

    [Fact]
    public void Copy_NullSource_ThrowsNullReferenceException() {
        // Arrange
        int[,,,] source = null!;
        int[,,,] destination = new int[1, 1, 1, 1];

        // Act & Assert
        Assert.Throws<NullReferenceException>(() => Array4D.Copy(source, 0, 0, 0, 0, destination, 0, 0, 0, 0, 1, 1, 1, 1));
    }

    [Fact]
    public void Fill_ZeroCount_DoesNothing() {
        // Arrange
        int[,,,] array = new int[1, 1, 1, 1];

        // Act
        Array4D.Fill(array, 42, 0, 0, 0, 0, 0, 1, 1, 1);

        // Assert
        Assert.Equal(0, array[0, 0, 0, 0]);
    }
}

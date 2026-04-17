namespace Metriks.Tests;

public class Array2DTests {
    [Fact]
    public void Copy_FullArray_CopiesAllElements() {
        // Arrange
        int[,] source = {
            { 1, 2, 3 },
            { 4, 5, 6 }
        };
        int[,] destination = new int[2, 3];

        // Act
        Array2D.Copy(source, 0, 0, destination, 0, 0, 3, 2);

        // Assert
        Assert.Equal(source, destination);
    }

    [Fact]
    public void Copy_SubRegion_CopiesCorrectElements() {
        // Arrange
        int[,] source = {
            { 1, 2, 3 },
            { 4, 5, 6 },
            { 7, 8, 9 }
        };
        int[,] destination = new int[2, 2];

        // Act
        // Copy 2x2 from top-left
        Array2D.Copy(source, 0, 0, destination, 0, 0, 2, 2);

        // Assert
        int[,] expected = {
            { 1, 2 },
            { 4, 5 }
        };
        Assert.Equal(expected, destination);
    }

    [Fact]
    public void Copy_WithOffsets_CopiesCorrectElements() {
        // Arrange
        int[,] source = {
            { 1, 2, 3 },
            { 4, 5, 6 },
            { 7, 8, 9 }
        };
        int[,] destination = new int[3, 3];

        // Act
        // Copy 2x2 from (1,1) in source to (1,1) in destination
        // source[1,1] is 5, region is {{5,6},{8,9}}
        Array2D.Copy(source, 1, 1, destination, 1, 1, 2, 2);

        // Assert
        int[,] expected = {
            { 0, 0, 0 },
            { 0, 5, 6 },
            { 0, 8, 9 }
        };
        Assert.Equal(expected, destination);
    }

    [Fact]
    public void Copy_EmptyRegion_DoesNothing() {
        // Arrange
        int[,] source = { { 1 } };
        int[,] destination = { { 0 } };

        // Act
        Array2D.Copy(source, 0, 0, destination, 0, 0, 0, 1);
        Array2D.Copy(source, 0, 0, destination, 0, 0, 1, 0);

        // Assert
        Assert.Equal(0, destination[0, 0]);
    }

    [Fact]
    public void Copy_PointAndSizeOverload_CopiesCorrectElements() {
        // Arrange
        int[,] source = {
            { 1, 2, 3 },
            { 4, 5, 6 },
            { 7, 8, 9 }
        };
        int[,] destination = new int[3, 3];

        // Act
        Array2D.Copy(
            source, new Point2D(1, 1),
            destination, new Point2D(0, 0),
            new Size2D(2, 2)
        );

        // Assert
        int[,] expected = {
            { 5, 6, 0 },
            { 8, 9, 0 },
            { 0, 0, 0 }
        };
        Assert.Equal(expected, destination);
    }

    [Fact]
    public void Copy_LargeArray_CopiesCorrectElements() {
        // Arrange
        int rows = 100;
        int cols = 100;
        int[,] source = new int[rows, cols];
        for (int r = 0; r < rows; r++) {
            for (int c = 0; c < cols; c++) {
                source[r, c] = r * cols + c;
            }
        }
        int[,] destination = new int[rows, cols];

        // Act
        Array2D.Copy(source, 0, 0, destination, 0, 0, cols, rows);

        // Assert
        Assert.Equal(source, destination);
    }

    [Fact]
    public void Copy_NullSource_ThrowsNullReferenceException() {
        // Arrange
        int[,] source = null!;
        int[,] destination = new int[1, 1];

        // Act & Assert
        Assert.Throws<NullReferenceException>(() => Array2D.Copy(source, 0, 0, destination, 0, 0, 1, 1));
    }

    [Fact]
    public void Copy_EmptyArrayWithPositiveCount_ThrowsIndexOutOfRangeException() {
        // Arrange
        int[,] source = new int[0, 0];
        int[,] destination = new int[1, 1];

        // Act & Assert
        Assert.Throws<IndexOutOfRangeException>(() => Array2D.Copy(source, 0, 0, destination, 0, 0, 1, 1));
    }
}

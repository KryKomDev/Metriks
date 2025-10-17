using System.Drawing;

namespace Metriks.Test;

public class List2DTests {
    
    [Fact]
    public void Constructor_DefaultCapacity_ShouldInitializeCorrectly() {
        // Arrange & Act
        var list2D = new List2D<int>();

        // Assert
        Assert.Equal(0, list2D.XSize);
        Assert.Equal(0, list2D.YSize);
        Assert.Equal(4, list2D.XCapacity);
        Assert.Equal(4, list2D.YCapacity);
        Assert.Equal(new Size(0, 0), list2D.Size);
    }

    [Fact]
    public void Constructor_CustomCapacity_ShouldInitializeCorrectly() {
        // Arrange & Act
        var list2D = new List2D<int>(10, 15);

        // Assert
        Assert.Equal(0, list2D.XSize);
        Assert.Equal(0, list2D.YSize);
        Assert.Equal(10, list2D.XCapacity);
        Assert.Equal(15, list2D.YCapacity);
    }

    [Fact]
    public void Constructor_From2DArray_ShouldInitializeCorrectly() {
        // Arrange
        var array = new[,] { { 1, 2, 3 }, { 4, 5, 6 } };

        // Act
        var list2D = new List2D<int>(array);

        // Assert
        Assert.Equal(2, list2D.XSize);
        Assert.Equal(3, list2D.YSize);
        Assert.Equal(1, list2D[0, 0]);
        Assert.Equal(2, list2D[0, 1]);
        Assert.Equal(3, list2D[0, 2]);
        Assert.Equal(4, list2D[1, 0]);
        Assert.Equal(5, list2D[1, 1]);
        Assert.Equal(6, list2D[1, 2]);
    }

    [Fact]
    public void Indexer_ValidIndices_ShouldSetAndGetCorrectly() {
        // Arrange
        var list2D = new List2D<int>();
        list2D.AddX();
        list2D.AddY();

        // Act & Assert
        list2D[0, 0] = 42;
        Assert.Equal(42, list2D[0, 0]);
    }

    [Theory]
    [InlineData(-1, 0)]
    [InlineData(0, -1)]
    [InlineData(1, 0)]
    [InlineData(0, 1)]
    public void Indexer_InvalidIndices_ShouldThrowIndexOutOfRangeException(int x, int y) {
        // Arrange
        var list2D = new List2D<int>();
        list2D.AddX();
        list2D.AddY();

        // Act & Assert
        Assert.Throws<IndexOutOfRangeException>(() => list2D[x, y]);
        Assert.Throws<IndexOutOfRangeException>(() => list2D[x, y] = 1);
    }

    [Fact]
    public void AddX_ShouldIncreaseXSize() {
        // Arrange
        var list2D = new List2D<int>();

        // Act
        list2D.AddX();
        list2D.AddX();

        // Assert
        Assert.Equal(2, list2D.XSize);
        Assert.Equal(0, list2D.YSize);
    }

    [Fact] 
    public void AddY_ShouldIncreaseYSize() {
        // Arrange
        var list2D = new List2D<int>();

        // Act
        list2D.AddY();
        list2D.AddY();

        // Assert
        Assert.Equal(0, list2D.XSize);
        Assert.Equal(2, list2D.YSize);
    }

    [Fact]
    public void AddX_ExceedsCapacity_ShouldGrowCapacity() {
        // Arrange
        var list2D = new List2D<int>(2, 2);

        // Act
        list2D.AddX();
        list2D.AddX();
        list2D.AddX(); // This should trigger capacity growth

        // Assert
        Assert.Equal(3, list2D.XSize);
        Assert.True(list2D.XCapacity > 2);
    }

    [Fact]
    public void AddY_ExceedsCapacity_ShouldGrowCapacity() {
        // Arrange
        var list2D = new List2D<int>(2, 2);
        list2D.AddX(); // Need at least one X to add Y

        // Act
        list2D.AddY();
        list2D.AddY();
        list2D.AddY(); // This should trigger capacity growth

        // Assert
        Assert.Equal(3, list2D.YSize);
        Assert.True(list2D.YCapacity > 2);
    }

    [Fact]
    public void InsertXAt_ValidIndex_ShouldInsertAtCorrectPosition() {
        // Arrange
        var list2D = new List2D<int>();
        list2D.AddX();
        list2D.AddX();
        list2D.AddY();
        list2D[0, 0] = 1;
        list2D[1, 0] = 3;

        // Act
        list2D.InsertXAt(1);
        list2D[1, 0] = 2;

        // Assert
        Assert.Equal(3, list2D.XSize);
        Assert.Equal(1, list2D[0, 0]);
        Assert.Equal(2, list2D[1, 0]);
        Assert.Equal(3, list2D[2, 0]);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(3)]
    public void InsertXAt_InvalidIndex_ShouldThrowIndexOutOfRangeException(int index) {
        // Arrange
        var list2D = new List2D<int>();
        list2D.AddX();
        list2D.AddX();

        // Act & Assert
        Assert.Throws<IndexOutOfRangeException>(() => list2D.InsertXAt(index));
    }

    [Fact]
    public void InsertYAt_ValidIndex_ShouldInsertAtCorrectPosition() {
        // Arrange
        var list2D = new List2D<int>();
        list2D.AddX();
        list2D.AddY();
        list2D.AddY();
        list2D[0, 0] = 1;
        list2D[0, 1] = 3;

        // Act
        list2D.InsertYAt(1);
        list2D[0, 1] = 2;

        // Assert
        Assert.Equal(3, list2D.YSize);
        Assert.Equal(1, list2D[0, 0]);
        Assert.Equal(2, list2D[0, 1]);
        Assert.Equal(3, list2D[0, 2]);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(3)]
    public void InsertYAt_InvalidIndex_ShouldThrowIndexOutOfRangeException(int index) {
        // Arrange
        var list2D = new List2D<int>();
        list2D.AddX();
        list2D.AddY();
        list2D.AddY();

        // Act & Assert
        Assert.Throws<IndexOutOfRangeException>(() => list2D.InsertYAt(index));
    }

    [Fact]
    public void RemoveX_WithElements_ShouldRemoveLastColumn() {
        // Arrange
        var list2D = new List2D<int>();
        list2D.AddX();
        list2D.AddX();
        list2D.AddY();
        list2D[0, 0] = 1;
        list2D[1, 0] = 2;

        // Act
        list2D.RemoveX();

        // Assert
        Assert.Equal(1, list2D.XSize);
        Assert.Equal(1, list2D[0, 0]);
    }

    [Fact]
    public void RemoveX_EmptyList_ShouldThrowInvalidOperationException() {
        // Arrange
        var list2D = new List2D<int>();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => list2D.RemoveX());
    }

    [Fact]
    public void RemoveY_WithElements_ShouldRemoveLastRow() {
        // Arrange
        var list2D = new List2D<int>();
        list2D.AddX();
        list2D.AddY();
        list2D.AddY();
        list2D[0, 0] = 1;
        list2D[0, 1] = 2;

        // Act
        list2D.RemoveY();

        // Assert
        Assert.Equal(1, list2D.YSize);
        Assert.Equal(1, list2D[0, 0]);
    }

    [Fact]
    public void RemoveY_EmptyList_ShouldThrowInvalidOperationException() {
        // Arrange
        var list2D = new List2D<int>();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => list2D.RemoveY());
    }

    [Fact]
    public void RemoveXAt_ValidIndex_ShouldRemoveCorrectColumn() {
        // Arrange
        var list2D = new List2D<int>();
        list2D.AddX();
        list2D.AddX();
        list2D.AddX();
        list2D.AddY();
        list2D[0, 0] = 1;
        list2D[1, 0] = 2;
        list2D[2, 0] = 3;

        // Act
        list2D.RemoveXAt(1);

        // Assert
        Assert.Equal(2, list2D.XSize);
        Assert.Equal(1, list2D[0, 0]);
        Assert.Equal(3, list2D[1, 0]);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(2)]
    public void RemoveXAt_InvalidIndex_ShouldThrowIndexOutOfRangeException(int index) {
        // Arrange
        var list2D = new List2D<int>();
        list2D.AddX();
        list2D.AddX();

        // Act & Assert
        Assert.Throws<IndexOutOfRangeException>(() => list2D.RemoveXAt(index));
    }

    [Fact]
    public void RemoveYAt_ValidIndex_ShouldRemoveCorrectRow() {
        // Arrange
        var list2D = new List2D<int>();
        list2D.AddX();
        list2D.AddY();
        list2D.AddY();
        list2D.AddY();
        list2D[0, 0] = 1;
        list2D[0, 1] = 2;
        list2D[0, 2] = 3;

        // Act
        list2D.RemoveYAt(1);

        // Assert
        Assert.Equal(2, list2D.YSize);
        Assert.Equal(1, list2D[0, 0]);
        Assert.Equal(3, list2D[0, 1]);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(2)]
    public void RemoveYAt_InvalidIndex_ShouldThrowIndexOutOfRangeException(int index) {
        // Arrange
        var list2D = new List2D<int>();
        list2D.AddX();
        list2D.AddY();
        list2D.AddY();

        // Act & Assert
        Assert.Throws<IndexOutOfRangeException>(() => list2D.RemoveYAt(index));
    }

    [Fact]
    public void Expand_ValidSizes_ShouldExpandCorrectly() {
        // Arrange
        var list2D = new List2D<int>();
        list2D.AddX();
        list2D.AddY();

        // Act
        list2D.Expand(3, 3);

        // Assert
        Assert.Equal(3, list2D.XSize); // Expand adds 1 to the parameter
        Assert.Equal(3, list2D.YSize); // Expand adds 1 to the parameter
    }

    [Fact]
    public void Expand_SmallerThanCurrent_ShouldThrowArgumentOutOfRangeException() {
        // Arrange
        var list2D = new List2D<int>();
        list2D.AddX();
        list2D.AddX();
        list2D.AddY();

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => list2D.Expand(0, 2));
        Assert.Throws<ArgumentOutOfRangeException>(() => list2D.Expand(2, 0));
    }

    [Fact]
    public void AllAtX_AllElementsMatch_ShouldReturnTrue() {
        // Arrange
        var list2D = new List2D<int>();
        list2D.AddX();
        list2D.AddY();
        list2D.AddY();
        list2D.AddY();
        list2D[0, 0] = 2;
        list2D[0, 1] = 4;
        list2D[0, 2] = 6;

        // Act
        var result = list2D.AllAtX(0, x => x % 2 == 0);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void AllAtX_NotAllElementsMatch_ShouldReturnFalse() {
        // Arrange
        var list2D = new List2D<int>();
        list2D.AddX();
        list2D.AddY();
        list2D.AddY();
        list2D.AddY();
        list2D[0, 0] = 2;
        list2D[0, 1] = 3;
        list2D[0, 2] = 6;

        // Act
        var result = list2D.AllAtX(0, x => x % 2 == 0);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void AllAtY_AllElementsMatch_ShouldReturnTrue() {
        // Arrange
        var list2D = new List2D<int>();
        list2D.AddX();
        list2D.AddX();
        list2D.AddX();
        list2D.AddY();
        list2D[0, 0] = 2;
        list2D[1, 0] = 4;
        list2D[2, 0] = 6;

        // Act
        var result = list2D.AllAtY(0, x => x % 2 == 0);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void AllAtY_NotAllElementsMatch_ShouldReturnFalse() {
        // Arrange
        var list2D = new List2D<int>();
        list2D.AddX();
        list2D.AddX();
        list2D.AddX();
        list2D.AddY();
        list2D[0, 0] = 2;
        list2D[1, 0] = 3;
        list2D[2, 0] = 6;

        // Act
        var result = list2D.AllAtY(0, x => x % 2 == 0);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void GetAtX_ShouldReturnCorrectElements() {
        // Arrange
        var list2D = new List2D<int>();
        list2D.AddX();
        list2D.AddY();
        list2D.AddY();
        list2D.AddY();
        list2D[0, 0] = 1;
        list2D[0, 1] = 2;
        list2D[0, 2] = 3;

        // Act
        var result = list2D.GetAtX(0).ToList();

        // Assert
        Assert.Equal(new[] { 1, 2, 3 }, result);
    }

    [Fact]
    public void GetAtY_ShouldReturnCorrectElements() {
        // Arrange
        var list2D = new List2D<int>();
        list2D.AddX();
        list2D.AddX();
        list2D.AddX();
        list2D.AddY();
        list2D[0, 0] = 1;
        list2D[1, 0] = 2;
        list2D[2, 0] = 3;

        // Act
        var result = list2D.GetAtY(0).ToList();

        // Assert
        Assert.Equal(new[] { 1, 2, 3 }, result);
    }

    [Fact]
    public void Place_WithoutOffset_ShouldPlaceAtOrigin() {
        
        // Arrange
        var list2D = new List2D<int>();
        var matrix = new[,] { { 1, 2 }, { 3, 4 } };

        // Act
        list2D.Place(matrix);

        // Assert
        Assert.Equal(2, list2D.XSize);
        Assert.Equal(2, list2D.YSize);
        Assert.Equal(1, list2D[0, 0]);
        Assert.Equal(2, list2D[0, 1]);
        Assert.Equal(3, list2D[1, 0]);
        Assert.Equal(4, list2D[1, 1]);
    }

    [Fact]
    public void Place_WithPositiveOffset_ShouldPlaceAtOffset() {
        
        // Arrange
        var list2D = new List2D<int>();
        list2D.Expand(4, 4); // Make sure we have enough space
        var matrix = new[,] { { 1, 2 } };

        // Act
        list2D.Place(matrix, new Point(2, 1));

        // Assert
        Assert.Equal(1, list2D[2, 1]);
        Assert.Equal(2, list2D[2, 2]);
    }

    [Fact]
    public void GetEnumerator_ShouldEnumerateColumns() {
        
        // Arrange
        var list2D = new List2D<int>();
        list2D.AddX();
        list2D.AddX();
        list2D.AddY();
        list2D.AddY();
        list2D[0, 0] = 1;
        list2D[0, 1] = 2;
        list2D[1, 0] = 3;
        list2D[1, 1] = 4;

        // Act
        var columns = list2D.ToJagged();

        // Assert
        Assert.Equal(2, columns.Length);
        Assert.Equal(new[] { 1, 2 }, columns[0].ToArray());
        Assert.Equal(new[] { 3, 4 }, columns[1].ToArray());
    }

    [Fact]
    public void Size_Property_ShouldReturnCorrectSize() {
        
        // Arrange
        var list2D = new List2D<int>();
        list2D.AddX();
        list2D.AddX();
        list2D.AddY();
        list2D.AddY();
        list2D.AddY();

        // Act
        var size = list2D.Size;

        // Assert
        Assert.Equal(new Size(2, 3), size);
    }
}
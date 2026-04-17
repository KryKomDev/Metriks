namespace Metriks.Tests;

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
        Assert.Equal(new Size2D(0, 0), list2D.Size);
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
    public void Constructor_From2DArray_YLarger_ShouldInitializeCorrectly() {
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
    public void Constructor_From2DArray_XLarger_ShouldInitializeCorrectly() {
        // Arrange
        var array = new[,] { { 1, 2 }, { 3, 4 }, { 5, 6 } };

        // Act
        var list2D = new List2D<int>(array);

        // Assert
        Assert.Equal(3, list2D.XSize);
        Assert.Equal(2, list2D.YSize);
        Assert.Equal(1, list2D[0, 0]);
        Assert.Equal(2, list2D[0, 1]);
        Assert.Equal(3, list2D[1, 0]);
        Assert.Equal(4, list2D[1, 1]);
        Assert.Equal(5, list2D[2, 0]);
        Assert.Equal(6, list2D[2, 1]);
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
        list2D.InsertAtX(1);
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
        Assert.Throws<IndexOutOfRangeException>(() => list2D.InsertAtX(index));
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
        list2D.InsertAtY(1);
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
        Assert.Throws<IndexOutOfRangeException>(() => list2D.InsertAtY(index));
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
        list2D.ShrinkX();

        // Assert
        Assert.Equal(1, list2D.XSize);
        Assert.Equal(1, list2D[0, 0]);
    }

    [Fact]
    public void RemoveX_EmptyList_ShouldThrowInvalidOperationException() {
        // Arrange
        var list2D = new List2D<int>();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => list2D.ShrinkX());
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
        list2D.ShrinkY();

        // Assert
        Assert.Equal(1, list2D.YSize);
        Assert.Equal(1, list2D[0, 0]);
    }

    [Fact]
    public void RemoveY_EmptyList_ShouldThrowInvalidOperationException() {
        // Arrange
        var list2D = new List2D<int>();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => list2D.ShrinkY());
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
        list2D.RemoveAtX(1);

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
        Assert.Throws<IndexOutOfRangeException>(() => list2D.RemoveAtX(index));
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
        list2D.RemoveAtY(1);

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
        Assert.Throws<IndexOutOfRangeException>(() => list2D.RemoveAtY(index));
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
    public void Expand_Factory_ValidSizes_ShouldExpandCorrectly() {
        // Arrange
        var list2D = new List2D<int>();
        list2D.AddX();
        list2D.AddY();

        // Act
        list2D.Expand(3, 3, () => 1);

        // Assert
        Assert.Equal(3, list2D.XSize); // Expand adds 1 to the parameter
        Assert.Equal(3, list2D.YSize); // Expand adds 1 to the parameter
        Assert.Equal(new[,]{ { 0, 1, 1 }, { 1, 1, 1 }, { 1, 1, 1 } }, list2D.ToArray());
    }

    [Fact]
    public void Expand_Factory_SmallerThanCurrent_ShouldThrowArgumentOutOfRangeException() {
        // Arrange
        var list2D = new List2D<int>();
        list2D.AddX();
        list2D.AddX();
        list2D.AddY();

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => list2D.Expand(0, 2, () => 1));
        Assert.Throws<ArgumentOutOfRangeException>(() => list2D.Expand(2, 0, () => 1));
    }
    
    [Fact]
    public void Expand_Default_ValidSizes_ShouldExpandCorrectly() {
        // Arrange
        var list2D = new List2D<int>();
        list2D.AddX();
        list2D.AddY();

        // Act
        list2D.Expand(3, 3, 1);

        // Assert
        Assert.Equal(3, list2D.XSize); // Expand adds 1 to the parameter
        Assert.Equal(3, list2D.YSize); // Expand adds 1 to the parameter
        Assert.Equal(new[,]{ { 0, 1, 1 }, { 1, 1, 1 }, { 1, 1, 1 } }, list2D.ToArray());
    }

    [Fact]
    public void Expand_Default_SmallerThanCurrent_ShouldThrowArgumentOutOfRangeException() {
        // Arrange
        var list2D = new List2D<int>();
        list2D.AddX();
        list2D.AddX();
        list2D.AddY();

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => list2D.Expand(0, 2, 1));
        Assert.Throws<ArgumentOutOfRangeException>(() => list2D.Expand(2, 0, 1));
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
        list2D.Place(matrix, new Point2D(2, 1));

        // Assert
        Assert.Equal(1, list2D[2, 1]);
        Assert.Equal(2, list2D[2, 2]);
    }

    [Fact]
    public void Place_NoResize_NoOffset() {
        
        var list2D = new List2D<int>(2, 2);
        list2D.AddX();
        list2D.AddX();
        list2D.AddY();
        list2D.AddY();
        
        var matrix = new[,] { { 1, 2, 3 }, { 4, 5, 6 }, { 7, 8, 9 } };
        
        list2D.Place(matrix, resize: false);
        
        Assert.Equal(2, list2D.XSize);
        Assert.Equal(2, list2D.YSize);
        Assert.Equal(1, list2D[0, 0]);
        Assert.Equal(2, list2D[0, 1]);
        Assert.Equal(4, list2D[1, 0]);
        Assert.Equal(5, list2D[1, 1]);
    }
    
    [Fact]
    public void Place_NoResize_PositiveOffset() {
        
        var list2D = new List2D<int>(2, 2);
        list2D.AddX();
        list2D.AddX();
        list2D.AddY();
        list2D.AddY();
        
        var matrix = new[,] { { 1, 2, 3 }, { 4, 5, 6 }, { 7, 8, 9 } };
        
        list2D.Place(matrix, offsetPoint: new Point2D(1, 1), resize: false);
        
        Assert.Equal(2, list2D.XSize);
        Assert.Equal(2, list2D.YSize);
        Assert.Equal(0, list2D[0, 0]);
        Assert.Equal(0, list2D[0, 1]);
        Assert.Equal(0, list2D[1, 0]);
        Assert.Equal(1, list2D[1, 1]);
    }
    
    [Fact]
    public void Place_NoResize_NegativeOffset() {
        
        var list2D = new List2D<int>(2, 2);
        list2D.AddX();
        list2D.AddX();
        list2D.AddY();
        list2D.AddY();
        
        var matrix = new[,] { { 1, 2, 3 }, { 4, 5, 6 }, { 7, 8, 9 } };
        
        list2D.Place(matrix, offsetPoint: new Point2D(-1, -1), resize: false);
        
        Assert.Equal(2, list2D.XSize);
        Assert.Equal(2, list2D.YSize);
        Assert.Equal(5, list2D[0, 0]);
        Assert.Equal(6, list2D[0, 1]);
        Assert.Equal(8, list2D[1, 0]);
        Assert.Equal(9, list2D[1, 1]);
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
    public void Resize_DefaultValue_ShouldResizeCorrectly() {
        // Arrange
        var list2D = new List2D<int>(collection: new[,] { { 1, 2 }, { 3, 4 } });

        // Act
        list2D.Resize(3, 3, 42);

        // Assert
        Assert.Equal(3, list2D.XSize);
        Assert.Equal(3, list2D.YSize);
        Assert.Equal(1, list2D[0, 0]);
        Assert.Equal(2, list2D[0, 1]);
        Assert.Equal(42, list2D[0, 2]);
        Assert.Equal(3, list2D[1, 0]);
        Assert.Equal(4, list2D[1, 1]);
        Assert.Equal(42, list2D[1, 2]);
        Assert.Equal(42, list2D[2, 0]);
        Assert.Equal(42, list2D[2, 1]);
        Assert.Equal(42, list2D[2, 2]);
    }

    [Fact]
    public void Resize_Factory_ShouldResizeCorrectly() {
        // Arrange
        var list2D = new List2D<int>(collection: new[,] { { 1, 2 }, { 3, 4 } });
        int counter = 10;

        // Act
        list2D.Resize(3, 3, () => counter++);

        // Assert
        Assert.Equal(3, list2D.XSize);
        Assert.Equal(3, list2D.YSize);
        Assert.Equal(1, list2D[0, 0]);
        Assert.Equal(2, list2D[0, 1]);
        Assert.Equal(12, list2D[0, 2]); // counter: 10, 11, 12... (order of fill matters)
        Assert.Equal(3, list2D[1, 0]);
        Assert.Equal(4, list2D[1, 1]);
        Assert.Equal(15, list2D[1, 2]);
        Assert.Equal(16, list2D[2, 0]);
    }

    [Fact]
    public void Shrink_ValidSizes_ShouldShrinkCorrectly() {
        // Arrange
        var list2D = new List2D<int>(collection: new[,] { { 1, 2, 3 }, { 4, 5, 6 }, { 7, 8, 9 } });

        // Act
        list2D.Shrink(2, 2);

        // Assert
        Assert.Equal(2, list2D.XSize);
        Assert.Equal(2, list2D.YSize);
        Assert.Equal(1, list2D[0, 0]);
        Assert.Equal(2, list2D[0, 1]);
        Assert.Equal(4, list2D[1, 0]);
        Assert.Equal(5, list2D[1, 1]);
    }

    [Fact]
    public void Contains_ShouldReturnCorrectValue() {
        // Arrange
        var list2D = new List2D<int>(collection: new[,] { { 1, 2 }, { 3, 4 } });

        // Act & Assert
        Assert.True(list2D.Contains(3));
        Assert.False(list2D.Contains(5));
    }

    [Fact]
    public void ContainsAtX_ShouldReturnCorrectValue() {
        // Arrange
        var list2D = new List2D<int>(collection: new[,] { { 1, 2 }, { 3, 4 } });

        // Act & Assert
        Assert.True(list2D.ContainsAtX(1, 3));
        Assert.False(list2D.ContainsAtX(1, 1));
    }

    [Fact]
    public void ContainsAtY_ShouldReturnCorrectValue() {
        // Arrange
        var list2D = new List2D<int>(collection: new[,] { { 1, 2 }, { 3, 4 } });

        // Act & Assert
        Assert.True(list2D.ContainsAtY(1, 2));
        Assert.False(list2D.ContainsAtY(1, 1));
    }

    [Fact]
    public void Fill_Value_ShouldFillEntireList() {
        // Arrange
        var list2D = new List2D<int>();
        list2D.Expand(2, 2);

        // Act
        list2D.Fill(42);

        // Assert
        Assert.All(list2D.ToJagged(), col => Assert.All(col, val => Assert.Equal(42, val)));
    }

    [Fact]
    public void Fill_Value_Region_ShouldFillRegion() {
        // Arrange
        var list2D = new List2D<int>();
        list2D.Expand(3, 3);

        // Act
        list2D.Fill(42, 1, 2, 1, 2);

        // Assert
        Assert.Equal(0, list2D[0, 0]);
        Assert.Equal(42, list2D[1, 1]);
        Assert.Equal(42, list2D[1, 2]);
        Assert.Equal(42, list2D[2, 1]);
        Assert.Equal(42, list2D[2, 2]);
    }

    [Fact]
    public void Clear_ShouldResetList() {
        // Arrange
        var list2D = new List2D<int>(collection: new[,] { { 1, 2 }, { 3, 4 } });

        // Act
        list2D.Clear();

        // Assert
        Assert.Equal(0, list2D.XSize);
        Assert.Equal(0, list2D.YSize);
        Assert.Equal(4, list2D.XCapacity);
        Assert.Equal(4, list2D.YCapacity);
    }

    [Fact]
    public void CopyTo_Array_ShouldCopyCorrectly() {
        // Arrange
        var list2D = new List2D<int>(collection: new[,] { { 1, 2 }, { 3, 4 } });
        var target = new int[3, 3];

        // Act
        list2D.CopyTo(target, new Point2D(1, 1));

        // Assert
        Assert.Equal(1, target[1, 1]);
        Assert.Equal(2, target[1, 2]);
        Assert.Equal(3, target[2, 1]);
        Assert.Equal(4, target[2, 2]);
    }
    
    [Fact]
    public void Properties_ShouldReturnCorrectValues() {
        var list = new List2D<int>(new int[2, 3]);
        Assert.Equal(6, list.Count);
        Assert.Equal(2, list.XCount);
        Assert.Equal(3, list.YCount);
        Assert.False(list.IsReadOnly);
    }

    [Fact]
    public void Indexer_Range_ShouldWork() {
        var list = new List2D<int>(new[,] { { 1, 2, 3 }, { 4, 5, 6 }, { 7, 8, 9 } });
        
        var xRange = list[1..3, 1]; // elements at x=1,2 and y=1 => [5, 8]
        Assert.Equal(new[] { 5, 8 }, xRange);

        var yRange = list[1, 1..3]; // elements at x=1 and y=1,2 => [5, 6]
        Assert.Equal(new[] { 5, 6 }, yRange);
    }

    [Fact]
    public void Indexer_Index_ShouldWork() {
        var list = new List2D<int>(new[,] { { 1, 2 }, { 3, 4 } });
        Assert.Equal(4, list[^1, ^1]);
        list[^1, ^1] = 10;
        Assert.Equal(10, list[1, 1]);
    }

    [Fact]
    public void Fill_WithFunc_ShouldWork() {
        var list = new List2D<int>(new int[2, 2]);
        int val = 0;
        list.Fill(() => val++);
        Assert.Equal(0, list[0, 0]);
        Assert.Equal(1, list[0, 1]);
        Assert.Equal(2, list[1, 0]);
        Assert.Equal(3, list[1, 1]);
    }

    [Fact]
    public void FillRegion_WithFunc_ShouldWork() {
        var list = new List2D<int>(new int[4, 4]);
        int val = 0;
        list.Fill(() => val++, 1, 2, 1, 2); // x: 1,2; y: 1,2
        Assert.Equal(0, list[1, 1]);
        Assert.Equal(1, list[1, 2]);
        Assert.Equal(2, list[2, 1]);
        Assert.Equal(3, list[2, 2]);
        Assert.Equal(0, list[0, 0]);
    }

    [Fact]
    public void Place_List2D_ShouldWork() {
        var list = new List2D<int>(new int[4, 4]);
        var sub = new List2D<int>(new[,] { { 1, 2 }, { 3, 4 } });
        list.Place(sub, new Point2D(1, 1));
        
        Assert.Equal(1, list[1, 1]);
        Assert.Equal(2, list[1, 2]);
        Assert.Equal(3, list[2, 1]);
        Assert.Equal(4, list[2, 2]);
    }

    [Fact]
    public void Place_WithPredicate_ShouldWork() {
        var list = new List2D<int>(new int[2, 2]);
        list.Fill(10);
        var arr = new[,] { { 1, 20 }, { 30, 4 } };
        // Place only if source > target
        list.Place(arr, (tgt, src) => src > tgt, Point2D.Zero);
        
        Assert.Equal(10, list[0, 0]); // 1 < 10 (no place)
        Assert.Equal(20, list[0, 1]); // 20 > 10 (place)
        Assert.Equal(30, list[1, 0]); // 30 > 10 (place)
        Assert.Equal(10, list[1, 1]); // 4 < 10 (no place)
    }

    [Fact]
    public void CopyTo_Array_ShouldWork() {
        var list = new List2D<int>(new[,] { { 1, 2 }, { 3, 4 } });
        var target = new int[2, 2];
        list.CopyTo(target, Point2D.Zero);
        Assert.Equal(1, target[0, 0]);
        Assert.Equal(4, target[1, 1]);
    }
}
using System.Drawing;

namespace Metriks.Test;

public class PlaneTests {
    
    [Fact]
    public void Constructor_Default_ShouldInitializeWithZeroOffsets() {
        // Arrange & Act
        var list2D = new Plane<int>();

        // Assert
        Assert.Equal(0, list2D.XOriginOffset);
        Assert.Equal(0, list2D.YOriginOffset);
        Assert.Equal(0, list2D.XStart);
        Assert.Equal(0, list2D.YStart);
        Assert.Equal(-1, list2D.XEnd); // Size is 0, so end is -1
        Assert.Equal(-1, list2D.YEnd); // Size is 0, so end is -1
    }

    [Fact]
    public void Indexer_WithoutOffset_ShouldBehaveLikeBaseClass() {
        
        // Arrange
        var list2D = new Plane<int>();
        list2D.AddX();
        list2D.AddY();

        // Act & Assert
        list2D[0, 0] = 42;
        Assert.Equal(42, list2D[0, 0]);
    }

    [Fact]
    public void XStart_XEnd_YStart_YEnd_ShouldReflectOffsets() {
        // Arrange
        var list2D = new Plane<int>();
        list2D.AddX();
        list2D.AddX();
        list2D.AddX();
        list2D.AddY();
        list2D.AddY();

        // Act - Insert at negative coordinates to create offsets
        list2D.InsertAtY(0);
        list2D.InsertAtX(0);

        // Assert
        Assert.Equal(-1, list2D.XStart);
        Assert.Equal(-1, list2D.YStart);
        Assert.Equal(2, list2D.XEnd); // Size is 4, offset is 1, so end is 4-1-1 = 2
        Assert.Equal(1, list2D.YEnd); // Size is 3, offset is 1, so end is 3-1-1 = 1
    }

    [Fact]
    public void InsertXAt_AtNegativeIndex_ShouldAdjustOriginOffset() {
        // Arrange
        var list2D = new Plane<int>();
        list2D.AddX();
        list2D.AddX();
        list2D.AddY();
        list2D[0, 0] = 1;
        list2D[1, 0] = 2;

        // Act
        list2D.InsertAtX(0);
        list2D[-1, 0] = 0;

        // Assert
        Assert.Equal(1, list2D.XOriginOffset);
        Assert.Equal(-1, list2D.XStart);
        Assert.Equal(1, list2D.XEnd);
        Assert.Equal(0, list2D[-1, 0]);
        Assert.Equal(1, list2D[0, 0]);
        Assert.Equal(2, list2D[1, 0]);
    }

    [Fact]
    public void InsertYAt_AtNegativeIndex_ShouldAdjustOriginOffset() {
        // Arrange
        var list2D = new Plane<int>();
        list2D.AddX();
        list2D.AddY();
        list2D.AddY();
        list2D[0, 0] = 1;
        list2D[0, 1] = 2;

        // Act
        list2D.InsertAtY(0);
        list2D[0, -1] = 0;

        // Assert
        Assert.Equal(1, list2D.YOriginOffset);
        Assert.Equal(-1, list2D.YStart);
        Assert.Equal(1, list2D.YEnd);
        Assert.Equal(0, list2D[0, -1]);
        Assert.Equal(1, list2D[0, 0]);
        Assert.Equal(2, list2D[0, 1]);
    }

    [Fact]
    public void InsertXAt_AtPositiveIndex_ShouldNotAdjustOriginOffset() {
        // Arrange
        var list2D = new Plane<int>();
        list2D.AddX();
        list2D.AddX();
        list2D.AddY();

        // Act
        list2D.InsertAtX(1);

        // Assert
        Assert.Equal(0, list2D.XOriginOffset);
        Assert.Equal(3, list2D.XSize);
    }

    [Fact]
    public void InsertYAt_AtPositiveIndex_ShouldNotAdjustOriginOffset() {
        // Arrange
        var list2D = new Plane<int>();
        list2D.AddX();
        list2D.AddY();
        list2D.AddY();

        // Act
        list2D.InsertAtY(1);

        // Assert
        Assert.Equal(0, list2D.YOriginOffset);
        Assert.Equal(3, list2D.YSize);
    }

    [Fact]
    public void RemoveXAt_BeforeOrigin_ShouldDecreaseOriginOffset() {
        // Arrange
        var list2D = new Plane<int>();
        list2D.AddX();
        list2D.AddX();
        list2D.AddX();
        list2D.AddY();
        list2D.InsertAtY(0); // Creates offset of 1
            
        // Act
        list2D.RemoveAtY(-1);

        // Assert
        Assert.Equal(0, list2D.XOriginOffset);
        Assert.Equal(3, list2D.XSize);
    }

    [Fact]
    public void RemoveYAt_BeforeOrigin_ShouldDecreaseOriginOffset() {
        // Arrange
        var list2D = new Plane<int>();
        list2D.AddX();
        list2D.AddY();
        list2D.AddY();
        list2D.AddY();
        list2D.InsertAtX(0); // Creates offset of 1
            
        // Act
        list2D.RemoveAtX(-1);

        // Assert
        Assert.Equal(0, list2D.YOriginOffset);
        Assert.Equal(3, list2D.YSize);
    }

    [Fact]
    public void RemoveXAt_AtOrAfterOrigin_ShouldNotChangeOriginOffset() {
        // Arrange
        var list2D = new Plane<int>();
        list2D.AddX();
        list2D.AddX();
        list2D.AddX();
        list2D.AddY();
        list2D.InsertAtY(0); // Creates offset of 1
            
        // Act
        list2D.RemoveAtY(0);

        // Assert
        Assert.Equal(0, list2D.XOriginOffset);
        Assert.Equal(3, list2D.XSize);
    }

    [Fact]
    public void RemoveYAt_AtOrAfterOrigin_ShouldNotChangeOriginOffset() {
        // Arrange
        var list2D = new Plane<int>();
        list2D.AddX();
        list2D.AddY();
        list2D.AddY();
        list2D.AddY();
        list2D.InsertAtX(0); // Creates offset of 1
            
        // Act
        list2D.RemoveAtX(0);

        // Assert
        Assert.Equal(0, list2D.YOriginOffset);
        Assert.Equal(3, list2D.YSize);
    }

    [Fact]
    public void GetAtX_ShouldAdjustForOffset() {
        // Arrange
        var list2D = new Plane<int>();
        list2D.AddX();
        list2D.AddX();
        list2D.AddY();
        list2D.AddY();
        list2D.InsertAtX(0); // Creates offset of 1
        list2D[-1, 0] = 1;
        list2D[-1, 1] = 2;

        // Act
        var result = list2D.GetAtX(-1).ToList();

        // Assert
        Assert.Equal(new[] { 1, 2 }, result);
    }

    [Fact]
    public void GetAtY_ShouldAdjustForOffset() {
        // Arrange
        var list2D = new Plane<int>();
        list2D.AddX();
        list2D.AddX();
        list2D.AddY();
        list2D.AddY();
        list2D.InsertAtY(0); // Creates offset of 1
        list2D[0, -1] = 1;
        list2D[1, -1] = 2;

        // Act
        var result = list2D.GetAtY(-1).ToList();

        // Assert
        Assert.Equal(new[] { 1, 2 }, result);
    }

    [Fact]
    public void AllAtX_ShouldAdjustForOffset() {
        // Arrange
        var list2D = new Plane<int>();
        list2D.AddX();
        list2D.AddY();
        list2D.AddY();
        list2D.InsertAtX(0); // Creates offset of 1
        list2D[-1, 0] = 2;
        list2D[-1, 1] = 4;

        // Act
        var result = list2D.AllAtX(-1, x => x % 2 == 0);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void AllAtY_ShouldAdjustForOffset() {
        // Arrange
        var list2D = new Plane<int>();
        list2D.AddX();
        list2D.AddX();
        list2D.AddY();
        list2D.InsertAtY(0); // Creates offset of 1
        list2D[0, -1] = 2;
        list2D[1, -1] = 4;

        // Act
        var result = list2D.AllAtY(-1, x => x % 2 == 0);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Place_WithoutOffset_ShouldPlaceAtOrigin() {
        // Arrange
        var list2D = new Plane<int>();
        var matrix = new[,] { { 1, 2 }, { 3, 4 } };

        // Act
        list2D.Place(matrix, null);

        // Assert
        Assert.Equal(1, list2D[0, 0]);
        Assert.Equal(2, list2D[0, 1]);
        Assert.Equal(3, list2D[1, 0]);
        Assert.Equal(4, list2D[1, 1]);
    }

    [Fact]
    public void Place_WithPositiveOffset_ShouldPlaceAtOffset() {
        // Arrange
        var list2D = new Plane<int>();
        var matrix = new int[,] { { 1, 2 } };

        // Act
        list2D.Place(matrix, new Point2D(2, 1));

        // Assert
        Assert.Equal(1, list2D[2, 1]);
        Assert.Equal(2, list2D[2, 2]);
    }

    [Fact]
    public void Place_WithNegativeOffset_ShouldAdjustOriginOffsets() {
        // Arrange
        var list2D = new Plane<int>();
        var matrix = new int[,] { { 1, 2 }, { 3, 4 } };

        // Act
        list2D.Place(matrix, new Point2D(-1, -2));

        // Assert
        Assert.Equal(1, list2D.XOriginOffset);
        Assert.Equal(2, list2D.YOriginOffset);
        Assert.Equal(1, list2D[-1, -2]);
        Assert.Equal(2, list2D[-1, -1]);
        Assert.Equal(3, list2D[0, -2]);
        Assert.Equal(4, list2D[0, -1]);
    }

    [Fact]
    public void Place_WithPartialNegativeOffset_ShouldAdjustRelevantOffsets() {
        // Arrange
        var list2D = new Plane<int>();
        var matrix = new int[,] { { 1, 2 } };

        // Act
        list2D.Place(matrix, new Point2D(-1, 1));

        // Assert
        Assert.Equal(1, list2D.XOriginOffset);
        Assert.Equal(0, list2D.YOriginOffset);
        Assert.Equal(1, list2D[-1, 1]);
        Assert.Equal(2, list2D[-1, 2]);
    }

    [Fact]
    public void MultipleNegativeInsertions_ShouldAccumulateOffsets() {
        // Arrange
        var list2D = new Plane<int>();
        list2D.AddX();
        list2D.AddY();

        // Act
        list2D.InsertAtY(0);
        list2D.InsertAtY(-1);
        list2D.InsertAtX(0);
        list2D.InsertAtX(-1);

        // Assert
        Assert.Equal(2, list2D.XOriginOffset);
        Assert.Equal(2, list2D.YOriginOffset);
        Assert.Equal(-2, list2D.XStart);
        Assert.Equal(-2, list2D.YStart);
        Assert.Equal(0, list2D.XEnd); // Size is 3, offset is 2, so end is 3-1-2 = 0
        Assert.Equal(0, list2D.YEnd); // Size is 3, offset is 2, so end is 3-1-2 = 0
    }

    [Fact]
    public void NegativeIndexing_ShouldWorkCorrectly() {
        // Arrange
        var list2D = new Plane<int>();
        list2D.AddX();
        list2D.AddX();
        list2D.AddY();
        list2D.AddY();
        list2D.InsertAtY(0);
        list2D.InsertAtX(0);

        // Act & Assert
        list2D[-1, -1] = 1;
        list2D[-1, 0] = 2;
        list2D[0, -1] = 3;
        list2D[0, 0] = 4;
        list2D[1, 1] = 5;

        Assert.Equal(1, list2D[-1, -1]);
        Assert.Equal(2, list2D[-1, 0]);
        Assert.Equal(3, list2D[0, -1]);
        Assert.Equal(4, list2D[0, 0]);
        Assert.Equal(5, list2D[1, 1]);
    }

    [Theory]
    [InlineData(-3, 0)]
    [InlineData(3, 0)]
    [InlineData(0, -3)]
    [InlineData(0, 3)]
    public void Indexer_OutOfBounds_ShouldThrowIndexOutOfRangeException(int x, int y) {
        // Arrange
        var list2D = new Plane<int>();
        list2D.AddX();
        list2D.AddY();
        list2D.InsertAtY(0);
        list2D.InsertAtX(0);

        // Act & Assert
        Assert.Throws<IndexOutOfRangeException>(() => list2D[x, y]);
        Assert.Throws<IndexOutOfRangeException>(() => list2D[x, y] = 1);
    }

    [Fact]
    public void OriginOffsets_AfterComplexOperations_ShouldBeCorrect() {
        // Arrange
        var list2D = new Plane<int>();
        list2D.AddX();
        list2D.AddX();
        list2D.AddY();
        list2D.AddY();

        // Act - Complex sequence of operations
        list2D.InsertAtY(0); // XOriginOffset becomes 1
        list2D.InsertAtX(0); // YOriginOffset becomes 1
        list2D.InsertAtY(-1); // XOriginOffset becomes 2
        list2D.InsertAtX(-1); // YOriginOffset becomes 2
        list2D.RemoveAtY(-1); // XOriginOffset becomes 1
        list2D.RemoveAtX(0);  // YOriginOffset stays 1

        // Assert
        Assert.Equal(1, list2D.XOriginOffset);
        Assert.Equal(1, list2D.YOriginOffset);
        Assert.Equal(-1, list2D.XStart);
        Assert.Equal(-1, list2D.YStart);
    }
}
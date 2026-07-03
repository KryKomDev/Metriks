namespace Metriks.Tests;

public class Array4DTests {
    [Fact]
    public void Copy_FullArray_CopiesAllElements() {
        // Arrange
        var source = new int[2, 2, 2, 2];
        var val    = 0;

        for (var w = 0; w < 2; w++)
        for (var x = 0; x < 2; x++)
        for (var y = 0; y < 2; y++)
        for (var z = 0; z < 2; z++)
            source[w, x, y, z] = ++val;

        var destination = new int[2, 2, 2, 2];

        // Act
        Array4D.Copy(source, 0, 0, 0, 0, destination, 0, 0, 0, 0, 2, 2, 2, 2);

        // Assert
        Assert.Equal(source, destination);
    }

    [Fact]
    public void Copy_SubRegion_CopiesCorrectElements() {
        // Arrange
        var source = new int[3, 3, 3, 3];
        source[1, 1, 1, 1] = 1234;

        var destination = new int[1, 1, 1, 1];

        // Act
        Array4D.Copy(source, 1, 1, 1, 1, destination, 0, 0, 0, 0, 1, 1, 1, 1);

        // Assert
        Assert.Equal(1234, destination[0, 0, 0, 0]);
    }

    [Fact]
    public void Copy_PointAndSizeOverload_CopiesCorrectElements() {
        // Arrange
        var source = new int[3, 3, 3, 3];
        source[1, 1, 1, 1] = 42;
        var destination = new int[1, 1, 1, 1];

        // Act
        Array4D.Copy(
            source,
            new Point4D(1, 1, 1, 1),
            destination,
            new Point4D(0, 0, 0, 0),
            new Size4D(1, 1, 1, 1)
        );

        // Assert
        Assert.Equal(42, destination[0, 0, 0, 0]);
    }

    [Fact]
    public void Copy_Shorthand_CopiesCorrectElements() {
        // Arrange
        var source = new int[2, 2, 2, 2];
        source[0, 0, 0, 0] = 42;
        var destination = new int[3, 3, 3, 3];

        // Act
        Array4D.Copy(source, destination, new Point4D(1, 1, 1, 1));

        // Assert
        Assert.Equal(42, destination[1, 1, 1, 1]);
    }

    [Fact]
    public void Fill_SubRegion_FillsCorrectElements() {
        // Arrange
        var array = new int[3, 3, 3, 3];

        // Act
        Array4D.Fill(array, 42, 1, 1, 1, 1, 1, 1, 1, 1);

        // Assert
        Assert.Equal(42, array[1, 1, 1, 1]);
        Assert.Equal(0,  array[0, 0, 0, 0]);
    }

    [Fact]
    public void Clear_SubRegion_ClearsCorrectElements() {
        // Arrange
        var array = new int[3, 3, 3, 3];
        Array4D.Fill(array, 42, 0, 0, 0, 0, 3, 3, 3, 3);

        // Act
        Array4D.Clear(array, 1, 1, 1, 1, 1, 1, 1, 1);

        // Assert
        Assert.Equal(0,  array[1, 1, 1, 1]);
        Assert.Equal(42, array[0, 0, 0, 0]);
    }

    [Fact]
    public void Copy_NullSource_ThrowsNullReferenceException() {
        // Arrange
        int[,,,] source      = null!;
        var      destination = new int[1, 1, 1, 1];

        // Act & Assert
        Assert.Throws<NullReferenceException>(() => Array4D.Copy(source, 0, 0, 0, 0, destination, 0, 0, 0, 0, 1, 1, 1, 1));
    }

    [Fact]
    public void Fill_ZeroCount_DoesNothing() {
        // Arrange
        var array = new int[1, 1, 1, 1];

        // Act
        Array4D.Fill(array, 42, 0, 0, 0, 0, 0, 1, 1, 1);

        // Assert
        Assert.Equal(0, array[0, 0, 0, 0]);
    }

    [Fact]
    public void SliceAtW_ReturnsCorrectSlice() {
        var array = new int[2, 2, 2, 2];
        var val   = 0;

        for (var w = 0; w < 2; w++)
        for (var x = 0; x < 2; x++)
        for (var y = 0; y < 2; y++)
        for (var z = 0; z < 2; z++)
            array[w, x, y, z] = ++val;

        var slice = Array4D.SliceAtW(array, 1);

        Assert.Equal(9,  slice[0, 0, 0]);
        Assert.Equal(16, slice[1, 1, 1]);
    }

    [Fact]
    public void SliceAtX_ReturnsCorrectSlice() {
        var array = new int[2, 2, 2, 2];
        var val   = 0;

        for (var w = 0; w < 2; w++)
        for (var x = 0; x < 2; x++)
        for (var y = 0; y < 2; y++)
        for (var z = 0; z < 2; z++)
            array[w, x, y, z] = ++val;

        var slice = Array4D.SliceAtX(array, 1);

        Assert.Equal(5,  slice[0, 0, 0]);
        Assert.Equal(16, slice[1, 1, 1]);
    }

    [Fact]
    public void SliceAtY_ReturnsCorrectSlice() {
        var array = new int[2, 2, 2, 2];
        var val   = 0;

        for (var w = 0; w < 2; w++)
        for (var x = 0; x < 2; x++)
        for (var y = 0; y < 2; y++)
        for (var z = 0; z < 2; z++)
            array[w, x, y, z] = ++val;

        var slice = Array4D.SliceAtY(array, 1);

        Assert.Equal(3,  slice[0, 0, 0]);
        Assert.Equal(16, slice[1, 1, 1]);
    }

    [Fact]
    public void SliceAtZ_ReturnsCorrectSlice() {
        var array = new int[2, 2, 2, 2];
        var val   = 0;

        for (var w = 0; w < 2; w++)
        for (var x = 0; x < 2; x++)
        for (var y = 0; y < 2; y++)
        for (var z = 0; z < 2; z++)
            array[w, x, y, z] = ++val;

        var slice = Array4D.SliceAtZ(array, 1);

        Assert.Equal(2,  slice[0, 0, 0]);
        Assert.Equal(16, slice[1, 1, 1]);
    }

    [Fact]
    public void Flatten_ReturnsCorrectFlattenedArray() {
        var array = new int[2, 2, 2, 2];
        var val   = 0;

        for (var w = 0; w < 2; w++)
        for (var x = 0; x < 2; x++)
        for (var y = 0; y < 2; y++)
        for (var z = 0; z < 2; z++)
            array[w, x, y, z] = ++val;

        var flat = Array4D.Flatten(array);

        var expected = new int[16];

        for (var i = 0; i < 16; i++)
            expected[i] = i + 1;

        Assert.Equal(expected, flat);
    }
}
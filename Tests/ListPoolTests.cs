// Metriks.Tests
// Copyright (c) KryKom & ZlomenyMesic 2026

namespace Metriks.Tests;

public class ListPoolTests {
    [Fact]
    public void RentAndReturn_2D_ShouldReuseInstanceAndResetState() {
        var list1 = ListPool2D<int>.Rent(3, 4);
        Assert.NotNull(list1);
        Assert.Equal(0, list1.XSize);
        Assert.Equal(0, list1.YSize);
        Assert.True(list1.XCapacity >= 3);
        Assert.True(list1.YCapacity >= 4);

        // Fill and resize to set size
        list1.Resize(3, 4, 42);
        Assert.Equal(42, list1[0, 0]);

        ListPool2D<int>.Return(list1);

        var list2 = ListPool2D<int>.Rent(3, 4);
        // Verify same instance is returned
        Assert.Same(list1, list2);
        // Verify size is reset to 0
        Assert.Equal(0, list2.XSize);
        Assert.Equal(0, list2.YSize);

        // Verify elements are cleared
        list2.Resize(3, 4);
        Assert.Equal(0, list2[0, 0]);
    }

    [Fact]
    public void RentAndReturn_3D_ShouldReuseInstanceAndResetState() {
        var list1 = ListPool3D<int>.Rent(2, 2, 2);
        Assert.NotNull(list1);
        Assert.Equal(0, list1.XSize);
        Assert.Equal(0, list1.YSize);
        Assert.Equal(0, list1.ZSize);

        list1.Resize(2, 2, 2, 99);
        Assert.Equal(99, list1[0, 0, 0]);

        ListPool3D<int>.Return(list1);

        var list2 = ListPool3D<int>.Rent(2, 2, 2);
        Assert.Same(list1, list2);
        Assert.Equal(0, list2.XSize);

        list2.Resize(2, 2, 2);
        Assert.Equal(0, list2[0, 0, 0]);
    }

    [Fact]
    public void RentAndReturn_4D_ShouldReuseInstanceAndResetState() {
        var list1 = ListPool4D<int>.Rent(2, 2, 2, 2);
        Assert.NotNull(list1);
        Assert.Equal(0, list1.WSize);
        Assert.Equal(0, list1.XSize);
        Assert.Equal(0, list1.YSize);
        Assert.Equal(0, list1.ZSize);

        list1.Resize(2, 2, 2, 2, 77);
        Assert.Equal(77, list1[0, 0, 0, 0]);

        ListPool4D<int>.Return(list1);

        var list2 = ListPool4D<int>.Rent(2, 2, 2, 2);
        Assert.Same(list1, list2);
        Assert.Equal(0, list2.WSize);

        list2.Resize(2, 2, 2, 2);
        Assert.Equal(0, list2[0, 0, 0, 0]);
    }
}

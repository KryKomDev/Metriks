using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Metriks.Tests;

public class LinqExtensionsTests {
    [Fact]
    public void Linq2D_Flatten_ShouldReturnAllElements() {
        var list = new List2D<int>();
        list.Resize(2, 2);
        list[0, 0] = 1;
        list[0, 1] = 2;
        list[1, 0] = 3;
        list[1, 1] = 4;

        var flat = list.Flatten().ToList();

        Assert.Equal(new[] { 1, 2, 3, 4 }, flat);
    }

    [Fact]
    public void Linq2D_AnyAndAll_ShouldEvaluateCorrectly() {
        var list = new List2D<int>();
        list.Resize(2, 2);
        list[0, 0] = 1;
        list[0, 1] = 2;
        list[1, 0] = 3;
        list[1, 1] = 4;

        Assert.True(list.Any());
        Assert.True(list.Any(x => x == 3));
        Assert.False(list.Any(x => x == 10));
        Assert.True(list.All(x => x > 0));
        Assert.False(list.All(x => x < 4));
        Assert.Equal(2, list.Count(x => x % 2 == 0));
    }

    [Fact]
    public void Linq2D_Select_ShouldProjectCorrectly() {
        var list = new List2D<int>();
        list.Resize(2, 2);
        list[0, 0] = 1;
        list[0, 1] = 2;
        list[1, 0] = 3;
        list[1, 1] = 4;

        var projected = list.Select(x => x * 10);

        Assert.Equal(2, projected.XSize);
        Assert.Equal(2, projected.YSize);
        Assert.Equal(10, projected[0, 0]);
        Assert.Equal(20, projected[0, 1]);
        Assert.Equal(30, projected[1, 0]);
        Assert.Equal(40, projected[1, 1]);
    }

    [Fact]
    public void Linq2D_AnyAtXAndY_ShouldWork() {
        var list = new List2D<int>();
        list.Resize(3, 3);
        list[1, 1] = 42;

        Assert.True(list.AnyAtX(1, x => x == 42));
        Assert.False(list.AnyAtX(0, x => x == 42));
        Assert.True(list.AnyAtY(1, x => x == 42));
        Assert.False(list.AnyAtY(2, x => x == 42));
    }

    [Fact]
    public void Linq2D_SpaceSelect_ShouldProjectWithOffsets() {
        var space = new Space2D<int>(new[,] {
            { 1, 2 },
            { 3, 4 }
        });
        space.MoveOrigin(1, 1);

        var projected = space.Select(x => x * 10);

        Assert.Equal(1, projected.XOriginOffset);
        Assert.Equal(1, projected.YOriginOffset);
        Assert.Equal(10, projected[-1, -1]);
        Assert.Equal(20, projected[-1, 0]);
        Assert.Equal(30, projected[0, -1]);
        Assert.Equal(40, projected[0, 0]);
    }

    [Fact]
    public void Linq3D_Flatten_And_AllAtXYZ_ShouldWork() {
        var list = new List3D<int>();
        list.Resize(2, 2, 2);
        list[0, 1, 0] = 5;
        list[1, 1, 1] = 10;

        var flat = list.Flatten().ToList();
        Assert.Equal(8, flat.Count);
        Assert.Equal(2, flat.Count(x => x > 0));

        Assert.True(list.AnyAtX(0, x => x == 5));
        Assert.False(list.AnyAtX(0, x => x == 10));

        Assert.True(list.AllAtY(1, x => x == 0 || x == 5 || x == 10));
        Assert.False(list.AllAtY(1, x => x == 0));
    }

    [Fact]
    public void Linq4D_Flatten_And_AllAtWXYZ_ShouldWork() {
        var list = new List4D<int>();
        list.Resize(2, 2, 2, 2);
        list[1, 0, 1, 0] = 7;

        var flat = list.Flatten().ToList();
        Assert.Equal(16, flat.Count);
        Assert.Equal(7, flat.Max());

        Assert.True(list.AnyAtW(1, x => x == 7));
        Assert.False(list.AnyAtW(0, x => x == 7));
    }
}

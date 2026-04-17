namespace Metriks.Tests;

public class ThrowHelperTests {
    
    [Fact]
    public void ThrowIfLt_ShouldThrow_WhenLess() {
        Assert.Throws<Exception>(() => ThrowHelper.ThrowIfLt(1, 2));
        ThrowHelper.ThrowIfLt(2, 1);
        ThrowHelper.ThrowIfLt(1, 1);
    }

    [Fact]
    public void ThrowIfLeq_ShouldThrow_WhenLessOrEqual() {
        Assert.Throws<Exception>(() => ThrowHelper.ThrowIfLeq(1, 2));
        Assert.Throws<Exception>(() => ThrowHelper.ThrowIfLeq(1, 1));
        ThrowHelper.ThrowIfLeq(2, 1);
    }

    [Fact]
    public void ThrowIfGt_ShouldThrow_WhenGreater() {
        Assert.Throws<Exception>(() => ThrowHelper.ThrowIfGt(2, 1));
        ThrowHelper.ThrowIfGt(1, 2);
        ThrowHelper.ThrowIfGt(1, 1);
    }

    [Fact]
    public void ThrowIfGeq_ShouldThrow_WhenGreaterOrEqual() {
        Assert.Throws<Exception>(() => ThrowHelper.ThrowIfGeq(2, 1));
        Assert.Throws<Exception>(() => ThrowHelper.ThrowIfGeq(1, 1));
        ThrowHelper.ThrowIfGeq(1, 2);
    }

    [Fact]
    public void ThrowIf_ShouldThrow_WhenTrue() {
        Assert.Throws<Exception>(() => ThrowHelper.ThrowIf(true));
        ThrowHelper.ThrowIf(false);
    }

    [Fact]
    public void ThrowIfNegative_ShouldThrow_WhenNegative() {
        Assert.Throws<Exception>(() => ThrowHelper.ThrowIfNegative(-1));
        ThrowHelper.ThrowIfNegative(0);
        ThrowHelper.ThrowIfNegative(1);
    }

    [Fact]
    public void ThrowIfNull_ShouldThrow_WhenNull() {
        Assert.Throws<ArgumentNullException>(() => ThrowHelper.ThrowIfNull(null));
        ThrowHelper.ThrowIfNull(new object());
    }
}
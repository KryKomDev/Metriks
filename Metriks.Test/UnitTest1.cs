namespace Metriks.Test;

using Metriks;

public class UnitTest1 {
    
    [Fact]
    public void Test1() { }

    public static void Test() {
        var l = new List2D<int>();
        l.Expand(5, 5);
    }
}
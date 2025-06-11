namespace Metriks.Test;

using Metriks;

public class UnitTest1 {
    
    [Fact]
    public void Test1() { }

    public static unsafe void Test() {
        ListND<int> l = new(3, 3);

        Console.WriteLine(l.Count);
        Console.WriteLine(l.Capacity);
        
        l.Resize(0, 2);
        l.Resize(1, 2);
        l.Resize(2, 2);
        
        Console.WriteLine(l.Count);
        Console.WriteLine(l.Capacity);
    }
    public void Test1() {
        var x = stackalloc int[99999999];
    }
}
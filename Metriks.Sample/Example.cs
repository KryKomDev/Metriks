using System.Drawing;

namespace Metriks.Sample;

public static class Example {
    
    public static void Main() {
        var l = new FixedOriginList2D<int>();
        
        l.Expand(5, 5);
        
        l.WriteTable();
        Console.WriteLine();
        
        l.Place(new[,]{{1,1}, {1, 1}}, new Point(-3, -3));

        l.WriteTable();
        Console.WriteLine();
        
        l.InsertXAt(0);
        l.InsertYAt(0);
        l[0, 0] = 2;
        
        l.WriteTable();
        Console.WriteLine();
    }
}
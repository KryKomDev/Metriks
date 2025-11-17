namespace Metriks.Sample;

public static class Example {
    
    public static void Main() {
        
        // var l = new List2D<int>(new[,] { { 1, 2 }, { 3, 4 } });
        //
        // // create
        // l.AddX();
        // l.AddY();
        // l.InsertAtY(1);
        // l.InsertAtX(1);
        // l.Write();
        //
        // // destroy
        // l.RemoveAtY(1);
        // l.RemoveAtX(1);
        // l.ShrinkY();
        // l.ShrinkX();
        // l.Write();
        
        var p = new Plane<int>(new [,] { { 1, 2 }, { 3, 4 } });
        
        // create
        p.InsertAtX(1);
        p.InsertAtY(1);
        p.AddX();
        p.AddY();
        p.InsertAtX(0);
        p.InsertAtY(0);
        p.Write();
        
        Console.WriteLine(p.OriginOffset);
        
        // destroy
        p.RemoveAtX(1);
        p.RemoveAtY(1);
        p.ShrinkX();
        p.ShrinkY();
        p.RemoveAtX(-1);
        p.RemoveAtY(-1);
        p.Write();
        
        Console.WriteLine(p.OriginOffset);
        
        // destroy again
        p.RemoveAtX(0);
        p.RemoveAtY(0);
        p.Write();
        
        Console.WriteLine(p.OriginOffset);
    }
}
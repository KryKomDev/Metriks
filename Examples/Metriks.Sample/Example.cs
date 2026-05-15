using System.ComponentModel;
using System.Numerics;

namespace Metriks.Sample;

public static class Example {
    
    public static void Main() {

        int[,] arr = {
            { 0,  1,  2,  3  },
            { 4,  5,  6,  7  },
            { 8,  9,  10, 11 },
            { 12, 13, 14, 15 },
            { 16, 17, 18, 19 },
            { 20, 21, 22, 23 },
        };
        
        Console.WriteLine("x: " + arr.GetLength(0));
        Console.WriteLine("y: " + arr.GetLength(1));
        
        int[,] des = new int[6, 3];

        Array2D.Copy(arr, new Point2D(0, 1), des, Point2D.Zero, new Size2D(6, 3));

        Console.WriteLine(des.ToCollectionString());

        // var a = new List2D<int>();
        // a.Resize(151, 40, () => 1);
        // a.Write();
        // a.Resize(152, 39, () => 2);
        // a.Write();
        //
        // return;
        //
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
        //
        // var p = new Space2D<int>(new[,] { { 1, 2 }, { 3, 4 } });
        //
        // // create
        // p.InsertAtX(1);
        // p.InsertAtY(1);
        // p.AddX();
        // p.AddY();
        // p.InsertAtX(0);
        // p.InsertAtY(0);
        // p.Write();
        //
        // Console.WriteLine(p.OriginOffset);
        //
        // // destroy
        // p.RemoveAtX(1);
        // p.RemoveAtY(1);
        // p.ShrinkX();
        // p.ShrinkY();
        // p.RemoveAtX(-1);
        // p.RemoveAtY(-1);
        // p.Write();
        //
        // Console.WriteLine(p.OriginOffset);
        //
        // // destroy again
        // p.RemoveAtX(0);
        // p.RemoveAtY(0);
        // p.Write();
        //
        // Console.WriteLine(p.OriginOffset);

    }
}
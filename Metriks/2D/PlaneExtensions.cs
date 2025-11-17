// Metriks
// Copyright (c) KryKom & ZlomenyMesic 2025

namespace Metriks;

public static class PlaneExtensions {
    
    public static void WriteTable<T>(this Plane<T> list) {
        
        int maxXLen = Math.Max(list.XStart.ToString().Length, list.XEnd.ToString().Length);
        int maxYLen = Math.Max(list.YStart.ToString().Length, list.YEnd.ToString().Length);

        Console.Write(new string(' ', maxYLen + 1));
        for (int x = list.XStart; x < list.XEnd; x++) {
            Console.Write($"\e[1m {x.ToString().PadLeft(maxXLen)}");
        }

        Console.WriteLine("\e[0m");

        for (int y = list.YStart; y < list.YEnd; y++) {
            Console.Write($"\e[1m {y.ToString().PadLeft(maxYLen)} \e[0m");
            
            for (int x = list.XStart; x < list.XEnd; x++) {
                Console.Write($"{list[x, y]?.ToString()?.PadLeft(maxXLen)} ");
            }    
            Console.WriteLine();
        }
    }
}
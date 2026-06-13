namespace Metriks.Sample;

public static class Example {
    
    public static void Main() {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("=================================================");
        Console.WriteLine("         METRIKS MULTIDIMENSIONAL SAMPLES        ");
        Console.WriteLine("=================================================");
        Console.ResetColor();

        try {
            RunArray2DExample();
            RunList2DExample();
            RunList3DExample();
            RunList4DExample();
            RunSpace2DExample();
            RunSlicingAndFlatteningExample();
        }
        catch (Exception ex) {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"An error occurred during sample execution: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
            Console.ResetColor();
        }

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("\n=================================================");
        Console.WriteLine("         ALL SAMPLES COMPLETED SUCCESSFULLY      ");
        Console.WriteLine("=================================================");
        Console.ResetColor();
    }

    private static void RunArray2DExample() {
        PrintHeader("1. Array2D Operations");

        int[,] source = {
            { 1, 2, 3 },
            { 4, 5, 6 },
            { 7, 8, 9 }
        };

        Console.WriteLine("Source 2D Array:");
        source.Write();

        // Copy a sub-region
        int[,] destination = new int[3, 3];
        Console.WriteLine("Copying a 2x2 sub-region from source [1,1] to destination [0,0]:");
        Array2D.Copy(source, new Point2D(1, 1), destination, new Point2D(0, 0), new Size2D(2, 2));
        destination.Write();
    }

    private static void RunList2DExample() {
        PrintHeader("2. List2D<T> Operations");

        // Initialize from a 2D array
        var list = new List2D<int>(new int[,] {
            { 10, 20 },
            { 30, 40 }
        });

        Console.WriteLine("Initial List2D:");
        list.Write();

        // Resize the list
        Console.WriteLine("Resizing list to 3x3 (filling new cells with 99):");
        list.Resize(3, 3, () => 99);
        list.Write();

        // Add rows/columns
        Console.WriteLine("Inserting a new row at Y index 1:");
        list.InsertAtY(1);
        list.Write();

        Console.WriteLine("Inserting a new column at X index 1:");
        list.InsertAtX(1);
        list.Write();

        // Slice list using range indexing (Index & Range features)
        Console.WriteLine("Slicing: list[0..2, 2] (first two elements of column 2):");
        int[] slice = list[0..2, 2];
        Console.WriteLine($"Slice values: [ {string.Join(", ", slice)} ]");
    }

    private static void RunList3DExample() {
        PrintHeader("3. List3D<T> Operations");

        var list3D = new List3D<int>();
        list3D.Resize(2, 2, 2);
        // Populate
        for (int x = 0; x < 2; x++)
            for (int y = 0; y < 2; y++)
                for (int z = 0; z < 2; z++)
                    list3D[x, y, z] = (x + 1) * (y + 1) * (z + 1);

        Console.WriteLine($"Created a 3D List with size {list3D.Size} and total elements Count = {list3D.Count}.");
        Console.WriteLine($"Element at [1, 1, 1]: {list3D[1, 1, 1]}");

        Console.WriteLine("Resizing 3D List to 3x3x3 (filling new cells with -1):");
        list3D.Resize(3, 3, 3, () => -1);
        Console.WriteLine($"New size: {list3D.Size}, Element at [2, 2, 2]: {list3D[2, 2, 2]}");
    }

    private static void RunList4DExample() {
        PrintHeader("4. List4D<T> Operations");

        var list4D = new List4D<int>();
        list4D.Resize(2, 2, 2, 2);
        // Populate
        for (int w = 0; w < 2; w++)
            for (int x = 0; x < 2; x++)
                for (int y = 0; y < 2; y++)
                    for (int z = 0; z < 2; z++)
                        list4D[w, x, y, z] = w + x + y + z;

        Console.WriteLine($"Created a 4D List with size {list4D.Size} and total elements Count = {list4D.Count}.");
        Console.WriteLine($"Element at [1, 1, 1, 1]: {list4D[1, 1, 1, 1]}");

        Console.WriteLine("Resizing 4D List to 3x3x3x3 (filling new cells with -9):");
        list4D.Resize(3, 3, 3, 3, () => -9);
        Console.WriteLine($"New size: {list4D.Size}, Element at [2, 2, 2, 2]: {list4D[2, 2, 2, 2]}");
    }

    private static void RunSpace2DExample() {
        PrintHeader("5. Space2D<T> Spatial Operations");

        var space = new Space2D<int>(new int[,] {
            { 1, 2 },
            { 3, 4 }
        });

        Console.WriteLine("Initial Space2D (origin at [0,0]):");
        space.Write();

        Console.WriteLine("Moving the origin offset by (1, 1):");
        space.MoveOrigin(1, 1);
        Console.WriteLine($"Current bounds: X[{space.XStart} to {space.XEnd}], Y[{space.YStart} to {space.YEnd}]");
        Console.WriteLine($"Element at shifted coordinates [-1, -1] (originally [0,0]): {space[-1, -1]}");

        Console.WriteLine("Inserting a column at X coordinate -1:");
        space.InsertAtX(-1);
        space.Write();
        Console.WriteLine($"New origin offset: {space.OriginOffset}");
    }

    private static void RunSlicingAndFlatteningExample() {
        PrintHeader("6. Slicing and Flattening");

        // 3D List slicing
        var list3D = new List3D<int>(new int[,,] {
            { { 1, 2 }, { 3, 4 } },
            { { 5, 6 }, { 7, 8 } }
        });

        Console.WriteLine("Slicing a 3D list at X coordinate 1 (returns a 2D enumerable slice):");
        IEnumerable2D<int> slice2D = list3D.GetAtX(1);
        slice2D.Write();

        // 2D Array Flattening
        int[,] array2D = {
            { 10, 20 },
            { 30, 40 }
        };

        Console.WriteLine("Flattening a 2D array into a 1D array:");
        int[] flattened = Array2D.Flatten(array2D);
        Console.WriteLine($"Flattened elements: [ {string.Join(", ", flattened)} ]");
    }

    private static void PrintHeader(string title) {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"\n--- {title} ---");
        Console.ResetColor();
    }
}
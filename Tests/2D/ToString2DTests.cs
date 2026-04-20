namespace Metriks.Tests;

public class ToString2DTests {
    
    [Fact]
    public void ToCollectionString_ShouldReturnFormattedString() {
        var list = new List2D<int>(new int[,] { { 1, 2 }, { 3, 4 } });
        var result = list.ToCollectionString();
        
        // Expected (based on the code):
        // [
        //    [ 1, 2 ],
        //    [ 3, 4 ]
        // ]
        
        // Wait, string.Join(",\n", ...)
        // Environment.NewLine might be better in the code, but it uses \n.
        
        Assert.Contains("[ 1, 2 ]", result);
        Assert.Contains("[ 3, 4 ]", result);
    }
}
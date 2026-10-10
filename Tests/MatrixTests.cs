// Metriks.Tests
// Copyright (c) KryKom & ZlomenyMesic 2026

using System.Diagnostics.CodeAnalysis;

namespace Metriks.Tests;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public class MatrixTests {

    [Fact]
    public void Determinant_EmptyMatrix_ShouldReturnOne() {
        var m = new int[0, 0];
        Assert.Equal(1, Matrix.Determinant(m));
    }

    [Fact]
    public void Determinant_1x1_ShouldReturnSingleElement() {
        var m1 = new[,] { { 7 } };
        var m2 = new[,] { { -5 } };
        Assert.Equal(7,  Matrix.Determinant(m1));
        Assert.Equal(-5, Matrix.Determinant(m2));
    }

    [Fact]
    public void Determinant_2x2_ShouldCalculateCorrectly() {
        var m = new[,] {
            { 1, 2 },
            { 3, 4 }
        };

        // 1*4 - 2*3 = -2
        Assert.Equal(-2, Matrix.Determinant(m));
    }

    [Fact]
    public void Determinant_3x3_ShouldCalculateCorrectly() {
        var m = new[,] {
            { 6, 1, 1 },
            { 4, -2, 5 },
            { 2, 8, 7 }
        };

        Assert.Equal(-306, Matrix.Determinant(m));

        var singular = new[,] {
            { 1, 2, 3 },
            { 4, 5, 6 },
            { 7, 8, 9 }
        };

        Assert.Equal(0, Matrix.Determinant(singular));
    }

    [Fact]
    public void Determinant_3x3_WithPivotZero_ShouldSwapRowsCorrectly() {
        var m = new[,] {
            { 0, 1, 2 },
            { 3, 4, 5 },
            { 6, 7, 8 }
        };

        Assert.Equal(0, Matrix.Determinant(m));

        var m2 = new[,] {
            { 0, 2, 3 },
            { 4, 5, 6 },
            { 7, 8, 9 }
        };

        // det = - 2*(4*9 - 6*7) + 3*(4*8 - 5*7) = -2*(36-42) + 3*(32-35) = -2*(-6) + 3*(-3) = 12 - 9 = 3
        Assert.Equal(3, Matrix.Determinant(m2));
    }

    [Fact]
    public void Determinant_4x4_Identity_ShouldReturnOne() {
        var identity = Matrix.GetIdentity(4);
        Assert.Equal(1, Matrix.Determinant(identity));
    }

    [Fact]
    public void Determinant_4x4_Triangular_ShouldReturnDiagonalProduct() {
        var m = new[,] {
            { 2, 3, 1, 5 },
            { 0, 3, 4, 1 },
            { 0, 0, -2, 6 },
            { 0, 0, 0, 4 }
        };

        // Product of diagonal: 2 * 3 * -2 * 4 = -48
        Assert.Equal(-48, Matrix.Determinant(m));
    }

    [Fact]
    public void Determinant_4x4_General_ShouldCalculateCorrectly() {
        var m = new[,] {
            { 1, 0, 2, -1 },
            { 3, 0, 0, 5 },
            { 2, 1, 4, -3 },
            { 1, 0, 5, 0 }
        };

        // Expansion along col 1 (second column, index 1): only row 2 (index 2) is non-zero (element 1, sign -):
        // minor at (2,1):
        // [ 1, 2, -1 ]
        // [ 3, 0,  5 ]
        // [ 1, 5,  0 ]
        // det of minor: 1*(0 - 25) - 2*(0 - 5) + (-1)*(15 - 0) = -25 + 10 - 15 = -30
        // cofactor at (2,1) has sign (-1)^(2+1) = -1.
        // det = 1 * (-1) * (-30) = 30.
        Assert.Equal(30, Matrix.Determinant(m));
    }

    [Fact]
    public void Determinant_5x5_ShouldCalculateCorrectly() {
        var m = new[,] {
            { 2, 0, 0, 0, 0 },
            { 0, 3, 0, 0, 0 },
            { 0, 0, 4, 0, 0 },
            { 0, 0, 0, 5, 0 },
            { 0, 0, 0, 0, 6 }
        };

        Assert.Equal(720, Matrix.Determinant(m));
    }

    [Fact]
    public void Determinant_NonSquareMatrix_ShouldThrowArgumentException() {
        var m = new[,] {
            { 1, 2, 3 },
            { 4, 5, 6 }
        };

        Assert.Throws<ArgumentException>(() => Matrix.Determinant(m));
    }

    [Fact]
    public void Add_ShouldAddMatricesCorrectly() {
        var a = new[,] { { 1, 2 }, { 3, 4 } };
        var b = new[,] { { 5, 6 }, { 7, 8 } };
        var c = Matrix.Add(a, b);
        Assert.Equal(6,  c[0, 0]);
        Assert.Equal(8,  c[0, 1]);
        Assert.Equal(10, c[1, 0]);
        Assert.Equal(12, c[1, 1]);
    }

    [Fact]
    public void Sub_ShouldSubtractMatricesCorrectly() {
        var a = new[,] { { 5, 6 }, { 7, 8 } };
        var b = new[,] { { 1, 2 }, { 3, 4 } };
        var c = Matrix.Sub(a, b);
        Assert.Equal(4, c[0, 0]);
        Assert.Equal(4, c[0, 1]);
        Assert.Equal(4, c[1, 0]);
        Assert.Equal(4, c[1, 1]);
    }

    [Fact]
    public void Mul_ShouldMultiplyMatricesCorrectly() {
        var a = new[,] { { 1, 2 }, { 3, 4 } };
        var b = new[,] { { 2, 0 }, { 1, 2 } };
        var c = Matrix.Mul(a, b);

        // [1*2+2*1, 1*0+2*2] = [4, 4]
        // [3*2+4*1, 3*0+4*2] = [10, 8]
        Assert.Equal(4,  c[0, 0]);
        Assert.Equal(4,  c[0, 1]);
        Assert.Equal(10, c[1, 0]);
        Assert.Equal(8,  c[1, 1]);
    }

    [Fact]
    public void Transpose_ShouldTransposeMatrixCorrectly() {
        var a = new[,] { { 1, 2, 3 }, { 4, 5, 6 } };
        var t = Matrix.Transpose(a);
        Assert.Equal(3, t.GetLength(0));
        Assert.Equal(2, t.GetLength(1));
        Assert.Equal(1, t[0, 0]);
        Assert.Equal(4, t[0, 1]);
        Assert.Equal(2, t[1, 0]);
        Assert.Equal(5, t[1, 1]);
        Assert.Equal(3, t[2, 0]);
        Assert.Equal(6, t[2, 1]);
    }

    [Fact]
    public void GetIdentity_ShouldCreateIdentityMatrix() {
        var id = Matrix.GetIdentity(3);

        for (var i = 0; i < 3; i++) {
            for (var j = 0; j < 3; j++) {
                Assert.Equal(i == j ? 1 : 0, id[i, j]);
            }
        }
    }

    [Fact]
    public void Rank_EmptyMatrix_ShouldReturnZero() {
        var m0x0 = new int[0, 0];
        var m0x3 = new int[0, 3];
        var m3x0 = new int[3, 0];
        Assert.Equal(0, Matrix.Rank(m0x0));
        Assert.Equal(0, Matrix.Rank(m0x3));
        Assert.Equal(0, Matrix.Rank(m3x0));
    }

    [Fact]
    public void Rank_1x1_ShouldReturnCorrectRank() {
        var zero     = new[,] { { 0 } };
        var nonZero  = new[,] { { 5 } };
        var negative = new[,] { { -7 } };
        Assert.Equal(0, Matrix.Rank(zero));
        Assert.Equal(1, Matrix.Rank(nonZero));
        Assert.Equal(1, Matrix.Rank(negative));
    }

    [Fact]
    public void Rank_2x2_ShouldCalculateCorrectly() {
        var identity = Matrix.GetIdentity(2);
        Assert.Equal(2, Matrix.Rank(identity));

        var zero = new int[2, 2];
        Assert.Equal(0, Matrix.Rank(zero));

        var rank1Row = new[,] {
            { 1, 2 },
            { 2, 4 }
        };

        Assert.Equal(1, Matrix.Rank(rank1Row));

        var rank1Col = new[,] {
            { 1, 2 },
            { 3, 6 }
        };

        Assert.Equal(1, Matrix.Rank(rank1Col));

        var fullRank = new[,] {
            { 1, 2 },
            { 3, 4 }
        };

        Assert.Equal(2, Matrix.Rank(fullRank));
    }

    [Fact]
    public void Rank_3x3_ShouldCalculateCorrectly() {
        var identity = Matrix.GetIdentity(3);
        Assert.Equal(3, Matrix.Rank(identity));

        var singular = new[,] {
            { 1, 2, 3 },
            { 4, 5, 6 },
            { 7, 8, 9 }
        };

        Assert.Equal(2, Matrix.Rank(singular));

        var rank1 = new[,] {
            { 1, 2, 3 },
            { 2, 4, 6 },
            { 3, 6, 9 }
        };

        Assert.Equal(1, Matrix.Rank(rank1));

        var fullRank = new[,] {
            { 6, 1, 1 },
            { 4, -2, 5 },
            { 2, 8, 7 }
        };

        Assert.Equal(3, Matrix.Rank(fullRank));
    }

    [Fact]
    public void Rank_RectangularMatrix_ShouldCalculateCorrectly() {
        var rect2x3_rank2 = new[,] {
            { 1, 2, 3 },
            { 4, 5, 6 }
        };

        Assert.Equal(2, Matrix.Rank(rect2x3_rank2));

        var rect2x3_rank1 = new[,] {
            { 1, 2, 3 },
            { 2, 4, 6 }
        };

        Assert.Equal(1, Matrix.Rank(rect2x3_rank1));

        var rect3x2_rank2 = new[,] {
            { 1, 2 },
            { 3, 4 },
            { 5, 6 }
        };

        Assert.Equal(2, Matrix.Rank(rect3x2_rank2));

        var rect3x2_rank1 = new[,] {
            { 1, 2 },
            { 2, 4 },
            { 3, 6 }
        };

        Assert.Equal(1, Matrix.Rank(rect3x2_rank1));

        var rect3x4_rank3 = new[,] {
            { 1, 0, 2, -1 },
            { 3, 0, 0, 5 },
            { 2, 1, 4, -3 }
        };

        Assert.Equal(3, Matrix.Rank(rect3x4_rank3));
    }

    [Fact]
    public void Rank_WithZeroPivot_ShouldSwapRowsCorrectly() {
        var m = new[,] {
            { 0, 1, 2 },
            { 3, 4, 5 },
            { 6, 7, 8 }
        };

        Assert.Equal(2, Matrix.Rank(m));

        var m2 = new[,] {
            { 0, 2, 3 },
            { 4, 5, 6 },
            { 7, 8, 9 }
        };

        Assert.Equal(3, Matrix.Rank(m2));
    }

    [Fact]
    public void Triangle_EmptyMatrix_ShouldReturnEmptyMatrix() {
        var m0x0 = new int[0, 0];
        var t0x0 = Matrix.Triangle(m0x0);
        Assert.Equal(0, t0x0.GetLength(0));
        Assert.Equal(0, t0x0.GetLength(1));

        var m0x3 = new int[0, 3];
        var t0x3 = Matrix.Triangle(m0x3);
        Assert.Equal(0, t0x3.GetLength(0));
        Assert.Equal(3, t0x3.GetLength(1));

        var m3x0 = new int[3, 0];
        var t3x0 = Matrix.Triangle(m3x0);
        Assert.Equal(3, t3x0.GetLength(0));
        Assert.Equal(0, t3x0.GetLength(1));
    }

    [Fact]
    public void Triangle_1x1_ShouldReturnSingleElement() {
        var m = new[,] { { 5 } };
        var t = Matrix.Triangle(m);
        Assert.Equal(1,   t.GetLength(0));
        Assert.Equal(1,   t.GetLength(1));
        Assert.Equal(5.0, t[0, 0]);
    }

    [Fact]
    public void Triangle_2x2_ShouldCalculateCorrectly() {
        var m = new[,] {
            { 1, 2 },
            { 3, 4 }
        };

        var t = Matrix.Triangle(m);

        // Pivoting swaps row 1 with row 0: [3, 4], and row 1 becomes [0, 2 - (1/3)*4] = [0, 2/3]
        Assert.Equal(3.0,       t[0, 0], 5);
        Assert.Equal(4.0,       t[0, 1], 5);
        Assert.Equal(0.0,       t[1, 0], 5);
        Assert.Equal(2.0 / 3.0, t[1, 1], 5);
    }

    [Fact]
    public void Triangle_3x3_ShouldCalculateCorrectly() {
        var m = new[,] {
            { 2, 1, -1 },
            { -3, -1, 2 },
            { -2, 1, 2 }
        };

        var t = Matrix.Triangle(m);

        // All elements strictly below the main diagonal must be 0
        Assert.Equal(0.0, t[1, 0], 5);
        Assert.Equal(0.0, t[2, 0], 5);
        Assert.Equal(0.0, t[2, 1], 5);
    }

    [Fact]
    public void Triangle_3x3_Singular_ShouldHaveZeroRow() {
        var singular = new[,] {
            { 1, 2, 3 },
            { 4, 5, 6 },
            { 7, 8, 9 }
        };

        var t = Matrix.Triangle(singular);
        Assert.Equal(0.0, t[1, 0], 5);
        Assert.Equal(0.0, t[2, 0], 5);
        Assert.Equal(0.0, t[2, 1], 5);
        Assert.Equal(0.0, t[2, 2], 5);
    }

    [Fact]
    public void Triangle_RectangularMatrix_ShouldCalculateCorrectly() {
        var rect2x3 = new[,] {
            { 1, 2, 3 },
            { 4, 5, 6 }
        };

        var t2x3 = Matrix.Triangle(rect2x3);
        Assert.Equal(2,   t2x3.GetLength(0));
        Assert.Equal(3,   t2x3.GetLength(1));
        Assert.Equal(0.0, t2x3[1, 0], 5);

        var rect3x2 = new[,] {
            { 1, 2 },
            { 3, 4 },
            { 5, 6 }
        };

        var t3x2 = Matrix.Triangle(rect3x2);
        Assert.Equal(3,   t3x2.GetLength(0));
        Assert.Equal(2,   t3x2.GetLength(1));
        Assert.Equal(0.0, t3x2[1, 0], 5);
        Assert.Equal(0.0, t3x2[2, 0], 5);
        Assert.Equal(0.0, t3x2[2, 1], 5);
    }

    [Fact]
    public void Triangle_WithZeroPivot_ShouldSwapRowsCorrectly() {
        var m = new[,] {
            { 0, 1, 2 },
            { 3, 4, 5 },
            { 6, 7, 8 }
        };

        var t = Matrix.Triangle(m);
        Assert.Equal(6.0, t[0, 0], 5);
        Assert.Equal(0.0, t[1, 0], 5);
        Assert.Equal(0.0, t[2, 0], 5);
        Assert.Equal(0.0, t[2, 1], 5);
    }

    [Fact]
    public void Add_Double_ShouldAddMatricesCorrectly() {
        var a = new[,] { { 1.5, 2.5 }, { 3.0, 4.25 } };
        var b = new[,] { { 0.5, 1.5 }, { 2.0, 0.75 } };
        var c = Matrix.Add(a, b);
        Assert.Equal(2.0, c[0, 0], 5);
        Assert.Equal(4.0, c[0, 1], 5);
        Assert.Equal(5.0, c[1, 0], 5);
        Assert.Equal(5.0, c[1, 1], 5);
    }

    [Fact]
    public void Add_Double_MismatchedDimensions_ShouldThrowArgumentException() {
        var a = new double[2, 2];
        var b = new double[2, 3];
        Assert.Throws<ArgumentException>(() => Matrix.Add(a, b));
    }

    [Fact]
    public void Sub_Double_ShouldSubtractMatricesCorrectly() {
        var a = new[,] { { 5.5, 6.5 }, { 7.0, 8.25 } };
        var b = new[,] { { 1.5, 2.0 }, { 3.0, 4.25 } };
        var c = Matrix.Sub(a, b);
        Assert.Equal(4.0, c[0, 0], 5);
        Assert.Equal(4.5, c[0, 1], 5);
        Assert.Equal(4.0, c[1, 0], 5);
        Assert.Equal(4.0, c[1, 1], 5);
    }

    [Fact]
    public void Sub_Double_MismatchedDimensions_ShouldThrowArgumentException() {
        var a = new double[2, 2];
        var b = new double[3, 2];
        Assert.Throws<ArgumentException>(() => Matrix.Sub(a, b));
    }

    [Fact]
    public void Mul_Double_ShouldMultiplyMatricesCorrectly() {
        var a = new[,] { { 1.5, 2.0 }, { 3.0, 4.5 } };
        var b = new[,] { { 2.0, 0.5 }, { 1.0, 2.0 } };
        var c = Matrix.Mul(a, b);

        // [1.5*2+2*1, 1.5*0.5+2*2] = [5.0, 4.75]
        // [3*2+4.5*1, 3*0.5+4.5*2] = [10.5, 10.5]
        Assert.Equal(5.0,  c[0, 0], 5);
        Assert.Equal(4.75, c[0, 1], 5);
        Assert.Equal(10.5, c[1, 0], 5);
        Assert.Equal(10.5, c[1, 1], 5);
    }

    [Fact]
    public void Mul_Double_IncompatibleDimensions_ShouldThrowArgumentException() {
        var a = new double[2, 3];
        var b = new double[2, 3];
        Assert.Throws<ArgumentException>(() => Matrix.Mul(a, b));
    }

    [Fact]
    public void Transpose_Double_ShouldTransposeCorrectly() {
        var a = new[,] { { 1.5, 2.5, 3.5 }, { 4.5, 5.5, 6.5 } };
        var t = Matrix.Transpose(a);
        Assert.Equal(3,   t.GetLength(0));
        Assert.Equal(2,   t.GetLength(1));
        Assert.Equal(1.5, t[0, 0], 5);
        Assert.Equal(4.5, t[0, 1], 5);
        Assert.Equal(2.5, t[1, 0], 5);
        Assert.Equal(5.5, t[1, 1], 5);
        Assert.Equal(3.5, t[2, 0], 5);
        Assert.Equal(6.5, t[2, 1], 5);
    }

    [Fact]
    public void Determinant_Double_EmptyMatrix_ShouldReturnOne() {
        var m = new double[0, 0];
        Assert.Equal(1.0, Matrix.Determinant(m), 5);
    }

    [Fact]
    public void Determinant_Double_1x1_ShouldReturnSingleElement() {
        var m = new[,] { { 4.5 } };
        Assert.Equal(4.5, Matrix.Determinant(m), 5);
    }

    [Fact]
    public void Determinant_Double_2x2_ShouldCalculateCorrectly() {
        var m = new[,] {
            { 1.5, 2.0 },
            { 3.0, 4.0 }
        };

        // 1.5*4.0 - 2.0*3.0 = 6.0 - 6.0 = 0.0
        Assert.Equal(0.0, Matrix.Determinant(m), 5);

        var m2 = new[,] {
            { 2.5, 1.5 },
            { 0.5, 3.0 }
        };

        // 2.5*3.0 - 1.5*0.5 = 7.5 - 0.75 = 6.75
        Assert.Equal(6.75, Matrix.Determinant(m2), 5);
    }

    [Fact]
    public void Determinant_Double_3x3_ShouldCalculateCorrectly() {
        var m = new[,] {
            { 6.0, 1.0, 1.0 },
            { 4.0, -2.0, 5.0 },
            { 2.0, 8.0, 7.0 }
        };

        Assert.Equal(-306.0, Matrix.Determinant(m), 5);
    }

    [Fact]
    public void Determinant_Double_4x4_General_ShouldCalculateCorrectly() {
        var m = new[,] {
            { 1.0, 0.0, 2.0, -1.0 },
            { 3.0, 0.0, 0.0, 5.0 },
            { 2.0, 1.0, 4.0, -3.0 },
            { 1.0, 0.0, 5.0, 0.0 }
        };

        Assert.Equal(30.0, Matrix.Determinant(m), 5);
    }

    [Fact]
    public void Determinant_Double_NonSquare_ShouldThrowArgumentException() {
        var m = new double[2, 3];
        Assert.Throws<ArgumentException>(() => Matrix.Determinant(m));
    }

    [Fact]
    public void Rank_Double_ShouldCalculateCorrectly() {
        var rect = new[,] {
            { 1.0, 2.0, 3.0 },
            { 4.0, 5.0, 6.0 }
        };

        Assert.Equal(2, Matrix.Rank(rect));

        var singular = new[,] {
            { 1.0, 2.0, 3.0 },
            { 2.0, 4.0, 6.0 }
        };

        Assert.Equal(1, Matrix.Rank(singular));
    }

    [Fact]
    public void Triangle_Double_ShouldCalculateCorrectly() {
        var m = new[,] {
            { 1.0, 2.0 },
            { 3.0, 4.0 }
        };

        var t = Matrix.Triangle(m);
        Assert.Equal(3.0,       t[0, 0], 5);
        Assert.Equal(4.0,       t[0, 1], 5);
        Assert.Equal(0.0,       t[1, 0], 5);
        Assert.Equal(2.0 / 3.0, t[1, 1], 5);
    }

    [Fact]
    public void Inverse_EmptyMatrix_ShouldReturnEmptyMatrix() {
        var emptyInt = new int[0, 0];
        var invInt   = Matrix.Inverse(emptyInt);
        Assert.Equal(0, invInt.GetLength(0));
        Assert.Equal(0, invInt.GetLength(1));

        var emptyDouble = new double[0, 0];
        var invDouble   = Matrix.Inverse(emptyDouble);
        Assert.Equal(0, invDouble.GetLength(0));
        Assert.Equal(0, invDouble.GetLength(1));
    }

    [Fact]
    public void Inverse_1x1_ShouldCalculateCorrectly() {
        var m   = new[,] { { 4 } };
        var inv = Matrix.Inverse(m);
        Assert.Equal(0.25, inv[0, 0], 5);

        var mDouble   = new[,] { { 0.5 } };
        var invDouble = Matrix.Inverse(mDouble);
        Assert.Equal(2.0, invDouble[0, 0], 5);
    }

    [Fact]
    public void Inverse_2x2_ShouldCalculateCorrectly() {
        var m = new[,] {
            { 1, 2 },
            { 3, 4 }
        };

        var inv = Matrix.Inverse(m);

        // det = -2. inv = [[-2, 1], [1.5, -0.5]]
        Assert.Equal(-2.0, inv[0, 0], 5);
        Assert.Equal(1.0,  inv[0, 1], 5);
        Assert.Equal(1.5,  inv[1, 0], 5);
        Assert.Equal(-0.5, inv[1, 1], 5);

        // Check A * A^(-1) = I
        var mDouble = new double[,] { { 1, 2 }, { 3, 4 } };
        var prod    = Matrix.Mul(mDouble, inv);
        Assert.Equal(1.0, prod[0, 0], 5);
        Assert.Equal(0.0, prod[0, 1], 5);
        Assert.Equal(0.0, prod[1, 0], 5);
        Assert.Equal(1.0, prod[1, 1], 5);
    }

    [Fact]
    public void Inverse_3x3_ShouldCalculateCorrectly() {
        var m = new[,] {
            { 1.0, 2.0, 3.0 },
            { 0.0, 1.0, 4.0 },
            { 5.0, 6.0, 0.0 }
        };

        var inv  = Matrix.Inverse(m);
        var prod = Matrix.Mul(m, inv);

        for (var i = 0; i < 3; i++) {
            for (var j = 0; j < 3; j++) {
                Assert.Equal(i == j ? 1.0 : 0.0, prod[i, j], 5);
            }
        }
    }

    [Fact]
    public void Inverse_4x4_ShouldCalculateCorrectly() {
        var m = new[,] {
            { 1.0, 0.0, 2.0, -1.0 },
            { 3.0, 0.0, 0.0, 5.0 },
            { 2.0, 1.0, 4.0, -3.0 },
            { 1.0, 0.0, 5.0, 0.0 }
        };

        var inv  = Matrix.Inverse(m);
        var prod = Matrix.Mul(m, inv);

        for (var i = 0; i < 4; i++) {
            for (var j = 0; j < 4; j++) {
                Assert.Equal(i == j ? 1.0 : 0.0, prod[i, j], 5);
            }
        }
    }

    [Fact]
    public void Inverse_SingularMatrix_ShouldThrowInvalidOperationException() {
        var singularInt = new[,] {
            { 1, 2 },
            { 2, 4 }
        };

        Assert.Throws<InvalidOperationException>(() => Matrix.Inverse(singularInt));

        var singularDouble = new[,] {
            { 1.0, 2.0, 3.0 },
            { 4.0, 5.0, 6.0 },
            { 7.0, 8.0, 9.0 }
        };

        Assert.Throws<InvalidOperationException>(() => Matrix.Inverse(singularDouble));

        var zero1x1 = new[,] { { 0.0 } };
        Assert.Throws<InvalidOperationException>(() => Matrix.Inverse(zero1x1));
    }

    [Fact]
    public void Inverse_NonSquareMatrix_ShouldThrowArgumentException() {
        var m = new[,] {
            { 1, 2, 3 },
            { 4, 5, 6 }
        };

        Assert.Throws<ArgumentException>(() => Matrix.Inverse(m));

        var mDouble = new double[3, 2];
        Assert.Throws<ArgumentException>(() => Matrix.Inverse(mDouble));
    }
}
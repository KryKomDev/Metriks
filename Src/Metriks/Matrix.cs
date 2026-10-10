namespace Metriks;

public static class Matrix {
    public static int[,] Add(int[,] a, int[,] b) {
        if (a.Len0 != b.Len0 || a.Len1 != b.Len1)
            throw new ArgumentException("Matrices must have the same dimensions");

        var result = new int[a.Len0, a.Len1];

        for (var i = 0; i < a.Len0; i++)
        for (var j = 0; j < a.Len1; j++) {
            result[i, j] = a[i, j] + b[i, j];
        }

        return result;
    }

    public static double[,] Add(double[,] a, double[,] b) {
        if (a.Len0 != b.Len0 || a.Len1 != b.Len1)
            throw new ArgumentException("Matrices must have the same dimensions");

        var result = new double[a.Len0, a.Len1];

        for (var i = 0; i < a.Len0; i++)
        for (var j = 0; j < a.Len1; j++) {
            result[i, j] = a[i, j] + b[i, j];
        }

        return result;
    }

    public static int[,] Sub(int[,] a, int[,] b) {
        if (a.Len0 != b.Len0 || a.Len1 != b.Len1)
            throw new ArgumentException("Matrices must have the same dimensions");

        var result = new int[a.Len0, a.Len1];

        for (var i = 0; i < a.Len0; i++)
        for (var j = 0; j < a.Len1; j++) {
            result[i, j] = a[i, j] - b[i, j];
        }

        return result;
    }

    public static double[,] Sub(double[,] a, double[,] b) {
        if (a.Len0 != b.Len0 || a.Len1 != b.Len1)
            throw new ArgumentException("Matrices must have the same dimensions");

        var result = new double[a.Len0, a.Len1];

        for (var i = 0; i < a.Len0; i++)
        for (var j = 0; j < a.Len1; j++) {
            result[i, j] = a[i, j] - b[i, j];
        }

        return result;
    }

    public static int[,] Mul(int[,] a, int[,] b) {
        if (a.Len1 != b.Len0)
            throw new ArgumentException("Matrices must have compatible dimensions");

        var result = new int[a.Len0, b.Len1];

        for (int i = 0; i < a.Len0; i++)
        for (int j = 0; j < b.Len1; j++) {
            for (int k = 0; k < a.Len1; k++) {
                result[i, j] += a[i, k] * b[k, j];
            }
        }

        return result;
    }

    public static double[,] Mul(double[,] a, double[,] b) {
        if (a.Len1 != b.Len0)
            throw new ArgumentException("Matrices must have compatible dimensions");

        var result = new double[a.Len0, b.Len1];

        for (var i = 0; i < a.Len0; i++)
        for (var j = 0; j < b.Len1; j++) {
            for (var k = 0; k < a.Len1; k++) {
                result[i, j] += a[i, k] * b[k, j];
            }
        }

        return result;
    }

    public static int[,] Transpose(int[,] a) {
        var result = new int[a.Len1, a.Len0];

        for (int i = 0; i < a.Len0; i++)
        for (int j = 0; j < a.Len1; j++) {
            result[j, i] = a[i, j];
        }

        return result;
    }

    public static double[,] Transpose(double[,] a) {
        var result = new double[a.Len1, a.Len0];

        for (var i = 0; i < a.Len0; i++)
        for (var j = 0; j < a.Len1; j++) {
            result[j, i] = a[i, j];
        }

        return result;
    }

    public static int[,] GetIdentity(int n) {
        var result = new int[n, n];

        for (int i = 0; i < n; i++) {
            result[i, i] = 1;
        }

        return result;
    }

    public static int Determinant(int[,] m) {
        if (m.Len0 != m.Len1)
            throw new ArgumentException("Matrix must be square");

        var n = m.Len0;

        switch (n) {
            case 0: return 1;
            case 1: return m[0, 0];
            case 2: return m[0, 0] * m[1, 1] - m[0, 1] * m[1, 0];
            case 3: {
                return
                    m[0, 0] * (m[1, 1] * m[2, 2] - m[1, 2] * m[2, 1]) -
                    m[0, 1] * (m[1, 0] * m[2, 2] - m[1, 2] * m[2, 0]) +
                    m[0, 2] * (m[1, 0] * m[2, 1] - m[1, 1] * m[2, 0]);
            }
        }

        var a         = (int[,])m.Clone();
        var sign      = 1;
        var pivotPrev = 1;

        for (var k = 0; k < n - 1; k++) {
            if (a[k, k] == 0) {
                var swapRow = -1;

                for (var i = k + 1; i < n; i++) {
                    if (a[i, k] == 0)
                        continue;

                    swapRow = i;

                    break;
                }

                if (swapRow == -1)
                    return 0;

                for (var j = k; j < n; j++) {
                    (a[k, j], a[swapRow, j]) = (a[swapRow, j], a[k, j]);
                }

                sign = -sign;
            }

            var pivot = a[k, k];

            for (var i = k + 1; i < n; i++) {
                for (var j = k + 1; j < n; j++) {
                    var numerator = (long)a[i, j] * pivot - (long)a[i, k] * a[k, j];
                    a[i, j] = (int)(numerator / pivotPrev);
                }
            }

            pivotPrev = pivot;
        }

        return sign * a[n - 1, n - 1];
    }

    public static double Determinant(double[,] m) {
        if (m.Len0 != m.Len1)
            throw new ArgumentException("Matrix must be square");

        var n = m.Len0;

        switch (n) {
            case 0: return 1.0;
            case 1: return m[0, 0];
            case 2: return m[0, 0] * m[1, 1] - m[0, 1] * m[1, 0];
            case 3: {
                return
                    m[0, 0] * (m[1, 1] * m[2, 2] - m[1, 2] * m[2, 1]) -
                    m[0, 1] * (m[1, 0] * m[2, 2] - m[1, 2] * m[2, 0]) +
                    m[0, 2] * (m[1, 0] * m[2, 1] - m[1, 1] * m[2, 0]);
            }
        }

        var          a   = (double[,])m.Clone();
        var          det = 1.0;
        const double eps = 1e-12;

        for (var col = 0; col < n; col++) {
            var pivotRow = col;
            var maxVal   = Math.Abs(a[pivotRow, col]);

            for (var i = col + 1; i < n; i++) {
                var val = Math.Abs(a[i, col]);

                if (val > maxVal) {
                    maxVal   = val;
                    pivotRow = i;
                }
            }

            if (maxVal <= eps)
                return 0.0;

            if (pivotRow != col) {
                for (var j = col; j < n; j++) {
                    (a[col, j], a[pivotRow, j]) = (a[pivotRow, j], a[col, j]);
                }

                det = -det;
            }

            var pivot = a[col, col];
            det *= pivot;

            for (var i = col + 1; i < n; i++) {
                var factor = a[i, col] / pivot;

                if (Math.Abs(factor) <= eps)
                    continue;

                for (var j = col + 1; j < n; j++) {
                    a[i, j] -= factor * a[col, j];
                }
            }
        }

        return det;
    }

    /// <summary>
    /// Calculates the triangle form of the matrix m.
    /// </summary>
    public static double[,] Triangle(int[,] m) => Triangle(ToDouble(m));

    /// <summary>
    /// Calculates the triangle form of the matrix m.
    /// </summary>
    public static double[,] Triangle(double[,] m) {
        var rows = m.Len0;
        var cols = m.Len1;

        var a = (double[,])m.Clone();

        if (rows == 0 || cols == 0)
            return a;

        const double eps  = 1e-9;
        var          rank = 0;

        for (var col = 0; col < cols && rank < rows; col++) {
            var pivotRow = rank;
            var maxVal   = Math.Abs(a[pivotRow, col]);

            for (var i = rank + 1; i < rows; i++) {
                var val = Math.Abs(a[i, col]);

                if (!(val > maxVal))
                    continue;

                maxVal   = val;
                pivotRow = i;
            }

            if (maxVal <= eps)
                continue;

            if (pivotRow != rank) {
                for (var j = col; j < cols; j++) {
                    (a[rank, j], a[pivotRow, j]) = (a[pivotRow, j], a[rank, j]);
                }
            }

            var pivot = a[rank, col];

            for (var i = rank + 1; i < rows; i++) {
                var factor = a[i, col] / pivot;

                if (Math.Abs(factor) <= eps)
                    continue;

                a[i, col] = 0;

                for (var j = col + 1; j < cols; j++) {
                    a[i, j] -= factor * a[rank, j];
                }
            }

            rank++;
        }

        return a;
    }

    public static int Rank(int[,] m) => Rank(ToDouble(m));

    public static int Rank(double[,] m) {
        var rows = m.Len0;
        var cols = m.Len1;

        if (rows == 0 || cols == 0)
            return 0;

        var          a    = (double[,])m.Clone();
        const double eps  = 1e-9;
        var          rank = 0;

        for (var col = 0; col < cols && rank < rows; col++) {
            var pivotRow = rank;
            var maxVal   = Math.Abs(a[pivotRow, col]);

            for (var i = rank + 1; i < rows; i++) {
                var val = Math.Abs(a[i, col]);

                if (!(val > maxVal))
                    continue;

                maxVal   = val;
                pivotRow = i;
            }

            if (maxVal <= eps)
                continue;

            if (pivotRow != rank) {
                for (var j = col; j < cols; j++) {
                    (a[rank, j], a[pivotRow, j]) = (a[pivotRow, j], a[rank, j]);
                }
            }

            var pivot = a[rank, col];

            for (var i = rank + 1; i < rows; i++) {
                var factor = a[i, col] / pivot;

                if (Math.Abs(factor) <= eps)
                    continue;

                for (var j = col; j < cols; j++) {
                    a[i, j] -= factor * a[rank, j];
                }
            }

            rank++;
        }

        return rank;
    }

    public static double[,] Inverse(int[,] m) => Inverse(ToDouble(m));

    public static double[,] Inverse(double[,] m) {
        if (m.Len0 != m.Len1)
            throw new ArgumentException("Matrix must be square");

        var n = m.Len0;

        if (n == 0)
            return new double[0, 0];

        var a   = (double[,])m.Clone();
        var inv = new double[n, n];

        for (var i = 0; i < n; i++) {
            inv[i, i] = 1.0;
        }

        const double eps = 1e-12;

        for (var col = 0; col < n; col++) {
            var pivotRow = col;
            var maxVal   = Math.Abs(a[pivotRow, col]);

            for (var i = col + 1; i < n; i++) {
                var val = Math.Abs(a[i, col]);

                if (!(val > maxVal))
                    continue;

                maxVal   = val;
                pivotRow = i;
            }

            if (maxVal <= eps)
                throw new InvalidOperationException("Matrix is singular");

            if (pivotRow != col) {
                for (var j = 0; j < n; j++) {
                    (a[col, j], a[pivotRow, j])     = (a[pivotRow, j], a[col, j]);
                    (inv[col, j], inv[pivotRow, j]) = (inv[pivotRow, j], inv[col, j]);
                }
            }

            var pivot = a[col, col];

            for (var j = 0; j < n; j++) {
                a[col, j]   /= pivot;
                inv[col, j] /= pivot;
            }

            for (var i = 0; i < n; i++) {
                if (i == col)
                    continue;

                var factor = a[i, col];

                if (Math.Abs(factor) <= eps)
                    continue;

                for (var j = 0; j < n; j++) {
                    a[i, j]   -= factor * a[col, j];
                    inv[i, j] -= factor * inv[col, j];
                }
            }
        }

        return inv;
    }

    private static double[,] ToDouble(int[,] m) {
        var rows   = m.Len0;
        var cols   = m.Len1;
        var result = new double[rows, cols];

        for (var i = 0; i < rows; i++)
        for (var j = 0; j < cols; j++) {
            result[i, j] = m[i, j];
        }

        return result;
    }
}
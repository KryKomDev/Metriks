# Contiguous vs Jagged

```
BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.8457/25H2/2025Update/HudsonValley2)
12th Gen Intel Core i5-12400F 2.50GHz, 1 CPU, 12 logical and 6 physical cores
.NET SDK 10.0.203
  [Host]     : .NET 10.0.7 (10.0.7, 10.0.726.21808), X64 RyuJIT x86-64-v3
  DefaultJob : .NET 10.0.7 (10.0.7, 10.0.726.21808), X64 RyuJIT x86-64-v3
```

| Method              | Size |          Mean |        Error |       StdDev |     Gen0 |     Gen1 |     Gen2 | Allocated |
|---------------------|------|--------------:|-------------:|-------------:|---------:|---------:|---------:|----------:|
| Read_Jagged         | 10   |      40.22 ns |     0.564 ns |     0.527 ns |        - |        - |        - |         - |
| Read_Flat           | 10   |      43.06 ns |     0.795 ns |     0.705 ns |        - |        - |        - |         - |
| Write_Jagged        | 10   |      37.73 ns |     0.764 ns |     1.786 ns |        - |        - |        - |         - |
| Write_Flat          | 10   |      39.18 ns |     0.568 ns |     0.504 ns |        - |        - |        - |         - |
| InsertColumn_Jagged | 10   |      19.54 ns |     0.435 ns |     0.595 ns |   0.0187 |        - |        - |     176 B |
| InsertColumn_Flat   | 10   |      24.32 ns |     0.524 ns |     0.490 ns |   0.0493 |        - |        - |     464 B |
| InsertRow_Jagged    | 10   |      93.85 ns |     1.588 ns |     1.765 ns |   0.0875 |   0.0002 |        - |     824 B |
| InsertRow_Flat      | 10   |      63.42 ns |     1.313 ns |     2.083 ns |   0.0492 |        - |        - |     464 B |
| Resize_Jagged       | 10   |     137.40 ns |     2.758 ns |     4.757 ns |   0.2406 |   0.0019 |        - |    2264 B |
| Resize_Flat         | 10   |      74.66 ns |     1.541 ns |     1.582 ns |   0.1725 |        - |        - |    1624 B |
| Read_Jagged         | 100  |   3,261.07 ns |    42.832 ns |    40.065 ns |        - |        - |        - |         - |
| Read_Flat           | 100  |   4,614.65 ns |    70.203 ns |    65.668 ns |        - |        - |        - |         - |
| Write_Jagged        | 100  |   3,277.18 ns |    65.310 ns |   147.416 ns |        - |        - |        - |         - |
| Write_Flat          | 100  |   5,079.82 ns |    97.991 ns |    91.661 ns |        - |        - |        - |         - |
| InsertColumn_Jagged | 100  |      74.39 ns |     3.780 ns |    10.785 ns |   0.1334 |        - |        - |    1256 B |
| InsertColumn_Flat   | 100  |   2,129.80 ns |    41.992 ns |    46.674 ns |   4.2725 |        - |        - |   40424 B |
| InsertRow_Jagged    | 100  |   3,047.60 ns |    60.685 ns |   101.392 ns |   4.6768 |   0.6866 |        - |   44024 B |
| InsertRow_Flat      | 100  |   2,412.52 ns |    37.185 ns |    34.783 ns |   4.2725 |        - |        - |   40424 B |
| Resize_Jagged       | 100  |   7,988.72 ns |   148.289 ns |   212.671 ns |  17.6849 |   8.0872 |        - |  166424 B |
| Resize_Flat         | 100  |  33,246.52 ns |   662.918 ns | 1,012.346 ns |  49.9878 |  49.9878 |  49.9878 |  160041 B |
| Read_Jagged         | 500  |  70,390.28 ns |   603.760 ns |   535.217 ns |        - |        - |        - |         - |
| Read_Flat           | 500  | 103,965.94 ns |   906.164 ns |   847.626 ns |        - |        - |        - |         - |
| Write_Jagged        | 500  |  81,761.04 ns | 1,081.859 ns |   903.401 ns |        - |        - |        - |         - |
| Write_Flat          | 500  | 117,955.24 ns | 1,181.211 ns | 1,104.906 ns |        - |        - |        - |         - |
| InsertColumn_Jagged | 500  |     267.95 ns |     4.917 ns |     6.394 ns |   0.6437 |        - |        - |    6056 B |
| InsertColumn_Flat   | 500  | 108,869.00 ns | 1,957.220 ns | 1,830.785 ns | 183.8379 | 183.7158 | 183.7158 | 1002081 B |
| InsertRow_Jagged    | 500  |  77,166.57 ns | 1,477.869 ns | 1,382.399 ns | 108.3984 |  67.7490 |        - | 1020024 B |
| InsertRow_Flat      | 500  | 107,803.62 ns | 2,097.225 ns | 2,059.755 ns | 180.0537 | 179.9316 | 179.9316 | 1002080 B |
| Resize_Jagged       | 500  | 302,980.51 ns | 6,050.208 ns | 6,724.790 ns | 428.2227 | 427.7344 |        - | 4032024 B |
| Resize_Flat         | 500  | 332,913.23 ns | 6,493.920 ns | 9,518.704 ns | 434.5703 | 434.5703 | 434.5703 | 4000159 B |

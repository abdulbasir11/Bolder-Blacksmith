﻿
using System.Collections.Generic;

public static class Dictionaries
{
    public static readonly Dictionary<string, double[]> weights =
        new Dictionary<string, double[]>
        {
            {"covalence", new [] { 1.0 / 8.0, 1.0 / 8.0, 2.0 / 8.0, 3.0 / 8.0, 2.0 / 8.0, 1.0 / 8.0, 1.0 / 8.0, 1.0 / 8.0 } },
            {"covalence_test", new [] {0.9,0.0,0.0,0.0,0.0,0.0,0.0,0.0} }
        };

    public static readonly Dictionary<int, string> cubics =
        new Dictionary<int, string>
         {
            {1,"body-centered" },
            {2, "face-centered" },
            {3, "hexagonal-cuboid" }
         };

    public static readonly Dictionary<int, string> polygons =
    new Dictionary<int, string>
        {
            {3,"trigonal"},
            {4, "tetragonal" },
            {5, "pentagonal" },
            {6, "hexagonal"},
            {7, "septagonal" },
            {8, "octagonal" },
            {9, "nonogonal" },
            {10, "decagonal" },
            {11, "undecagonal" },
            {12, "dodecagonal" }

        };


}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridUtility
{

    public static int CalculateChebyshevDistance(int x1, int x2, int y1, int y2)
    {
        return Math.Max(Math.Abs(x2 - x1), Math.Abs(y2 - y1));
    }

}

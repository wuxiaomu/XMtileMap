using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point {

    public Point Parent { get; set; }
    public float H;
    public int X;
    public int Y;
    public float G;
    public float F;
    public bool Walkable = false;
    public Point(int x, int y, Point parent = null)
    {
        this.X = x;
        this.Y = y;
        this.Parent = parent;
    }
    public void UpdateParent(Point parent, float g)
    {
        this.Parent = parent;
        this.G = g;
        this.F = G + H;
    }
}

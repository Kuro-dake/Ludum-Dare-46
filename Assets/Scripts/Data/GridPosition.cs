using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class GridPosition
{
    public int x, y;
    const int max_y = 100000;
    public GridPosition(int _x, int _y)
    {
        if(y >= max_y)
        {
            throw new UnityException("GetHashCode is currently not going to work with y this big, so rework the GetHashCode func as needed. o_O Post me on r/programminghorror. I DON'T CARE! :'(");
        }
        x = _x;
        y = _y;
    }

    public override bool Equals(object other)
    {
        if (!(other is GridPosition))
        {
            return false;
        }
        GridPosition other_gp = (GridPosition)other;
        return this == other_gp;
    }

    public static bool operator ==(GridPosition a, GridPosition b)
    {
        return a.x == b.x && a.y == b.y;
    }
    public static bool operator !=(GridPosition a, GridPosition b)
    {
        return !(a==b);
    }

    public override int GetHashCode()
    {
        return x * max_y + y + 1;
    }

    public static GridPosition gp(int x, int y)
    {
        return new GridPosition(x,y);
    }

    public static GridPosition operator + (GridPosition a, GridPosition b)
    {
        return GridPosition.gp(a.x + b.x, a.y + b.y);
    }

    public GridPosition this[direction d]
    {
        get
        {
            switch (d)
            {
                case direction.up:
                    return gp(x, y + 1);
                case direction.down:
                    return gp(x, y - 1);
                case direction.left:
                    return gp(x - 1, y);
                case direction.right:
                    return gp(x + 1, y);
            }
            return gp(x, y);
        }
    }

    public static GridPosition zero
    {
        get { return gp(0, 0); }
    }

    public override string ToString()
    {
        return "x: " + x + "; y: " + y;
    }

}

using System;

public enum Sides
{
    Bottom, // 3
    Right,  // 2
    Left,   // 1
    Top,    // 0
}

public enum SidesAndDiagonals
{
    Top = 0,
    TopRight = 1,
    Right = 2,
    BottomRight = 3,
    Bottom = 4,
    BottomLeft = 5,
    Left = 6,
    TopLeft = 7
}

public class Tile
{
    public static readonly int[] tableWeight =
    {
        1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
        2, 4, int.MaxValue, 1, 1, 1
    };

    public int id;
    public Tile[] adjacents2 = new Tile[4];
    public Tile[] adjacents = new Tile[8];

    public int autoTileId;
    public int autoFowId;

    public Tile previous = null;

    public bool isVisited = false;

    public bool CanMove
    {
        get
        {
            return (autoTileId != (int)TileTypes.Empty && Weight < int.MaxValue);
        }
    }

    public int Weight
    {
        get
        {
            if (autoTileId < 0 || autoTileId >= tableWeight.Length)
            {
                return int.MaxValue;
            }
            return tableWeight[autoTileId];
        }
    }

    public void Clear()
    {
        previous = null;
    }

    public void UpdateAuotoTileId()
    {
        autoTileId = 0;
        for (int i = 0; i < adjacents2.Length; ++i)
        {
            if (adjacents2[i] != null)
            {
                autoTileId |= (1 << (adjacents2.Length - 1 - i));
            }
        }
    }

    //public void UpdateAuotoTileId()
    //{
    //    int mask = 0;

    //    if (adjacents[(int)SidesAndDiagonals.Top] != null) mask |= 1 << 0; 
    //    if (adjacents[(int)SidesAndDiagonals.Right] != null) mask |= 1 << 1; 
    //    if (adjacents[(int)SidesAndDiagonals.Bottom] != null) mask |= 1 << 2; 
    //    if (adjacents[(int)SidesAndDiagonals.Left] != null) mask |= 1 << 3;

    //    autoTileId = mask; // 0..15
    //}

    public void UpdateAuotoFowId()
    {
        var sb = new System.Text.StringBuilder();
        for (int i = 0; i < adjacents.Length; ++i)
        {
            if (adjacents[i] == null)
            {
                sb.Append("1");
            }
            else
            {
                sb.Append(adjacents[i].isVisited ? "0" : "1");
            }
        }
        autoFowId = System.Convert.ToInt32(sb.ToString(), 2);
    }

    public void RemoveAdjacents(Tile tile)
    {
        for (int i = 0; i < adjacents2.Length; ++i)
        {
            if (adjacents2[i] == null)
                continue;

            if (adjacents2[i].id == tile.id)
            {
                adjacents2[i] = null;
                break;
            }
        }
        UpdateAuotoTileId();
    }

    public void ClearAdjacents()
    {
        for (int i = 0; i < adjacents2.Length; ++i)
        {
            if (adjacents2[i] == null)
                continue;
            adjacents2[i].RemoveAdjacents(this);
            adjacents2[i] = null;
        }
        UpdateAuotoTileId();
    }
}

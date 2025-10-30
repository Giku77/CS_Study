using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum TileTypes
{
    Empty = -1,
    // 0, 14
    Grass = 15,
    Tree = 16,
    Hills = 17,
    Mountains = 18,
    Towns = 19,
    Castle = 20,
    Monster = 21
}

public class Map
{ 
    public int rows = 0;
    public int cols = 0;

    public Tile[] tiles;

    public Tile castleTile;
    public Tile startTile;

    //public Tile[] LandTiles
    //{
    //    get { return tiles.Where(t => t.autoTileId != (int)TileTypes.Empty).ToArray(); }
    //}

    //public Tile[] CoastTiles
    //{
    //    get
    //    {
    //        return tiles.Where(t =>
    //            t.autoTileId != (int)TileTypes.Empty &&      
    //            t.adjacents.Any(n => n == null || n.autoTileId == (int)TileTypes.Empty) 
    //        ).ToArray();
    //    }
    //}


    public Tile[] CoastTiles
    {

        get
        {
            return tiles.Where(t => t.autoTileId < (int)TileTypes.Grass).ToArray();
        }
    }

    public Tile[] LandTiles
    {
        get
        {
            return tiles.Where(t => t.autoTileId >= (int)TileTypes.Grass).ToArray();
        }
    }

    public void Init(int rows, int cols)   // 0: O 1: X
    {
        this.rows = rows;
        this.cols = cols;

        tiles = new Tile[rows * cols];
        for (int i = 0; i  < tiles.Length; i++)
        {
            tiles[i] = new Tile();
            tiles[i].id = i;
        }

        for (var r = 0; r < rows; ++r)
        {
            for (var c = 0; c < cols; ++c)
            {
                var index = r * cols + c;

                var indexU = (r - 1) * cols + c;
                var indexR = r * cols + c + 1;
                var indexD = (r + 1) * cols + c;
                var indexL = r * cols + c - 1;

                if ((r - 1) >= 0)
                {
                    tiles[index].adjacents[(int)SidesAndDiagonals.Top] = tiles[indexU];
                    tiles[index].adjacents2[(int)Sides.Top] = tiles[indexU];
                }
                if (c + 1 < cols)
                {
                    tiles[index].adjacents[(int)SidesAndDiagonals.Right] = tiles[indexR];
                    tiles[index].adjacents2[(int)Sides.Right] = tiles[indexR];
                }
                if (r + 1 < rows)
                {
                    tiles[index].adjacents[(int)SidesAndDiagonals.Bottom] = tiles[indexD];
                    tiles[index].adjacents2[(int)Sides.Bottom] = tiles[indexD];
                }
                if (c - 1 >= 0)
                {
                    tiles[index].adjacents[(int)SidesAndDiagonals.Left] = tiles[indexL];
                    tiles[index].adjacents2[(int)Sides.Left] = tiles[indexL];
                }
                // Diagonals
                if ((r - 1) >= 0 && (c + 1) < cols)
                {
                    tiles[index].adjacents[(int)SidesAndDiagonals.TopRight] = tiles[indexU + 1];
                }
                if ((r + 1) < rows && (c + 1) < cols)
                {
                    tiles[index].adjacents[(int)SidesAndDiagonals.BottomRight] = tiles[indexD + 1];
                }
                if ((r + 1) < rows && (c - 1) >= 0)
                {
                    tiles[index].adjacents[(int)SidesAndDiagonals.BottomLeft] = tiles[indexD - 1];
                }
                if ((r - 1) >= 0 && (c - 1) >= 0)
                {
                    tiles[index].adjacents[(int)SidesAndDiagonals.TopLeft] = tiles[indexU - 1];
                }
            }
        }

        for (int i = 0; i < tiles.Length; i++)
        {
            tiles[i].UpdateAuotoTileId();
            tiles[i].UpdateAuotoFowId();
        }
    }
    
    public bool CreateIsland(
        float erodePercent,
        int erodeIterations,
        float lakePercent,
        float treePercent,
        float hillPercent,
        float mountainPercent,
        float townPercent,
        float monsterPercent)
    {
        DecorateTiles(LandTiles, lakePercent, TileTypes.Empty);

        for (int i = 0; i < erodeIterations; ++i)
            DecorateTiles(CoastTiles, erodePercent, TileTypes.Empty);

        DecorateTiles(LandTiles, treePercent, TileTypes.Tree);
        DecorateTiles(LandTiles, hillPercent, TileTypes.Hills);
        DecorateTiles(LandTiles, mountainPercent, TileTypes.Mountains);
        DecorateTiles(LandTiles, townPercent, TileTypes.Towns);
        DecorateTiles(LandTiles, monsterPercent, TileTypes.Monster);

        var towns = tiles.Where(x => x.autoTileId == (int)TileTypes.Towns).ToArray();
        ShuffleTiles(towns);
        startTile = towns[0];

        var catsleTargets = tiles.Where(x => x.autoTileId == (int)TileTypes.Towns).ToArray();
        ShuffleTownsToCasstlee(catsleTargets);
        if (castleTile == null)
            return false;
        //castleTile = catsleTargets[Random.Range(0, catsleTargets.Length)];

        return true;
    }

    public void DecorateTiles(Tile[] tiles, float percent, TileTypes tileType)
    {
        int total = Mathf.FloorToInt(tiles.Length * percent);

        ShuffleTiles(tiles);

        for (int i = 0; i < total; ++i)
        {
            if (tileType == TileTypes.Empty)
                tiles[i].ClearAdjacents();

            tiles[i].autoTileId = (int)tileType;
        }
    }

    public void ShuffleTiles(Tile[] tiles)
    {
        // Fisher-Yates 셔플 알고리즘 구현
        for (int i = tiles.Length - 1; i > 0; i--)
        {
            // 0과 i 사이의 무작위 인덱스 선택
            int randomIndex = Random.Range(0, i + 1);
            
            // i번째 요소와 무작위로 선택된 요소 교환
            Tile temp = tiles[i];
            tiles[i] = tiles[randomIndex];
            tiles[randomIndex] = temp;
        }
    }

    private bool ShuffleTownsToCasstlee(Tile[] towns)
    {
        var search = new TileSearch();
        search.Init(this);
        for (int i = 0; i < towns.Length; i++)
        {
            if (towns[i] == startTile) continue;
            bool found = search.Astar(startTile, towns[i]);
            if (found)
            {
                towns[i].autoTileId = (int)TileTypes.Castle;
                castleTile = towns[i];
                return true;
            }
        }
        return false;
    }
}

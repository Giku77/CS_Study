using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.Experimental.Playables;
using static GraphTest;
using System.Threading;

public class Stage : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject tilePrefab;
    private GameObject[] tileObjs;
    private GameObject player;

    public int mapWidth = 20;
    public int mapHeight = 20;

    [Range(0f, 0.9f)]
    public float erodePercent = 0.5f;
    public int erodeIteration = 2;
    [Range(0f, 0.9f)]
    public float lakePercent = 0.1f;

    [Range(0f, 0.9f)]
    public float treePercent = 0.1f;
    [Range(0f, 0.9f)]
    public float hillPercent = 0.1f;
    [Range(0f, 0.9f)]
    public float moutainPercent = 0.1f;
    [Range(0f, 0.9f)]
    public float townPercent = 0.1f;
    [Range(0f, 0.9f)]
    public float monsterPercent = 0.1f;

    public Vector2 tileSize = new Vector2(16, 16);

    //public Texture2D islandTexture;
    public Sprite[] islandSprites;
    public Sprite[] fowSprites;

    private Map map;

    public enum AlgorithmType
    {
        DFS,
        BFS,
        DFS_Recursive,
        PathFindingBFS,
        Dikjstra,
        AStar
    }

    private AlgorithmType algorithmType;
    public Map Map
    {
        get { return map; }
    }

    private Vector3 firstTilePos;

    public int ScreenPosToTileId(Vector3 screenPos)
    {
        screenPos.z = Mathf.Abs(transform.position.z - cam.transform.position.z);
        var worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        return WorldPosToTileId(worldPos);
    }

    public int WorldPosToTileId(Vector3 worldPos)
    {
        var pivot = firstTilePos;
        pivot.x -= tileSize.x * 0.5f;
        pivot.y += tileSize.y * 0.5f;

        var diff = worldPos - pivot;
        int x = Mathf.FloorToInt(diff.x / tileSize.x);
        int y = -Mathf.CeilToInt(diff.y / tileSize.y);

        x = Mathf.Clamp(x, 0, mapWidth - 1);
        y = Mathf.Clamp(y, 0, mapHeight - 1);

        return y * mapWidth + x;
    }

    public Vector3 GetTilePos(int y, int x)
    {
        var pos = firstTilePos;
        pos.x += tileSize.x * x;
        pos.y -= tileSize.y * y;
        return pos;
    }

    public Vector3 GetTilePos(int tileId)
    {
        return GetTilePos(tileId / mapWidth, tileId % mapWidth);
    }

    [ContextMenu("Search")]
    public void Search()
    {
        var search = new TileSearch();
        search.Init(map);

        var targetId = ScreenPosToTileId(Input.mousePosition);

        switch (algorithmType)
        {
            case AlgorithmType.DFS:
                search.DFS(map.tiles[map.startTile.id]);
                break;
            case AlgorithmType.BFS:
                search.BFS(map.tiles[map.startTile.id]);
                break;
            case AlgorithmType.DFS_Recursive:
                search.DFS_Recursive(map.tiles[map.startTile.id]);
                break;
            case AlgorithmType.PathFindingBFS:
                search.PathFind(map.tiles[map.startTile.id], map.tiles[targetId]);
                break;
            case AlgorithmType.Dikjstra:
                search.Dikjstra(map.tiles[map.startTile.id], map.tiles[targetId]);
                break;
            case AlgorithmType.AStar:
                search.Astar(map.tiles[map.startTile.id], map.tiles[targetId]);
                break;
        }
    }

    private void ResetStage()
    {
        bool succeed = false;
        while (!succeed)
        {
            map = new Map();
            map.Init(mapHeight, mapWidth);
            succeed = map.CreateIsland(erodePercent, erodeIteration, lakePercent,
                treePercent, hillPercent, moutainPercent, townPercent, monsterPercent);
        }
        CreateGrid();
        CreatePlayer();

    }
    private void CreatePlayer()
    {
        if (player != null)
        {
            Destroy(player);
        }
        player = Instantiate(playerPrefab, GetTilePos(map.startTile.id), Quaternion.identity);
    }

    private void CreateGrid()
    {
        if (tileObjs != null)
        {
            foreach (var tile in tileObjs)
            {
                Destroy(tile.gameObject);
            }
        }
        tileObjs = new GameObject[mapHeight * mapWidth];

        firstTilePos = Vector3.zero;
        firstTilePos.x -= mapWidth * tileSize.x * 0.5f;
        firstTilePos.y += mapHeight * tileSize.y * 0.5f;
        var pos = firstTilePos;
        for (int i = 0; i < mapHeight; ++i)
        {
            for (int j = 0; j < mapWidth; ++j)
            {
                var tileId = i * mapWidth + j;
                var tile = map.tiles[tileId];

                var newGo = Instantiate(tilePrefab, transform);
                newGo.transform.localPosition = pos;
                pos.x += tileSize.x;
                newGo.name = $"Tile ({i} , {j})";
                tileObjs[tileId] = newGo;
                DecorateTile(tileId);
            }
            pos.x = firstTilePos.x;
            pos.y -= tileSize.y;
        }
    }

    public void DecorateTile(int tileId)
    {
        var tile = map.tiles[tileId];
        var tileGo = tileObjs[tileId];
        var ren = tileGo.GetComponent<SpriteRenderer>();
        if (tile.autoTileId != (int)TileTypes.Empty)
        {
            Debug.Log($"Count: {islandSprites.Count()}, AutoTileID: {tile.autoTileId}");
            ren.sprite = islandSprites[tile.autoTileId];
        }
        else
        {
            ren.sprite = null;
        }

        //if (tile.isVisited)
        //{
        //    if (tile.autoTileId != (int)TileTypes.Empty)
        //    {
        //        ren.sprite = islandSprites[tile.autoTileId];
        //    }
        //    else
        //    {
        //        ren.sprite = null;
        //    }
        //}
        //else
        //{
        //    ren.sprite = fowSprites[tile.autoFowId];
        //}
    }

    public int visiteRadius = 1;

    public void OnTileVisited(Tile tile)
    {
        int centerX = tile.id % mapWidth;
        int centerY = tile.id / mapWidth;

        int radius = visiteRadius;
        for (int i = -radius; i <= radius; ++i)
        {
            for (int j = -radius; j <= radius; ++j)
            {
                int x = centerX + j;
                int y = centerY + i;
                if (x < 0 || x >= mapWidth || y < 0 || y >= mapHeight)
                    continue;

                int id = y * mapWidth + x;
                map.tiles[id].isVisited = true;
                DecorateTile(id);
            }
        }
        radius += 1;
        for (int i = -radius; i <= radius; ++i)
        {
            for (int j = -radius; j <= radius; ++j)
            {
 
                if (i == radius || i == -radius || j == radius || j == -radius)
                {
                    int x = centerX + j;
                    int y = centerY + i;
                    if (x < 0 || x >= mapWidth || y < 0 || y >= mapHeight)
                        continue;

                    int id = y * mapWidth + x;
                    map.tiles[id].UpdateAuotoFowId();
                    DecorateTile(id);
                }
            }
        }
    }

    private Camera cam;
    private CancellationTokenSource cts;

    private void Awake()
    {
        cam = Camera.main;
    }
    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log(ScreenPosToTileId(Input.mousePosition));
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            ResetStage();
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (map == null) return;
            var targetId = ScreenPosToTileId(Input.mousePosition);
            var targetTile = map.tiles[targetId];
            PlayerMoveToTile(targetTile);
        }
    }

    private void ClearPathColor()
    {
        foreach (var tileObj in tileObjs)
        {
            var ren = tileObj.GetComponent<SpriteRenderer>();
            ren.color = Color.white;
        }
    }

    private void PlayerMoveToTile(Tile tile)
    {
        var search = new TileSearch();
        search.Init(map);

        if (search.Astar(map.tiles[map.startTile.id], tile))
        {
            //StopAllCoroutines();
            cts?.Cancel();
            //cts?.Dispose();
            cts = new CancellationTokenSource();
            ClearPathColor();
            MovePlayerAlongPathColor(search, Color.red);
            MovePlayerAlongPathWithUniTask(search, cts).Forget();
            //StartCoroutine(MovePlayerAlongPath(search));
        }
    }

    private void OnDestroy()
    {
        cts?.Cancel();
        cts?.Dispose();
    }

    private void MovePlayerAlongPathColor(TileSearch tsh, Color c)
    {
        foreach (var tile in tsh.path)
        {
            var tileObj = tileObjs[tile.id];
            var ren = tileObj.GetComponent<SpriteRenderer>();
            ren.color = c;
        }
    }

    private IEnumerator MovePlayerAlongPath(TileSearch tsh)
    {
        var path = tsh.path;
        for (int i = 0; i < path.Count; ++i)
        {
            var tile = path[i];
            var targetPos = GetTilePos(tile.id);
            while (Vector3.Distance(player.transform.position, targetPos) > 0.1f)
            {
                player.transform.position = Vector3.MoveTowards(player.transform.position, targetPos, Time.deltaTime * 50f);
                yield return null;
            }
            player.transform.position = targetPos;
            map.startTile = tile;
            OnTileVisited(tile);
            yield return null;
        }
        MovePlayerAlongPathColor(tsh, Color.white);
    }

    private async UniTaskVoid MovePlayerAlongPathWithUniTask(TileSearch ts, CancellationTokenSource cts)
    {
        var path = ts.path;
        var token = cts.Token;
        for (int i = 0; i < path.Count; ++i)
        {
            if (token.IsCancellationRequested)
                return;
            var tile = path[i];
            var targetPos = GetTilePos(tile.id);
            while (!token.IsCancellationRequested && Vector3.Distance(player.transform.position, targetPos) > 0.1f)
            {
                player.transform.position = Vector3.MoveTowards(player.transform.position, targetPos, Time.deltaTime * 50f);
                await UniTask.Yield();
            }
            if (token.IsCancellationRequested || player == null) return;
            player.transform.position = targetPos;
            map.startTile = tile;
            OnTileVisited(tile);
            await UniTask.Yield();
        }
        MovePlayerAlongPathColor(ts, Color.white);
    }
}

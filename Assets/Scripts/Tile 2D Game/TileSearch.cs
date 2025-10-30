using System.Collections.Generic;
using UnityEngine;

public class TileSearch
{
    private Map map;

    public List<Tile> path = new List<Tile>();

    public void Init(Map map)
    {
        this.map = map;
    }

    public bool PathFindingBFS(Tile start, Tile end)
    {
        var visited = new HashSet<Tile>();
        var queue = new Queue<Tile>();

        visited.Add(start);

        queue.Enqueue(start);
        bool found = false;
        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            if (current == end)
            {
                found = true;
                break;
            }

            foreach (var neighbor in current.adjacents)
            {
                if (neighbor == null || !neighbor.CanMove) continue;
                if (visited.Contains(neighbor)) continue;

                visited.Add(neighbor);
                neighbor.previous = current;
                queue.Enqueue(neighbor);
            }
        }
        path.Clear();
        if (found)
        {
            var current = end;
            while (current != null)
            {
                path.Add(current);
                current = current.previous;
            }
            path.Reverse();
            return true;
        }
        return false;
    }

    public void PathFind(Tile start, Tile end)
    {
        var visited = new HashSet<Tile>();
        var queue = new Queue<Tile>();
        var parentMap = new Dictionary<Tile, Tile>();
        visited.Add(start);

        queue.Enqueue(start);
        bool found = false;
        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            if (current == end)
            {
                found = true;
                break;
            }

            foreach (var neighbor in current.adjacents)
            {
                if (neighbor == null || !neighbor.CanMove) continue;
                if (visited.Contains(neighbor)) continue;

                visited.Add(neighbor);
                parentMap[neighbor] = current;
                queue.Enqueue(neighbor);
            }
        }
        path.Clear();
        if (found)
        {
            var current = end;
            while (current != null)
            {
                path.Add(current);
                parentMap.TryGetValue(current, out current);
            }
            path.Reverse();
        }
    }

    public void DFS_Recursive(Tile node)
    {
        path.Clear();
        var visited = new HashSet<Tile>();
        DFS_Recursive(node, visited);
    }


    public void DFS_Recursive(Tile node, HashSet<Tile> visited)
    {
        if (node == null || !node.CanMove) return;
        visited.Add(node);
        path.Add(node);
        foreach (var neighbor in node.adjacents)
        {
            if (neighbor == null || !neighbor.CanMove) continue;
            if (visited.Contains(neighbor)) continue;
            DFS_Recursive(neighbor, visited);
        }
    }

    public void DFS(Tile node)
    {
        path.Clear();
        var visited = new HashSet<Tile>();
        var stack = new Stack<Tile>();

        visited.Add(node);
        stack.Push(node);

        while (stack.Count > 0)
        {
            var current = stack.Pop();
            path.Add(current);
            //visited.Add(current);

            foreach (var neighbor in current.adjacents)
            {
                if (neighbor == null || !neighbor.CanMove) continue;
                if (visited.Contains(neighbor)) continue;

                visited.Add(neighbor);
                stack.Push(neighbor);
            }
        }
    }

    public void BFS(Tile node)
    {
        path.Clear();
        var visited = new HashSet<Tile>();
        var queue = new Queue<Tile>();

        visited.Add(node);
        queue.Enqueue(node);
        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            path.Add(current);
            foreach (var neighbor in current.adjacents)
            {
                if (neighbor == null || !neighbor.CanMove) continue;
                if (visited.Contains(neighbor)) continue;

                visited.Add(neighbor);
                queue.Enqueue(neighbor);
            }
        }
    }

    public bool Dikjstra(Tile start, Tile goal)
    {
        path.Clear();
        ResetTilesPrevious();

        var visited = new HashSet<Tile>();
        var pQueue = new PriorityQueue<Tile, int>();
        var distances = new int[map.tiles.Length];
        for (int i = 0; i < distances.Length; ++i)
        {
            distances[i] = int.MaxValue;
        }

        distances[start.id] = start.Weight;
        pQueue.Enqueue(start, distances[start.id]);

        bool success = false;
        while (pQueue.Count > 0)
        {
            var currentNode = pQueue.Dequeue();
            if (visited.Contains(currentNode))
                continue;

            if (currentNode == goal)
            {
                success = true;
                break;
            }

            visited.Add(currentNode);

            foreach (var adjacent in currentNode.adjacents)
            {
                if (!adjacent.CanMove || visited.Contains(adjacent))
                    continue;

                var newDistance = distances[currentNode.id] + adjacent.Weight;
                if (distances[adjacent.id] > newDistance)
                {
                    distances[adjacent.id] = newDistance;
                    adjacent.previous = currentNode;
                    pQueue.Enqueue(adjacent, newDistance);
                }
            }
        }

        if (!success)
        {
            return false;
        }

        Tile step = goal;
        while (step != null)
        {
            path.Add(step);
            step = step.previous;
        }
        path.Reverse();
        return true;
    }

    private void ResetTilesPrevious()
    {
        foreach (var tile in map.tiles)
        {
            tile.previous = null;
        }
    }

    protected int Heuristic(Tile a, Tile b)
    {
        //int ax = a.id % map.cols;
        //int ay = a.id / map.cols;

        //int bx = b.id % map.cols;
        //int by = b.id / map.cols;

        //return Mathf.Abs(ax - bx) + Mathf.Abs(ay - by);
        int ax = a.id % map.cols, ay = a.id / map.cols;
        int bx = b.id % map.cols, by = b.id / map.cols;
        int dx = Mathf.Abs(ax - bx), dy = Mathf.Abs(ay - by);
        return Mathf.Max(dx, dy);
    }
    public bool Astar(Tile start, Tile goal)
    {
        path.Clear();
        ResetTilesPrevious();

        var visited = new HashSet<Tile>();
        var pQueue = new PriorityQueue<Tile, int>();
        var distances = new int[map.tiles.Length];
        var scores = new int[map.tiles.Length];

        for (int i = 0; i < distances.Length; ++i)
        {
            scores[i] = distances[i] = int.MaxValue;
        }

        distances[start.id] = start.Weight;
        scores[start.id] = distances[start.id] + Heuristic(start, goal);
        pQueue.Enqueue(start, scores[start.id]);

        bool success = false;
        while (pQueue.Count > 0)
        {
            var currentNode = pQueue.Dequeue();
            if (visited.Contains(currentNode))
            {
                continue;
            }

            if (currentNode == goal)
            {
                success = true;
                break;
            }

            visited.Add(currentNode);
            foreach (var adjacent in currentNode.adjacents)
            {
                if (adjacent == null) continue;
                if (!adjacent.CanMove || visited.Contains(adjacent))
                {
                    continue;
                }

                var newDistance = distances[currentNode.id] + adjacent.Weight;
                if (distances[adjacent.id] > newDistance)
                {
                    distances[adjacent.id] = newDistance;
                    scores[adjacent.id] = distances[adjacent.id] + Heuristic(adjacent, goal);
                    adjacent.previous = currentNode;
                    pQueue.Enqueue(adjacent, scores[adjacent.id]);
                }
            }
        }


        if (!success)
        {
            return false;
        }

        Tile step = goal;
        while (step != null)
        {
            path.Add(step);
            step = step.previous;
        }
        path.Reverse();
        return true;
    }
}

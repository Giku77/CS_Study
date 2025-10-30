using System.Collections.Generic;
using UnityEngine;

public class GraphSearch 
{
    private Graph graph;
    public List<GraphNode> path = new List<GraphNode>();
    public List<GraphNode> opened = new List<GraphNode>();
    public List<GraphNode> closed = new List<GraphNode>();

    public int[] lastG;                 
    public int[] lastF;                
    public int[] enqueueOrder;         
    public int[] popOrder;              

    private int _enqueueTick = 0;
    private int _popTick = 0;


    public void Init(Graph graph)
    {
        this.graph = graph;

        path.Clear(); opened.Clear(); closed.Clear();

        int n = graph.nodes.Length;
        lastG = new int[n];
        lastF = new int[n];                
        enqueueOrder = new int[n];
        popOrder = new int[n];

        for (int i = 0; i < n; i++)
        {
            lastG[i] = int.MaxValue;
            lastF[i] = int.MaxValue;
            enqueueOrder[i] = -1;
            popOrder[i] = -1;
        }
        _enqueueTick = 0;
        _popTick = 0;
    }

    public bool PathFindingBFS(GraphNode start, GraphNode end)
    {
        var visited = new HashSet<GraphNode>();
        var queue = new Queue<GraphNode>();

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

            foreach (var neighbor in current.neighbors)
            {
                if (neighbor == null || !neighbor.CanVisit) continue;
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

    public void PathFind(GraphNode start, GraphNode end)
    {
        var visited = new HashSet<GraphNode>();
        var queue = new Queue<GraphNode>();
        var parentMap = new Dictionary<GraphNode, GraphNode>();
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
    
            foreach (var neighbor in current.neighbors)
            {
                if (neighbor == null || !neighbor.CanVisit) continue;
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

    public void DFS_Recursive(GraphNode node)
    {
        path.Clear();
        var visited = new HashSet<GraphNode>();
        DFS_Recursive(node, visited);
    }


    public void DFS_Recursive(GraphNode node, HashSet<GraphNode> visited)
    {
        if (node == null || !node.CanVisit) return;
        visited.Add(node);
        path.Add(node);
        foreach (var neighbor in node.neighbors)
        {
            if (neighbor == null || !neighbor.CanVisit) continue;
            if (visited.Contains(neighbor)) continue;
            DFS_Recursive(neighbor, visited);
        }
    }

    public void DFS(GraphNode node)
    {
        path.Clear();
        var visited = new HashSet<GraphNode>();
        var stack = new Stack<GraphNode>();

        visited.Add(node);
        stack.Push(node);

        while (stack.Count > 0)
        {
            var current = stack.Pop();
            path.Add(current);
            //visited.Add(current);

            foreach (var neighbor in current.neighbors)
            {
                if (neighbor == null || !neighbor.CanVisit) continue;
                if (visited.Contains(neighbor)) continue;

                visited.Add(neighbor);
                stack.Push(neighbor);
            }
        }
    }

    public void BFS(GraphNode node)
    {
        path.Clear();
        var visited = new HashSet<GraphNode>();
        var queue = new Queue<GraphNode>();

        visited.Add(node);
        queue.Enqueue(node);
        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            path.Add(current);
            foreach (var neighbor in current.neighbors)
            {
                if (neighbor == null || !neighbor.CanVisit) continue;
                if (visited.Contains(neighbor)) continue;

                visited.Add(neighbor);  
                queue.Enqueue(neighbor);
            }
            //foreach (var neighbor in current.neighbors)
            //{
            //    if (!neighbor.CanVisit || visited.Contains(neighbor) || queue.Contains(neighbor))
            //        continue;
            //    if (!visited.Contains(neighbor))
            //    {
            //        visited.Add(neighbor);
            //        queue.Enqueue(neighbor);
            //    }
            //}
        }
    }

    public bool Dikjstra(GraphNode start, GraphNode goal)
    {
        path.Clear();
        opened.Clear();
        closed.Clear();
        graph.ResetNodePrevious();

        var visited = new HashSet<GraphNode>();
        var pQueue = new PriorityQueue<GraphNode, int>();
        var distances = new int[graph.nodes.Length];
        for (int i = 0; i < distances.Length; ++i)
        {
            distances[i] = int.MaxValue;
        }

        distances[start.id] = start.weight;
        lastG[start.id] = distances[start.id];
        pQueue.Enqueue(start, distances[start.id]);
        opened.Add(start);
        if (enqueueOrder[start.id] < 0) enqueueOrder[start.id] = _enqueueTick++;

        bool success = false;
        while (pQueue.Count > 0)
        {
            var currentNode = pQueue.Dequeue();
            if (visited.Contains(currentNode))
                continue;

            closed.Add(currentNode);
            popOrder[currentNode.id] = _popTick++;

            if (currentNode == goal)
            {
                success = true;
                break;
            }

            visited.Add(currentNode);

            foreach (var adjacent in currentNode.neighbors)
            {
                if (!adjacent.CanVisit || visited.Contains(adjacent))
                    continue;

                //Debug.Log($"d : {distances[currentNode.id]}");
                var newDistance = distances[currentNode.id] + adjacent.weight;
                //Debug.Log($"nd : {newDistance}");
                if (distances[adjacent.id] > newDistance)
                {
                    distances[adjacent.id] = newDistance;
                    lastG[adjacent.id] = newDistance;
                    adjacent.previous = currentNode;
                    pQueue.Enqueue(adjacent, newDistance);
                    Debug.Log($"Enqueue : {adjacent.id} with dist {newDistance}");

                    if (!opened.Contains(adjacent))
                        opened.Add(adjacent);
                    if (enqueueOrder[adjacent.id] < 0) enqueueOrder[adjacent.id] = _enqueueTick++;
                }
            }
        }

        if (!success)
        {
            return false;
        }

        GraphNode step = goal;
        while (step != null)
        {
            path.Add(step);
            step = step.previous;
        }
        path.Reverse();
        return true;
    }

    protected int Heuristic(GraphNode a, GraphNode b)
    {
        int ax = a.id % graph.cols;
        int ay = a.id / graph.cols;

        int bx = b.id % graph.cols;
        int by = b.id / graph.cols;

        return Mathf.Abs(ax - bx) + Mathf.Abs(ay - by);
    }
    public bool Astar(GraphNode start, GraphNode goal)
    {
        path.Clear();
        graph.ResetNodePrevious();

        var visited = new HashSet<GraphNode>();
        var pQueue = new PriorityQueue<GraphNode, int>();
        var distances = new int[graph.nodes.Length];
        var scores = new int[graph.nodes.Length];

        for (int i = 0; i < distances.Length; ++i)
        {
            scores[i] = distances[i] = int.MaxValue;
        }

        distances[start.id] = start.weight;
        scores[goal.id] = distances[start.id] + Heuristic(start, goal);
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
            foreach (var adjacent in currentNode.neighbors)
            {
                if (!adjacent.CanVisit || visited.Contains(adjacent))
                {
                    continue;
                }

                var newDistance = distances[currentNode.id] + adjacent.weight;
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

        GraphNode step = goal;
        while (step != null)
        {
            path.Add(step);
            step = step.previous;
        }
        path.Reverse();
        return true;
    }
}

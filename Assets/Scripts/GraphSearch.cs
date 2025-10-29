using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class GraphSearch 
{
    private Graph graph;
    public List<GraphNode> path = new List<GraphNode>();


    public void Init(Graph graph)
    {
        this.graph = graph;
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
}

using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class GraphNode
{
    public int id;
    public int weight = 1;

    public List<GraphNode> neighbors = new List<GraphNode>();

    public GraphNode previous = null;

    public bool CanVisit => neighbors.Count > 0;
}

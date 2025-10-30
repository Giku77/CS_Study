using System.Collections.Generic;
using UnityEngine;

public class GraphTest : MonoBehaviour
{
    public UIGraphNode nodePrefab;
    public List<UIGraphNode> uiNodes;

    public Transform uiNodeRoot;

    private Graph graph;

    public enum AlgorithmType
    {
        DFS,
        BFS,
        DFS_Recursive,
        PathFindingBFS,
        Dikjstra,
        AStar
    }

    private void Start()
    {
        //int[,] map = new int[5, 5]
        //{
        //    { 1, -1, 1, 3, 1 },
        //    { 1, -1, 1, 1, 1 },
        //    { 1, -1, 8, 5, 1 },
        //    { 1, -1, 3, 1, 1 },
        //    { 1, 1, 1, 1, 1 },
        //};

        int[,] map = new int[10, 10]
        {
            { 1, -1, 1, 3, 1, 1, -1, 1, 1, 1  },
            { 1, -1, 1, 3, 1, 1, 1, 5, 1, 1  },
            { 1, -1, 1, 3, 1, 1,1, 1, 1, 1  },
            { 1, -1, 1, 3, 1, 1, 1, 1, 1, 1  },
            { 1, -1, 1, 3, 1, 1, 10, 1, 1, 1  },
            { 1, -1, 1, 7, 1, 1, 9, 1, 1, 1  },
            { 1, 1, 1, 7, 7, 1, 10, 1, 1, 1  },
            { 1, 1, 1, 1, 1, 1, -1, 1, 1, 1  },
            { 1, 1, 1, 1, 1, 1, -1, 1, 1, 1  },
            { 1, 1, 1, 1, 1, 1, -1, 1, 1, 1  },

        };

        graph = new Graph();
        graph.Init(map);
        InitUINodes(graph);
    }

    public AlgorithmType algorithmType;
    public int startIndex;
    public int endIndex;

    [ContextMenu("Search")]
    public void Search()
    {
        var search = new GraphSearch();
        search.Init(graph);

        switch (algorithmType)
        {
            case AlgorithmType.DFS:
                search.DFS(graph.nodes[startIndex]);
                break;
            case AlgorithmType.BFS:
                search.BFS(graph.nodes[startIndex]);
                break;
            case AlgorithmType.DFS_Recursive:
                search.DFS_Recursive(graph.nodes[startIndex]);
                break;
            case AlgorithmType.PathFindingBFS:
                search.PathFind(graph.nodes[startIndex], graph.nodes[endIndex]);
                break;
            case AlgorithmType.Dikjstra:
                search.Dikjstra(graph.nodes[startIndex], graph.nodes[endIndex]);
                break;
            case AlgorithmType.AStar:
                search.Astar(graph.nodes[startIndex], graph.nodes[endIndex]);
                break;
        }
        ResetUINodes();

        //foreach (var n in search.opened)
        //    uiNodes[n.id].SetColor(new Color(0.4f, 0.6f, 1f));

        //foreach (var n in search.closed)
        //    uiNodes[n.id].SetColor(new Color(0.7f, 0.7f, 0.7f));

        //for (int i = 0; i < search.path.Count; i++)
        //{
        //    var node = search.path[i];
        //    var uiNode = uiNodes[node.id];
        //    uiNode.SetColor(Color.Lerp(Color.red, Color.green, (float)i / search.path.Count));
        //    uiNode.SetText($"ID: {node.id.ToString()} \nWeight: {node.weight} \nPath: {i}");
        //}

        foreach (var n in search.opened)
        {
            var ui = uiNodes[n.id];
            ui.SetColor(new Color(0.4f, 0.6f, 1f));
            string gTxt = (search.lastG[n.id] == int.MaxValue) ? "¡Ä" : search.lastG[n.id].ToString();
            string qTxt = (search.enqueueOrder[n.id] >= 0) ? search.enqueueOrder[n.id].ToString() : "-";
            ui.SetFontSize(12);
            ui.AppendText($"\n[OPEN] \nG:{gTxt} Q#{qTxt}");
        }

        foreach (var n in search.closed)
        {
            var ui = uiNodes[n.id];
            ui.SetColor(new Color(0.7f, 0.7f, 0.7f));
            string gTxt = (search.lastG[n.id] == int.MaxValue) ? "¡Ä" : search.lastG[n.id].ToString();
            string pTxt = (search.popOrder[n.id] >= 0) ? search.popOrder[n.id].ToString() : "-";
            ui.SetFontSize(12);
            ui.AppendText($"\n[CLOSED] \nG:{gTxt} P#{pTxt}");
        }

        for (int i = 0; i < search.path.Count; i++)
        {
            var node = search.path[i];
            var uiNode = uiNodes[node.id];
            uiNode.SetColor(Color.Lerp(Color.red, Color.green, (float)i / search.path.Count));

            string gTxt = (search.lastG[node.id] == int.MaxValue) ? "¡Ä" : search.lastG[node.id].ToString();
            uiNode.SetFontSize(18);
            uiNode.SetText($"ID:{node.id}\nWeight:{node.weight}\nG:{gTxt}\nPath:{i}");
        }
    }

    private void InitUINodes(Graph graph)
    {
        uiNodes = new List<UIGraphNode>();
        foreach (var node in graph.nodes)
        {
            var uiNode = Instantiate(nodePrefab, uiNodeRoot);
            uiNode.SetNode(node);
            uiNode.Reset();
            uiNodes.Add(uiNode);
        }
    }

    private void ResetUINodes()
    {
        foreach (var uiNode in uiNodes)
        {
            uiNode.Reset();
        }
    }
}

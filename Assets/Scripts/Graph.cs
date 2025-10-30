using UnityEngine;

public class Graph
{
    public int rows;
    public int cols;

    public GraphNode[] nodes;

    public void ResetNodePrevious()
    {
        foreach (var node in nodes)
        {
            node.previous = null;
        }
    }

    public void Init(int[,] grid)
    {
        rows = grid.GetLength(0);
        cols = grid.GetLength(1);

        nodes = new GraphNode[rows * cols];
        for (int i = 0; i < nodes.Length; i++)
        {
            nodes[i] = new GraphNode();
            nodes[i].id = i;
        }

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                int index = r * cols + c;
                nodes[index].weight = grid[r, c];

                if (grid[r, c] == -1)
                    continue;

                GraphNode node = nodes[index];

                //if (r - 1 >= 0 && grid[r - 1, c] >= 0)
                //{
                //   nodes[index].neighbors.Add(nodes[index - cols]);
                //}

                //if (r + 1 < rows && grid[r + 1, c] >= 0)
                //{
                //    nodes[index].neighbors.Add(nodes[index + cols]);
                //}

                //if (c - 1 >= 0 && grid[r, c - 1] >= 0)
                //{
                //    nodes[index].neighbors.Add(nodes[index - 1]);
                //}

                //if (c + 1 < cols && grid[r, c + 1] >= 0)
                //{
                //    nodes[index].neighbors.Add(nodes[index + 1]);
                //}   


                // Up
                if (r > 0)
                {
                    node.neighbors.Add(nodes[(r - 1) * cols + c]);
                }
                // Down
                if (r < rows - 1)
                {
                    node.neighbors.Add(nodes[(r + 1) * cols + c]);
                }
                // Left
                if (c > 0)
                {
                    node.neighbors.Add(nodes[r * cols + (c - 1)]);
                }
                // Right
                if (c < cols - 1)
                {
                    node.neighbors.Add(nodes[r * cols + (c + 1)]);
                }
            }
        }
    }
}

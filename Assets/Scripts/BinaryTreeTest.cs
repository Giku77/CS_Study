using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class BinaryTreeTest : MonoBehaviour
{
    public BinaryTreeVisualizer treeVisualizer;
    private VisualizableBST<int, string> tree;
    private List<int> insertedKeys = new List<int>();

    [SerializeField] private int nodeCount = 10;
    [SerializeField] private int minKey = 1;
    [SerializeField] private int maxKey = 1000;

    private void Start()    
    {
        GenerateRandomTree();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            int index = Random.Range(0, insertedKeys.Count);
            tree.Remove(insertedKeys[index]);
            Debug.Log($"Removed key: {insertedKeys[index]}");
            treeVisualizer.VisualizeTree(tree);
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            tree[insertedKeys[0]] = "Modified Value";
            Debug.Log($"Modified key: {insertedKeys[0]}");
            treeVisualizer.VisualizeTree(tree);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            int newKey;
            do
            {
                newKey = Random.Range(minKey, maxKey + 1);
            } while (tree.ContainsKey(newKey));
            string newValue = $"V-{newKey}";
            tree.Add(newKey, newValue);
            insertedKeys.Add(newKey);
            Debug.Log($"Added key: {newKey} / {tree.Count}");
            var leftcount = tree.GetLeftCount(tree.GetRoot());
            var rightcount = tree.GetRightCount(tree.GetRoot());
            Debug.Log($"Left Count: {leftcount} / Right Count: {rightcount}");
            treeVisualizer.VisualizeTree(tree);
            //RegenerateTree();
        }
    }

    public void GenerateRandomTree()
    {
        tree = new VisualizableBST<int, string>();
        insertedKeys.Clear();
        int addedNodes = 0;
        while (addedNodes < nodeCount)
        {
            int key = Random.Range(minKey, maxKey + 1);
            insertedKeys.Add(key);

            if (!tree.ContainsKey(key))
            {
                string value = $"V-{key}";
                tree.Add(key, value);
                addedNodes++;
            }
        }
        var leftcount = tree.GetLeftCount(tree.GetRoot());
        var rightcount = tree.GetRightCount(tree.GetRoot());
        Debug.Log($"Left Count: {leftcount} / Right Count: {rightcount}");

        var leftheight = tree.GetLeftHeight(tree.GetRoot());
        var rightheight = tree.GetRightHeight(tree.GetRoot());
        Debug.Log($"Left Height: {leftheight} / Right Height: {rightheight}");

        treeVisualizer.VisualizeTree(tree);
    }

    [ContextMenu("Generate New Random Tree")]
    public void RegenerateTree()
    {
        GenerateRandomTree();
    }
}
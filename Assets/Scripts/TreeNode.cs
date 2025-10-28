using UnityEngine;

public class TreeNode<Tkey, TValue>
{
    public Tkey Key { get; set; }
    public TValue Value { get; set; }

    public int Height { get; set; }

    public TreeNode<Tkey, TValue> Left { get; set; }
    public TreeNode<Tkey, TValue> Right { get; set; }

    public TreeNode(Tkey key, TValue value)
    {
        Key = key;
        Value = value;
        Left = null;
        Right = null;
        Height = 1;
    }

}

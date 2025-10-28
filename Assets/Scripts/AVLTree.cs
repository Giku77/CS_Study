using UnityEngine;

public class AVLTree<TKey, TValue> : BinarySearchTree<TKey, TValue> where TKey : System.IComparable<TKey>
{
    public AVLTree() : base()
    {
    }

    public int GetLeftCount(TreeNode<TKey, TValue> node)
    {
        if (node == null || node.Left == null)
        {
            return 0;
        }
        return CountNodes(node.Left);
    }

    public int GetLeftHeight(TreeNode<TKey, TValue> node)
    {
        if (node == null || node.Left == null)
        {
            return 0;
        }
        return Height(node.Left);
    }
    public int GetRightHeight(TreeNode<TKey, TValue> node)
    {
        if (node == null || node.Right == null)
        {
            return 0;
        }
        return Height(node.Right);
    }

    public int GetRightCount(TreeNode<TKey, TValue> node)
    {
        if (node == null || node.Right == null)
        {
            return 0;
        }
        return CountNodes(node.Right);
    }

    protected override TreeNode<TKey, TValue> Add(TreeNode<TKey, TValue> node, TKey key, TValue value)
    {
        node = base.Add(node, key, value);
        return Balance(node);
    }

    protected override TreeNode<TKey, TValue> AddOrUpdate(TreeNode<TKey, TValue> node, TKey key, TValue value)
    {
        node = base.AddOrUpdate(node, key, value);
        return Balance(node);
    }

    protected override TreeNode<TKey, TValue> Remove(TreeNode<TKey, TValue> node, TKey key)
    {
        node = base.Remove(node, key);
        if (node == null)
        {
            return null;
        }
        return Balance(node);
    }

    protected int BalanceFactor(TreeNode<TKey, TValue> node)
    {
        return node == null ? 0 : Height(node.Left) - Height(node.Right);
    }

    protected TreeNode<TKey, TValue> Balance(TreeNode<TKey, TValue> node)
    {
        int balance = BalanceFactor(node);
        if (balance > 1)
        {
            if (BalanceFactor(node.Left) < 0)
            {
                node.Left = LeftRotate(node.Left);
            }
            return RightRotate(node);
        }
        if (balance < -1)
        {
            if (BalanceFactor(node.Right) > 0)
            {
                node.Right = RightRotate(node.Right);
            }
            return LeftRotate(node);
        }
        return node;
    }

    protected TreeNode<TKey, TValue> RightRotate(TreeNode<TKey, TValue> y)
    {
        TreeNode<TKey, TValue> x = y.Left;
        TreeNode<TKey, TValue> T2 = x.Right;
        x.Right = y;
        y.Left = T2;
        UpdateHeight(y);
        UpdateHeight(x);
        return x;
    }

    protected TreeNode<TKey, TValue> LeftRotate(TreeNode<TKey, TValue> x)
    {
        TreeNode<TKey, TValue> y = x.Right;
        TreeNode<TKey, TValue> T2 = y.Left;
        y.Left = x;
        x.Right = T2;
        UpdateHeight(x);
        UpdateHeight(y);
        return y;
    }
}

using System.Collections;
using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class BinarySearchTree<TKey, TValue> : IDictionary<TKey, TValue> where TKey : IComparable<TKey>
{
    protected TreeNode<TKey, TValue> root;

    public BinarySearchTree()
    {
        root = null;
    }


    public TValue this[TKey key]
    {
        get
        {
            if (TryGetValue(key, out TValue value))
            {
                return value;
            }
            throw new KeyNotFoundException("The given key was not present in the BinarySearchTree.");
        }
        set
        {
            root = AddOrUpdate(root, key, value);
        }
    }

    protected virtual TreeNode<TKey, TValue> AddOrUpdate(TreeNode<TKey, TValue> node, TKey key, TValue value)
    {
        if (node == null)
        {
            return new TreeNode<TKey, TValue>(key, value);
        }
        int compareResult = key.CompareTo(node.Key);
        if (compareResult < 0)
        {
            node.Left = AddOrUpdate(node.Left, key, value);
        }
        else if (compareResult > 0)
        {
            node.Right = AddOrUpdate(node.Right, key, value);
        }
        else
        {
            node.Value = value;
        }
        UpdateHeight(node);
        return node;
    }

    public ICollection<TKey> Keys => InOrderTraversal().Select(kvp => kvp.Key).ToList();

    public ICollection<TValue> Values => InOrderTraversal().Select(kvp => kvp.Value).ToList();

    public int Count => CountNodes(root);

    protected virtual int CountNodes(TreeNode<TKey, TValue> node)
    {
        if (node == null)
        {
            return 0;
        }
        return 1 + CountNodes(node.Left) + CountNodes(node.Right);
    }

    public bool IsReadOnly => false;

    public void Add(TKey key, TValue value)
    {
        root = Add(root, key, value);
    }

    protected virtual TreeNode<TKey, TValue> Add(TreeNode<TKey, TValue> node, TKey key, TValue value)
    {
        if (node == null)
        {
            return new TreeNode<TKey, TValue>(key, value);
        }
        int compareResult = key.CompareTo(node.Key);
        if (compareResult < 0)
        {
            node.Left = Add(node.Left, key, value);
        }
        else if (compareResult > 0)
        {
            node.Right = Add(node.Right, key, value);
        }
        else
        {
            throw new ArgumentException("An element with the same key already exists in the BinarySearchTree.");
            //node.Value = value;
        }
        UpdateHeight(node);
        return node;
    }

    public void Add(KeyValuePair<TKey, TValue> item)
    {
        Add(item.Key, item.Value);
    }

    public void Clear()
    {
        root = null;
    }

    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
        return ContainsKey(item.Key);
    }

    public bool ContainsKey(TKey key)
    {
        return TryGetValue(key, out _);
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        foreach (var item in this)
        {
            array[arrayIndex++] = item;
        }
    }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        return InOrderTraversal().GetEnumerator();
    }

    public bool Remove(TKey key)
    {
        int initialCount = Count;
        root = Remove(root, key);
        return Count < initialCount;
        //return Remove(root, key) != null;
    }

    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        return Remove(item.Key);
    }

    protected virtual TreeNode<TKey, TValue> Remove(TreeNode<TKey, TValue> node, TKey key)
    {
        if (node == null)
        {
            return null;
        }
        int compareResult = key.CompareTo(node.Key);
        if (compareResult < 0)
        {
            node.Left = Remove(node.Left, key);
        }
        else if (compareResult > 0)
        {
            node.Right = Remove(node.Right, key);
        }
        else
        {
            if (node.Left == null)
            {
                return node.Right;
            }
            else if (node.Right == null)
            {
                return node.Left;
            }
            else
            {
                TreeNode<TKey, TValue> minRight = FindMin(node.Right);
                node.Key = minRight.Key;
                node.Value = minRight.Value;
                node.Right = Remove(node.Right, minRight.Key);
            }
        }
        UpdateHeight(node);
        return node;
    }

    protected virtual TreeNode<TKey, TValue> FindMin(TreeNode<TKey, TValue> node)
    {
        while (node.Left != null)
        {
            node = node.Left;
        }
        return node;
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        return TryGetValue(root, key, out value);
    }

    protected bool TryGetValue(TreeNode<TKey, TValue> node, TKey key, out TValue value)
    {
        if (node == null)
        {
            value = default(TValue);
            return false;
        }
        int compareResult = key.CompareTo(node.Key);
        if (compareResult < 0)
        {
            return TryGetValue(node.Left, key, out value);
        }
        else if (compareResult > 0)
        {
            return TryGetValue(node.Right, key, out value);
        }
        else
        {
            value = node.Value;
            return true;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public virtual IEnumerable<KeyValuePair<TKey, TValue>> InOrderTraversal()
    {
        return InOrderTraversal(root);
    }

    public virtual IEnumerable<KeyValuePair<TKey, TValue>> PreOrderTraversal()
    {
        return PreOrderTraversal(root);
    }

    public virtual IEnumerable<KeyValuePair<TKey, TValue>> PostOrderTraversal()
    {
        return PostOrderTraversal(root);
    }

    public virtual IEnumerable<KeyValuePair<TKey, TValue>> LevelOrderTraversal()
    {
        return LevelOrderTraversal(root);
    }

    //ÁÂ¡æ³ëµå¡æ¿ì	B, D, E, A, F, C, G
    protected virtual IEnumerable<KeyValuePair<TKey, TValue>> InOrderTraversal(TreeNode<TKey, TValue> node)
    {
        if (node != null)
        {
            foreach (var kvp in InOrderTraversal(node.Left))
            {
                yield return kvp;
            }
            yield return new KeyValuePair<TKey, TValue>(node.Key, node.Value);
            foreach (var kvp in InOrderTraversal(node.Right))
            {
                yield return kvp;
            }
        }
    }

    //³ëµå¡æÁÂ¡æ¿ì	A, B, D, E, C, F, G
    protected virtual IEnumerable<KeyValuePair<TKey, TValue>> PreOrderTraversal(TreeNode<TKey, TValue> node)
    {
        if (node != null)
        {
            yield return new KeyValuePair<TKey, TValue>(node.Key, node.Value);
            foreach (var kvp in PreOrderTraversal(node.Left))
            {
                yield return kvp;
            }
            foreach (var kvp in PreOrderTraversal(node.Right))
            {
                yield return kvp;
            }
        }
    }

    //ÁÂ¡æ¿ì¡æ³ëµå	D, E, B, F, G, C, A
    protected virtual IEnumerable<KeyValuePair<TKey, TValue>> PostOrderTraversal(TreeNode<TKey, TValue> node)
    {
        if (node != null)
        {
            foreach (var kvp in PostOrderTraversal(node.Left))
            {
                yield return kvp;
            }
            foreach (var kvp in PostOrderTraversal(node.Right))
            {
                yield return kvp;
            }
            yield return new KeyValuePair<TKey, TValue>(node.Key, node.Value);
        }
    }

    //³ëµå¡æÁÂ¡æ¿ì	A, B, C, D, E, F, G
    protected virtual IEnumerable<KeyValuePair<TKey, TValue>> LevelOrderTraversal(TreeNode<TKey, TValue> node)
    {   
        if (node == null)
            yield break;
        Queue<TreeNode<TKey, TValue>> queue = new Queue<TreeNode<TKey, TValue>>();
        queue.Enqueue(node);
        while (queue.Count > 0)
        {
            TreeNode<TKey, TValue> current = queue.Dequeue();
            yield return new KeyValuePair<TKey, TValue>(current.Key, current.Value);
            if (current.Left != null)
                queue.Enqueue(current.Left);
            if (current.Right != null)
                queue.Enqueue(current.Right);
        }
    }

    protected virtual void UpdateHeight(TreeNode<TKey, TValue> node)
    {
        if (node != null)
        {
            node.Height = 1 + Math.Max(Height(node.Left), Height(node.Right));
        }
    }

    protected virtual int Height(TreeNode<TKey, TValue> node)
    {
       return node == null ? 0 : node.Height;  
    }
}

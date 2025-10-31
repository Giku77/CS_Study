using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainingHashTable<TKey, TValue> : IDictionary<TKey, TValue>
{
    private LinkedList<KeyValuePair<TKey, TValue>>[] table;
    private const double LoadFactor = 0.75;
    private const int DefaultCapacity = 16;

    private int size;
    private int count;
    public ChainingHashTable(int capacity = DefaultCapacity)
    {
        size = capacity;
        table = new LinkedList<KeyValuePair<TKey, TValue>>[size];
        for (int i = 0; i < size; i++)
        {
            table[i] = new LinkedList<KeyValuePair<TKey, TValue>>();
        }
        count = 0;
    }

    public int GetHashIndex(TKey key)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));
        return (key.GetHashCode() & 0x7FFFFFFF) % size;
    }

    public TValue this[TKey key] { 
        get
        {
            if (TryGetValue(key, out TValue value))
            {
                return value;
            }
            throw new KeyNotFoundException("The given key was not present in the dictionary.");
        }
        set
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if ((double)count / size > LoadFactor)
            {
                Resize();
            }

            int index = GetHashIndex(key);
            var bucket = table[index];
            for (var node = bucket.First; node != null; node = node.Next)
            {
                if (node.Value.Key.Equals(key))
                {
                    node.Value = new KeyValuePair<TKey, TValue>(key, value);
                    return;
                }
            }
            bucket.AddLast(new KeyValuePair<TKey, TValue>(key, value));
            count++;
        }
    }

    public ICollection<TKey> Keys
    {
        get
        {
            var keys = new List<TKey>(count);
            for (int i = 0; i < size; i++)
            {
                var bucket = table[i];
                if (bucket != null)
                {
                    for (var node = bucket.First; node != null; node = node.Next)
                    {
                        keys.Add(node.Value.Key);
                    }
                }
            }
            return keys;
        }
    }

    public ICollection<TValue> Values
    {
        get
        {
            var values = new List<TValue>(count);
            for (int i = 0; i < size; i++)
            {
                var bucket = table[i];
                if (bucket != null)
                {
                    for (var node = bucket.First; node != null; node = node.Next)
                    {
                        values.Add(node.Value.Value);
                    }
                }
            }
            return values;
        }
    }

    public int Count => count;
    public int Size => size;

    public bool IsReadOnly => false;



    public void Add(TKey key, TValue value)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));

        if ((double)count / size > LoadFactor)
        {
            Resize();
        }

        int index = GetHashIndex(key);
        var bucket = table[index];
        if (bucket == null)
        {
            bucket = new LinkedList<KeyValuePair<TKey, TValue>>();
            table[index] = bucket;
        }
        for (var node = bucket.First; node != null; node = node.Next)
        {
            if (node.Value.Key.Equals(key))
                throw new ArgumentException("An item with the same key has already been added.");
        }

        bucket.AddLast(new KeyValuePair<TKey, TValue>(key, value));
        count++;
    }

    public void Resize()
    {
        int newsize = size * 2;
        var newtable = new LinkedList<KeyValuePair<TKey, TValue>>[newsize];

        for (int i = 0; i < size; i++)
        {
            var bucket = table[i];
            if (bucket != null)
            {
                foreach (var kvp in bucket)
                {
                    int newindex = (kvp.Key.GetHashCode() & 0x7FFFFFFF) % newsize;
                    if (newtable[newindex] == null)
                    {
                        newtable[newindex] = new LinkedList<KeyValuePair<TKey, TValue>>();
                    }
                    newtable[newindex].AddLast(kvp);
                }
            }
        }
        table = newtable;
        size = newsize;
    }

    public void Add(KeyValuePair<TKey, TValue> item)
    {
        Add(item.Key, item.Value);
    }

    public void Clear()
    {
        Array.Clear(table, 0, table.Length);
        count = 0;
    }

    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
        if (TryGetValue(item.Key, out TValue value))
        {
            return EqualityComparer<TValue>.Default.Equals(value, item.Value);
        }
        return false;
    }

    public bool ContainsKey(TKey key)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));

        int index = GetHashIndex(key);
        var bucket = table[index];
        if (bucket != null)
        {
            for (var node = bucket.First; node != null; node = node.Next)
            {
                if (node.Value.Key.Equals(key))
                    return true;
            }
        }
        return false;
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        if (array == null) throw new ArgumentNullException(nameof(array));
        if (arrayIndex < 0) throw new ArgumentOutOfRangeException(nameof(arrayIndex));
        if (array.Length - arrayIndex < count) throw new ArgumentException("Insufficient space");

        int currentIndex = arrayIndex;
        for (int i = 0; i < size; i++)
        {
            var bucket = table[i];
            if (bucket != null)
            {
                for (var node = bucket.First; node != null; node = node.Next)
                {
                    array[currentIndex++] = node.Value;
                }
            }
        }
    }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        for (int i = 0; i < size; i++)
        {
            var bucket = table[i];
            if (bucket != null)
            {
                for (var node = bucket.First; node != null; node = node.Next)
                {
                    yield return node.Value;
                }
            }
        }
    }

    public bool Remove(TKey key)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));

        int index = GetHashIndex(key);
        var bucket = table[index];
        if (bucket != null)
        {
            for (var node = bucket.First; node != null; node = node.Next)
            {
                if (node.Value.Key.Equals(key))
                {
                    bucket.Remove(node);
                    count--;
                    return true;
                }
            }
        }
        return false;
    }

    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
       if (TryGetValue(item.Key, out TValue value))
        {
            if (EqualityComparer<TValue>.Default.Equals(value, item.Value))
            {
                return Remove(item.Key);
            }
        }
        return false;
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));

        int index = GetHashIndex(key);
        var bucket = table[index];
        if (bucket != null)
        {
            for (var node = bucket.First; node != null; node = node.Next)
            {
                if (node.Value.Key.Equals(key))
                {
                    value = node.Value.Value;
                    return true;
                }
            }
        }
        value = default;
        return false;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

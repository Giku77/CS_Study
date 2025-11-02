using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ProbingStrategy
{
    Linear,
    Quadratic,
    DoubleHash
}

public class OpenAddressingHashTable<TKey, TValue> : IDictionary<TKey, TValue>
{
    private const int DefaultCapacity = 16;
    private const double LoadFactor = 0.6;

    private KeyValuePair<TKey, TValue>[] table;
    private bool[] occupied;
    private bool[] deleted;
    private int size;
    private int count;
    private ProbingStrategy probingStrategy;

    public OpenAddressingHashTable(ProbingStrategy strategy = ProbingStrategy.Linear, int capacity = DefaultCapacity)
    {
        probingStrategy = strategy;
        table = new KeyValuePair<TKey, TValue>[capacity];
        occupied = new bool[capacity];
        deleted = new bool[capacity];
        size = capacity;
        count = 0;
    }

    public bool TryGetStoredIndex(TKey key, out int index)
    {
        index = FindIndex(key);
        return index != -1;
    }

    public int GetPrimaryHash(TKey key)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));
        return (key.GetHashCode() & 0x7FFFFFFF) % size;
    }

    public int GetProbeIndex(TKey key, int i)
    {
        int hash1 = GetPrimaryHash(key);
        switch (probingStrategy)
        {
            case ProbingStrategy.Linear:
                return (hash1 + i) % size;
            case ProbingStrategy.Quadratic:
                return (hash1 + i * i) % size;
            case ProbingStrategy.DoubleHash:
                int hash2 = 1 + ((key.GetHashCode() & 0x7FFFFFFF) % (size - 1));
                return (hash1 + i * hash2) % size;
            default:
                throw new System.ArgumentOutOfRangeException();
        }
    }  


    public TValue this[TKey key] 
    { 
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
            int i = 0;
            int index = 0;

            do
            {
                index = GetProbeIndex(key, i);
                if (!occupied[index] || deleted[index])
                {
                    table[index] = new KeyValuePair<TKey, TValue>(key, value);
                    occupied[index] = true;
                    deleted[index] = false;
                    count++;
                    return;
                }
                if (table[index].Key.Equals(key))
                {
                    table[index] = new KeyValuePair<TKey, TValue>(key, value);
                    return;
                }
                i++;

                if (i > size)
                {
                    Resize();
                    i = 0;
                }
            }
            while (true);
        }
    }

    public ICollection<TKey> Keys
    {
        get
        {
            List<TKey> keys = new List<TKey>();
            for (int i = 0; i < size; i++)
            {
                if (occupied[i] && !deleted[i])
                {
                    keys.Add(table[i].Key);
                }
            }
            return keys;
        }
    }
    public ICollection<TValue> Values
    {
        get
        {
            List<TValue> values = new List<TValue>();
            for (int i = 0; i < size; i++)
            {
                if (occupied[i] && !deleted[i])
                {
                    values.Add(table[i].Value);
                }
            }
            return values;
        }
    }

    public int Count => count;
    public int Size => size;

    public bool IsReadOnly => false;

    public void Resize()
    {
        var oldtable = table;
        var oldOccupied = occupied;
        var oldDeleted = deleted;
        var oldSize = size;

        size = size * 2;
        table = new KeyValuePair<TKey, TValue>[size];
        occupied = new bool[size];
        deleted = new bool[size];
        count = 0;

        for (int i = 0; i < oldSize; i++)
        {
            if (oldOccupied[i] && !oldDeleted[i])
            {
                Add(oldtable[i].Key, oldtable[i].Value);
            }
        }
    }

    private int FindIndex(TKey key)
    {
        if (key == null)
            throw new System.ArgumentNullException(nameof(key));

        int i = 0;
        int index = 0;
        do
        {
            index = GetProbeIndex(key, i);
            if (!occupied[index] && !deleted[index])
            {
                return -1; 
            }
            if (occupied[index] && !deleted[index] && table[index].Key.Equals(key))
            {
                return index;
            }
            i++;
        }
        while (i < size);
        return -1;
    }

    public void Add(TKey key, TValue value)
    {
        if (key == null)
            throw new System.ArgumentNullException(nameof(key));

        if ((double)count / size > LoadFactor)
        {
            Resize();
        }

        int i = 0;
        int index = 0;

        do
        {
            index = GetProbeIndex(key, i);
            if (!occupied[index] || deleted[index])
            {
                table[index] = new KeyValuePair<TKey, TValue>(key, value);
                occupied[index] = true;
                deleted[index] = false;
                count++;
                return;
            }
            if (table[index].Key.Equals(key))
            {
                throw new System.ArgumentException("An item with the same key has already been added.");
            }
            i++;

            if (i > size)
            {
                Resize();
                i = 0;
            }
        }
        while (true);
    }

    public void Add(KeyValuePair<TKey, TValue> item)
    {
        Add(item.Key, item.Value);
    }

    public void Clear()
    {
        Array.Clear(table, 0, table.Length);
        Array.Clear(occupied, 0, occupied.Length);
        Array.Clear(deleted, 0, deleted.Length);
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
        return FindIndex(key) != -1;
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        if (array == null) throw new ArgumentNullException(nameof(array));
        if (arrayIndex < 0) throw new ArgumentOutOfRangeException(nameof(arrayIndex));
        if (array.Length - arrayIndex < count) throw new ArgumentException("Insufficient space");

        foreach (var item in this)
        {
            array[arrayIndex++] = item;
        }
    }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        for (int i = 0; i < size; i++)
        {
            if (occupied[i] && !deleted[i])
            {
                yield return table[i];
            }
        }
    }

    public bool Remove(TKey key)
    {
        int index = FindIndex(key);
        if (index != -1)
        {
            deleted[index] = true;
            count--;
            return true;
        }

        return false;
    }

    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        int index = FindIndex(item.Key);
        if (index != -1 && table[index].Value.Equals(item.Value))
        {
            deleted[index] = true;
            count--;
            return true;
        }
        return false;
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        int index = FindIndex(key);
        if (index != -1)
        {
            value = table[index].Value;
            return true;
        }

        value = default;
        return false;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SimpleHashTable<TKey, TValue> : IDictionary<TKey, TValue>
{

    private const int DefaultCapacity = 16;
    private const double LoadFactor = 0.75;


    private KeyValuePair<TKey, TValue>[] table;
    private bool[] occupied;

    private int size;
    private int count;

    public SimpleHashTable()
    {
        size = DefaultCapacity;
        table = new KeyValuePair<TKey, TValue>[size];
        occupied = new bool[size];
        count = 0;
    }


    private int GetIndex(TKey key, int size)
    {
        //int hash = key.GetHashCode() & 0x7FFFFFFF;
        //return hash % size;
        if (key == null)
            throw new ArgumentNullException(nameof(key));
        //int hash = key.GetHashCode();
        int hash = key.GetHashCode() & 0x7FFFFFFF;
        return Mathf.Abs(hash) % size;
    }

    private int GetIndex(TKey key)
    {
       return GetIndex(key, size);
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
            if (key == null)
                throw new ArgumentNullException(nameof(key));
            int index = GetIndex(key);
            if (occupied[index])
            {
                if (table[index].Key.Equals(key))
                {
                    table[index] = new KeyValuePair<TKey, TValue>(key, value);
                }
                else
                {
                    throw new NotImplementedException("Collision handling not implemented.");
                }
            }
            else if (!occupied[index])
            {
                table[index] = new KeyValuePair<TKey, TValue>(key, value);
                occupied[index] = true;
                count++;
                //Add(key, value);
            }
            else
            {
                throw new InvalidOperationException("Unexpected state in indexer set operation.");
            }
        }
    }

    public ICollection<TKey> Keys
    {
        get
        {
            var list = new List<TKey>(count);
            for (int i = 0; i < size; i++)
                if (occupied[i]) list.Add(table[i].Key);
            return list;
        }
    }

    public ICollection<TValue> Values
    {
        get
        {
            var list = new List<TValue>(count);
            for (int i = 0; i < size; i++)
                if (occupied[i]) list.Add(table[i].Value);
            return list;
        }
    }

    public int Count => count;

    public bool IsReadOnly => false;

    public void Add(TKey key, TValue value)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));

        if (count >= size * LoadFactor)
        {
            Resize();
        }

        int index = GetIndex(key);
        if (!occupied[index])
        {
            table[index] = new KeyValuePair<TKey, TValue>(key, value);
            occupied[index] = true;
            count++;
        }
        else if (table[index].Key.Equals(key))
        {
            throw new ArgumentException("An item with the same key has already been added.");
        }
        else
        {
            throw new NotImplementedException("Collision handling not implemented.");
        }

    }

    public void Resize()
    {
        int newSize = size * 2;
        var newTable = new KeyValuePair<TKey, TValue>[newSize];
        var newOccupied = new bool[newSize];

        for (int i = 0; i < size; i++)
        {
            if (occupied[i])
            {
                var kvp = table[i];
                int newIndex = GetIndex(kvp.Key, newSize);
                if (newOccupied[newIndex])
                {
                    throw new NotImplementedException("Collision handling not implemented during resize.");
                }

                newTable[newIndex] = kvp;
                newOccupied[newIndex] = true;
            }
        }
        table = newTable;
        occupied = newOccupied;
        size = newSize;
    }

    public void Add(KeyValuePair<TKey, TValue> item)
    {
        Add(item.Key, item.Value);
    }

    public void Clear()
    {
        Array.Clear(table, 0, table.Length);
        Array.Clear(occupied, 0, occupied.Length);
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
        int index = GetIndex(key);
        return occupied[index] && table[index].Key.Equals(key);
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
            if (occupied[i])
            {
                yield return table[i];
            }
        }
    }

    public bool Remove(TKey key)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));

        int index = GetIndex(key);
        if (occupied[index] && table[index].Key.Equals(key))
        {
            occupied[index] = false;
            table[index] = default;
            count--;
            return true;
        }
        return false;
    }

    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        return Remove(item.Key);
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));
        int index = GetIndex(key);
        if (occupied[index] && table[index].Key.Equals(key))
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

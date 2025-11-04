using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class HashTableTest : MonoBehaviour
{

    public UITable tablePrefab;
    public TMP_Dropdown chooseTable;
    public TMP_Dropdown chooseTableType;

    public TMP_InputField inputKey;
    public TMP_InputField inputValue;

    public TextMeshProUGUI infoText;

    public Transform uiTableRoot;

    private int size = 16;

    private List<UITable> uiTables = new List<UITable>();
    private List<string> keys = new List<string>();

    private OpenAddressingHashTable<string, int> openAddressingHashTable;
    private ChainingHashTable<string, int> chainingHashTable;

    private void AddTestOf100()
    {
        for (int i = 0; i < 100; i++)
        {
            string key = $"key{i}";
            int value = i;
            keys.Add(key);
            switch (chooseTable.value)
            {
                case 0:
                    openAddressingHashTable.Add(key, value);
                    break;
                case 1:
                    chainingHashTable.Add(key, value);
                    break;
            }
        }
        size = chooseTable.value == 0 ? openAddressingHashTable.Size : chainingHashTable.Size;
        DisplayHashTable(chooseTable.value == 0 ? openAddressingHashTable : chainingHashTable);
        UpdateUITable();
    }

    private void Start()
    {
        chooseTable.onValueChanged.AddListener(v =>
        {
            switch (v)
            {
                case 0:
                    keys.Clear();
                    openAddressingHashTable?.Clear();
                    InitOpenAddressingHashTable();
                    DisplayHashTable(openAddressingHashTable);
                    //SetInfoText("Open Addressing Hash Table Initialized");
                    chooseTableType.interactable = true;
                    break;
                case 1:
                    keys.Clear();
                    chainingHashTable?.Clear();
                    InitChainingHashTable();
                    DisplayHashTable(chainingHashTable);
                    //SetInfoText("Chaining Hash Table Initialized");
                    chooseTableType.interactable = false;
                    //AddTestOf100();
                    break;
            }
        });

        InitOpenAddressingHashTable();
        DisplayHashTable(openAddressingHashTable);
        //AddTestOf100();
        //SetInfoText($"Hash Table Initialized. Size : {size}");
    }

    private void SetInfoText(string text)
    {
        infoText.text = text;
    }

    private void InitOpenAddressingHashTable()
    {
        switch (chooseTable.value)
        {
            case 0:
                openAddressingHashTable = new OpenAddressingHashTable<string, int>(strategy: ProbingStrategy.Linear);
                break;
            case 1:
                openAddressingHashTable = new OpenAddressingHashTable<string, int>(strategy: ProbingStrategy.Quadratic);
                break;
            case 2:
                openAddressingHashTable = new OpenAddressingHashTable<string, int>(strategy: ProbingStrategy.DoubleHash);
                break;
        }
        SetInfoText($"Open Addressing Hash Table Initialized. Size {size}");
    }

    private void InitChainingHashTable()
    {
        chainingHashTable = new ChainingHashTable<string, int>();
        SetInfoText($"Chaining Hash Table Initialized. Size {size}");
    }

    public void OnAddButtonClicked()
    {
        string key = inputKey.text;
        int value = int.Parse(inputValue.text);
        if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(inputValue.text))
            return;
        var oldsize = size;

        if (!keys.Contains(key))
        {
            keys.Add(key);
        }

        switch (chooseTable.value)
        {
            case 0:
                openAddressingHashTable.Add(key, value);
                size = openAddressingHashTable.Size;
                if (openAddressingHashTable.TryGetStoredIndex(key, out var index))
                    SetInfoText($"Key: {key} added at index {index}");
                //var index = openAddressingHashTable.GetProbeIndex(key, 0);
                //SetInfoText($"Key: {key} added at index {index}");
                break;
            case 1:
                chainingHashTable.Add(key, value);
                size = chainingHashTable.Size;
                var hashIndex = chainingHashTable.GetHashIndex(key);
                SetInfoText($"Key: {key} added at index {hashIndex}");
                break;
        }

        if (oldsize != size)
        {
            for (int i = oldsize; i < size; i++)
            {
                var uiTable = Instantiate(tablePrefab, uiTableRoot);
                uiTables.Add(uiTable);
                uiTable.SetText($" I : {i}");
            }
        }
        UpdateUITable();
    }

    public void OnRemoveButtonClicked()
    {
        string key = inputKey.text;
        switch (chooseTable.value)
        {
            case 0:
                if (openAddressingHashTable.ContainsKey(key))
                {
                    keys.Remove(key);
                }
                if (openAddressingHashTable.TryGetStoredIndex(key, out var index))
                    SetInfoText($"Key: {key} removed from index {index}");
                //var index = openAddressingHashTable.GetProbeIndex(key, 0);
                //SetInfoText($"Key: {key} removed from index {index}");
                openAddressingHashTable.Remove(key);
                break;
            case 1:
                if (chainingHashTable.ContainsKey(key))
                {
                    keys.Remove(key);
                }
                var hashIndex = chainingHashTable.GetHashIndex(key);
                SetInfoText($"Key: {key} removed from index {hashIndex}");
                chainingHashTable.Remove(key);
                break;
        }
        UpdateUITable();
    }

    public void OnClearButtonClicked()
    {
        switch (chooseTable.value)
        {
            case 0:
                openAddressingHashTable.Clear();
                keys.Clear();
                break;
            case 1:
                chainingHashTable.Clear();
                keys.Clear();
                break;
        }
        SetInfoText("Hash Table Cleared");
        UpdateUITable();
    }

    private void UpdateUITable()
    {
        for (int i = 0; i < uiTables.Count; i++)
        {
            uiTables[i].SetText($" I : {i}");
        }
        switch (chooseTable.value)
        {
            case 0:
                for (int i = 0; i < keys.Count; i++)
                {
                    if (openAddressingHashTable.TryGetStoredIndex(keys[i], out var storedIndex))
                    {
                        uiTables[storedIndex].AppendText($" K: {keys[i]} V: {openAddressingHashTable[keys[i]]}");
                    }
                }
                break;
            case 1:
                for (int i = 0; i < keys.Count; i++)
                {
                    var itemIndex = chainingHashTable.GetHashIndex(keys[i]);
                    if (uiTables[itemIndex].text.text.Contains("K:"))
                        uiTables[itemIndex].SetFontSize(18);
                    uiTables[itemIndex].AppendText($" K: {keys[i]} V: {chainingHashTable[keys[i]]}");
                }
                break;
        }
    }

    private void DisplayHashTable(IEnumerable<KeyValuePair<string, int>> hashTable)
    {
        if (uiTables.Count > 0)
        {
            foreach (var uiTable in uiTables)
            {
                Destroy(uiTable.gameObject);
            }
            uiTables.Clear();
        }
        for (int i = 0; i < size; i++)
        {
            var uiTable = Instantiate(tablePrefab, uiTableRoot);
            uiTables.Add(uiTable);
            uiTables[i].SetText($" I : {i}");
        }
    }



    //private void Start()
    //{
    //    var hashTable = new SimpleHashTable<string, int>();

    //    hashTable.Add("one", 1);
    //    hashTable.Add("two", 2);
    //    hashTable.Add("three", 3);

    //    foreach (var key in hashTable.Keys)
    //    {
    //        Debug.Log($"Key: {key}, Value: {hashTable[key]}");
    //    }

    //    Debug.Log($"Contains key 'two': {hashTable.ContainsKey("two")}");
    //    hashTable.Remove("two");
    //    Debug.Log($"Contains key 'two' after removal: {hashTable.ContainsKey("two")}");

    //    var array = new KeyValuePair<string, int>[5];
    //    hashTable.CopyTo(array, 0);
    //    Debug.Log("Copied to array:");
    //    foreach (var kvp in array)
    //    {
    //        if (kvp.Key != null)
    //            Debug.Log($"Copied Key: {kvp.Key}, Copied Value: {kvp.Value}");
    //    }
    //    hashTable.Clear();
    //    Debug.Log($"Count after clear: {hashTable.Count}");

    //    for (int i = 0; i < 100; i++)
    //    {
    //        hashTable.Add($"key{i}", i);
    //    }
    //    Debug.Log($"Count after adding 100 items: {hashTable.Count}");


    //    foreach (var key in hashTable.Keys)
    //    {
    //        Debug.Log($"Key: {key}, Value: {hashTable[key]}");
    //    }
    //}
}

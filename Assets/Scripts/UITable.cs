using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class UITable : MonoBehaviour
{
    public Image image;
    public TextMeshProUGUI text;

    private IEnumerable<KeyValuePair<string, int>> hashTable;

    public void Reset()
    {
        //SetColor(node.CanVisit ? Color.white : Color.gray);
        //SetText($"ID: {node.id.ToString()} \nWeight: {node.weight}");
    }

    public void SetData(IEnumerable<KeyValuePair<string, int>> hashTable)
    {
        this.hashTable = hashTable;
    }

    public void SetColor(Color color)
    {
        image.color = color;
    }

    public void SetText(string text)
    {
        this.text.text = text;
    }

    public void AppendText(string text)
    {
        this.text.text += text;
    }

    public void SetFontSize(int size)
    {
        text.fontSize = size;
    }
}

using System.Collections.Generic;
using UnityEngine.Rendering;

public interface ISortStrategy<T>
{
    void Sort(T[] list);
}

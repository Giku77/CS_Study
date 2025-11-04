using System.Collections.Generic;

public class SelectionSortStrategy<T> : ISortStrategy<T>
{
    public void Sort(T[] list)
    {
        for (int i = 0; i < list.Length - 1; i++)
        {
            int minIndex = i;
            for (int j = i + 1; j < list.Length; j++)
            {
                if (Comparer<T>.Default.Compare(list[j], list[minIndex]) < 0)
                {
                    minIndex = j;
                }
            }
            if (minIndex != i)
            {
                T temp = list[i];
                list[i] = list[minIndex];
                list[minIndex] = temp;
            }
        }
    }
}

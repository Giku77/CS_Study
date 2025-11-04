using System.Collections.Generic;

public class BubbleSortStrategy<T> : ISortStrategy<T>
{
    public void Sort(T[] list)
    {
        for (int i = 0; i < list.Length - 1; i++)
        {
            for (int j = 0; j < list.Length - 1 - i; j++)
            {
                if (Comparer<T>.Default.Compare(list[j], list[j + 1]) > 0)
                {
                    T temp = list[j];
                    list[j] = list[j + 1];
                    list[j + 1] = temp;
                }
            }
        }
    }
}

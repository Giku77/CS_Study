using System.Collections.Generic;
public class InsertionSortStrategy<T> : ISortStrategy<T>
{
    public void Sort(T[] list)
    {
        for (int i = 1; i < list.Length; i++)
        {
            T key = list[i];
            int j = i - 1;

            while (j >= 0 && Comparer<T>.Default.Compare(list[j], key) > 0)
            {
                list[j + 1] = list[j];
                j--;
            }
            list[j + 1] = key;
        }
    }
}

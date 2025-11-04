using System.Collections.Generic;

public class HeapSortStrategy<T> : ISortStrategy<T>
{
    private void Swap(T[] array, int i, int j)
    {
        T temp = array[i];
        array[i] = array[j];
        array[j] = temp;
    }

    public void Sort(T[] list)
    {
        var n = list.Length;
        for (int i = n / 2 - 1; i >= 0; i--)
            Heapify(list, n, i);

        for (int i = n - 1; i > 0; i--)
        {
            Swap(list, 0, i);
            Heapify(list, i, 0);
        }
    }

    private void Heapify(T[] array, int n, int i)
    {
        int largest = i; 
        int left = 2 * i + 1; 
        int right = 2 * i + 2; 

        if (left < n && Comparer<T>.Default.Compare(array[left], array[largest]) > 0)
            largest = left;

        if (right < n && Comparer<T>.Default.Compare(array[right], array[largest]) > 0)
            largest = right;

        if (largest != i)
        {
            Swap(array, i, largest);
            Heapify(array, n, largest);
        }
    }   
}

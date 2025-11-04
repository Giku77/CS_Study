using System.Collections.Generic;

public class QuickSortStrategy<T> : ISortStrategy<T>
{

    private IComparer<T> _comparer;
    public QuickSortStrategy()
    {
    }

    public QuickSortStrategy(IComparer<T> comparer)
    {
        _comparer = comparer;
    }

    public void Sort(T[] list)
    {
        QuickSort(list, 0, list.Length - 1);
    }

    private void QuickSort(T[] array, int low, int high)
    {
        if (low < high)
        {
            int pi = Partition(array, low, high);

            QuickSort(array, low, pi - 1);
            QuickSort(array, pi + 1, high);
        }
    }

    private int Partition(T[] array, int low, int high)
    {
        T pivot = array[high];
        int i = (low - 1);

        for (int j = low; j < high; j++)
        {
            int comparison = _comparer != null ?
                _comparer.Compare(array[j], pivot) :
                Comparer<T>.Default.Compare(array[j], pivot);

            if (comparison < 0)
            {
                i++;
                Swap(array, i, j);
            }
        }
        Swap(array, i + 1, high);
        return i + 1;
    }

    private void Swap(T[] array, int i, int j)
    {
        T temp = array[i];
        array[i] = array[j];
        array[j] = temp;
    }
}

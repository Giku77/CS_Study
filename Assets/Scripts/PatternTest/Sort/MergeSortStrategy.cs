using System.Collections.Generic;

public class MergeSortStrategy<T> : ISortStrategy<T>
{
    public void Sort(T[] list)
    {
        MergeSort(list, 0, list.Length - 1);
    }

    private void MergeSort(T[] list, int left, int right)
    {
        if (left >= right)
            return;

        int mid = (right + left) / 2;
        MergeSort(list, left, mid);
        MergeSort(list, mid + 1, right);
        Merge(list, left, mid, right);
    }
    
    private void Merge(T[] list, int left, int mid, int right)
    {
        int leftSize = mid - left + 1;
        int rightSize = right - mid;

        T[] leftArray = new T[leftSize];
        T[] rightArray = new T[rightSize];

        for (int i = 0; i < leftSize; i++)
            leftArray[i] = list[left + i];
        for (int j = 0; j < rightSize; j++)
            rightArray[j] = list[mid + 1 + j];

        int leftIndex = 0, rightIndex = 0;
        int mergedIndex = left;

        while (leftIndex < leftSize && rightIndex < rightSize)
        {
            if (Comparer<T>.Default.Compare(leftArray[leftIndex], rightArray[rightIndex]) <= 0)
            {
                list[mergedIndex] = leftArray[leftIndex];
                leftIndex++;
            }
            else
            {
                list[mergedIndex] = rightArray[rightIndex];
                rightIndex++;
            }
            mergedIndex++;
        }

        while (leftIndex < leftSize)
        {
            list[mergedIndex] = leftArray[leftIndex];
            leftIndex++;
            mergedIndex++;
        }

        while (rightIndex < rightSize)
        {
            list[mergedIndex] = rightArray[rightIndex];
            rightIndex++;
            mergedIndex++;
        }
    }   
}

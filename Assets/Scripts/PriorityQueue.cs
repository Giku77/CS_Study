using System;
using System.Collections.Generic;

public class PriorityQueue<TElement, TPriority>
    where TPriority : IComparable<TPriority>
{
    private List<(TElement Element, TPriority Priority)> heap;

    public PriorityQueue()
    {
        heap = new List<(TElement, TPriority)>();
    }

    public int Count => heap.Count;

    public void Enqueue(TElement element, TPriority priority)
    {
        // TODO: ����
        // 1. �� ��Ҹ� ����Ʈ ���� �߰�
        // 2. HeapifyUp���� �� �Ӽ� ����
        heap.Add((element, priority));
        HeapifyUp(heap.Count - 1);
    }

    public TElement Dequeue()
    {
        // TODO: ����
        // 1. �� ť üũ �� ���� ó��
        // 2. ��Ʈ ��� ����
        // 3. ������ ��Ҹ� ��Ʈ�� �̵�
        // 4. HeapifyDown���� �� �Ӽ� ����
        // 5. ����� ��Ʈ ��� ��ȯ
        if (heap.Count == 0)
            throw new InvalidOperationException("�켱���� ť�� �������.");
        var rootElement = heap[0].Element;
        heap[0] = heap[heap.Count - 1];
        heap.RemoveAt(heap.Count - 1);
        if (heap.Count > 0)
            HeapifyDown(0);
        return rootElement;
    }

    public TElement Peek()
    {
        // TODO: ����
        // 1. �� ť üũ �� ���� ó��
        // 2. ��Ʈ ��� ��ȯ
        if (heap.Count == 0)
            throw new InvalidOperationException("�켱���� ť�� �������.");
        return heap[0].Element;
    }

    public void Clear()
    {
        // TODO: ����
        heap.Clear();
    }

    private void HeapifyUp(int index)
    {
        // TODO: ����
        // ���� ��尡 �θ𺸴� ������ ��ȯ�ϸ� ���� �̵�
        var parentIndex = (index - 1) / 2;
        while (index > 0 && heap[index].Priority.CompareTo(heap[parentIndex].Priority) < 0)
        {
           Swap(index, parentIndex);
           index = parentIndex;
           parentIndex = (index - 1) / 2;
        }

    }

    private void Swap(int indexA, int indexB) =>
        (heap[indexA], heap[indexB]) = (heap[indexB], heap[indexA]);

    private void HeapifyDown(int index)
    {
        // TODO: ����
        // ���� ��尡 �ڽĺ��� ũ�� �� ���� �ڽİ� ��ȯ�ϸ� �Ʒ��� �̵�
        var leftChildIndex = 2 * index + 1;
        var rightChildIndex = 2 * index + 2;

        while (leftChildIndex < heap.Count)
        {
            var smallestChildIndex = leftChildIndex;
            if (rightChildIndex < heap.Count &&
                heap[rightChildIndex].Priority.CompareTo(heap[leftChildIndex].Priority) < 0)
            {
                smallestChildIndex = rightChildIndex;
            }
            if (heap[index].Priority.CompareTo(heap[smallestChildIndex].Priority) <= 0)
            {
                break;
            }
            Swap(index, smallestChildIndex);
            index = smallestChildIndex;
            leftChildIndex = 2 * index + 1;
            rightChildIndex = 2 * index + 2;
        }
    }
}

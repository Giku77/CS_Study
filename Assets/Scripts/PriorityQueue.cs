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
        // TODO: 구현
        // 1. 새 요소를 리스트 끝에 추가
        // 2. HeapifyUp으로 힙 속성 복구
        heap.Add((element, priority));
        HeapifyUp(heap.Count - 1);
    }

    public TElement Dequeue()
    {
        // TODO: 구현
        // 1. 빈 큐 체크 및 예외 처리
        // 2. 루트 요소 저장
        // 3. 마지막 요소를 루트로 이동
        // 4. HeapifyDown으로 힙 속성 복구
        // 5. 저장된 루트 요소 반환
        if (heap.Count == 0)
            throw new InvalidOperationException("우선순위 큐가 비어있음.");
        var rootElement = heap[0].Element;
        heap[0] = heap[heap.Count - 1];
        heap.RemoveAt(heap.Count - 1);
        if (heap.Count > 0)
            HeapifyDown(0);
        return rootElement;
    }

    public TElement Peek()
    {
        // TODO: 구현
        // 1. 빈 큐 체크 및 예외 처리
        // 2. 루트 요소 반환
        if (heap.Count == 0)
            throw new InvalidOperationException("우선순위 큐가 비어있음.");
        return heap[0].Element;
    }

    public void Clear()
    {
        // TODO: 구현
        heap.Clear();
    }

    private void HeapifyUp(int index)
    {
        // TODO: 구현
        // 현재 노드가 부모보다 작으면 교환하며 위로 이동
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
        // TODO: 구현
        // 현재 노드가 자식보다 크면 더 작은 자식과 교환하며 아래로 이동
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

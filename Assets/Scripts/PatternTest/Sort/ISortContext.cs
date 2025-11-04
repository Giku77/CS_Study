using System.Collections.Generic;

public interface ISortContext<T>
{
    void Sort(T[] list);
    void SetStrategy(ISortStrategy<T> strategy);
}
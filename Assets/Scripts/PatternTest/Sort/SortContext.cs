using System.Collections.Generic;
public class SortContext<T> : ISortContext<T>
{
    private ISortStrategy<T> _strategy;
    public void SetStrategy(ISortStrategy<T> strategy)
    {
        _strategy = strategy;
    }

    public void Sort(T[] list)
    {
        _strategy.Sort(list);
    }
}

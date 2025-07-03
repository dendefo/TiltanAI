using System;
using System.Collections.Generic;
using System.Linq;

public class SimplePriorityQueue<T>
{
    private readonly SortedDictionary<float, Queue<T>> _dict = new();
    private readonly HashSet<T> _set = new();

    public int Count { get; private set; } = 0;

    public void Enqueue(T item, float priority)
    {
        if (_set.Contains(item)) return; // Prevent duplicates
        if (!_dict.TryGetValue(priority, out var queue))
        {
            queue = new Queue<T>();
            _dict[priority] = queue;
        }
        queue.Enqueue(item);
        _set.Add(item);
        Count++;
    }

    public T Dequeue()
    {
        if (Count == 0) throw new InvalidOperationException("Queue is empty");
        var firstPair = _dict.First();
        var queue = firstPair.Value;
        var item = queue.Dequeue();
        if (queue.Count == 0)
            _dict.Remove(firstPair.Key);
        _set.Remove(item);
        Count--;
        return item;
    }

    public bool Contains(T item) => _set.Contains(item);
}
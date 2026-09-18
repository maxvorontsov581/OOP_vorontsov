using System.Collections;

namespace OOP.Domain;

public class Repository<T> : IReadOnlyRepository<T>, IEnumerable<T> where T : class, IEntity
{
    private readonly List<T> _items = new();

    public void Add(T item)
    {
        for (int i = 0; i < _items.Count; i++)
        {
            if (_items[i].Id == item.Id)
                return;
        }

        _items.Add(item);
    }

    public bool Remove(T item) => _items.Remove(item);

    public IReadOnlyCollection<T> FindAll(Predicate<T> predicate)
    {
        List<T> result = new();
        foreach (T x in _items)
        {
            if (predicate(x))
                result.Add(x);
        }
        return result.AsReadOnly();
    }

    public T? this[Guid id] => _items.FirstOrDefault(x => x.Id == id);

    public T? GetById(Guid id) => this[id];

    public IEnumerable<T> GetAll()
    {
        foreach (T item in _items)
            yield return item;
    }

    public IEnumerator<T> GetEnumerator()
    {
        foreach (T item in _items)
            yield return item;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public void Clear() => _items.Clear();
    public int Count => _items.Count;
}

public static class ReportExtensions
{
    public static string ToReportTable<T>(this IEnumerable<T> items)
    {
        return string.Join(Environment.NewLine, items.Select(x => "- " + x));
    }
}

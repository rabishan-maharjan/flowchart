public interface IScriptableList<T>
{
    void Remove(T item);
    int Count { get; }
    T this[int index] { get; set; }
    int IndexOf(T item);
}

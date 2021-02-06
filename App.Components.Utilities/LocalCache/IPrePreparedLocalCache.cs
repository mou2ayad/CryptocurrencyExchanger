namespace App.Components.Utilities.Cache
{
    public interface IPrePreparedLocalCache<T, Y> 
    {
        Y this[T key] { get; }

        void Clear();
        bool Contains(T key);

        bool TryGetValue(T key, out Y result);
        bool AddUpdate(T key, Y value);
        bool RemoveIfExists(T key);
    }
}
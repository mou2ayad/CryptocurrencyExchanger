using System;
using System.Collections.Generic;


namespace App.Components.Utilities.Cache
{
    public class PrePreparedLocalCache<T, Y> : IPrePreparedLocalCache<T, Y> 
    {
        private string name;
        private Func<Dictionary<T, Y>> populate;
        private Func<T, Y> lazyAdd;
        private Dictionary<T, Y> values;
        private DateTime lastPopulate;
        private static readonly object _lock = new object();
        private static readonly object _lock2 = new object();

        public PrePreparedLocalCache(string name)
        {
            values = new Dictionary<T, Y>();
            this.name = name;
        }
        public PrePreparedLocalCache(string name, Func<Dictionary<T, Y>> populate) : this(name)
        {
            this.populate = populate;
            Populate();
        }
        public PrePreparedLocalCache(string name, Func<T, Y> LazyAdd) : this(name)
        {                 
            this.lazyAdd = LazyAdd;         
        }

        public PrePreparedLocalCache(string name, Func<Dictionary<T, Y>> populate, Func<T, Y> LazyAdd) : this(name,populate)
        {           
            this.lazyAdd = LazyAdd;           
        }

        protected virtual void Populate()
        {
            if (populate != null)
                values = populate.Invoke();
            lastPopulate = DateTime.Now;
        }

        public virtual bool TryGetValue(T key, out Y result)
        {
            if (values.TryGetValue(key, out result))
                return true;
            lock (_lock)
            {
                if (values.TryGetValue(key, out result))
                    return true;
                if (lazyAdd != null)
                {                    
                    var value = lazyAdd.Invoke(key);
                    if (value != null)
                    {
                        lock(_lock2)
                        {
                            Y tempValue; 
                            if (!values.TryGetValue(key, out tempValue))
                                values.Add(key, value);
                        }
                    }                                     
                }
                else if (lastPopulate.AddHours(1) < DateTime.Now)                 
                    Populate();
            }
            return values.TryGetValue(key, out result);
        }
        public Y this[T key]
        {
            get
            {
                Y temp;
                if (TryGetValue(key, out temp))
                    return temp;
                throw new Exception("key is not found in the Cache");
            }                             
        }
        public bool Contains(T key)
        {
            return values.ContainsKey(key);
        }
        public void Clear()
        {
            this.values.Clear();
        }

        public bool AddUpdate(T key, Y value)
        {
            Y tempValue;
            if (values.TryGetValue(key, out tempValue))
            {
                if (tempValue.Equals(value))
                    return false;
                values[key] = value;
            }
            else
            {
                lock (_lock2)
                {
                    if (!values.TryGetValue(key, out tempValue))
                        values.Add(key, value);
                }
            }
            return true;
        }

        public bool RemoveIfExists(T key)
        {
            Y tempValue;
            if (values.TryGetValue(key, out tempValue))
            {
                values.Remove(key);
                return true;
            }
            return false;
        }
    }
}

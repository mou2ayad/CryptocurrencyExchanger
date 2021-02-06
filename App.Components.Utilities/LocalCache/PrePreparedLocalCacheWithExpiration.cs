using System;
using System.Collections.Generic;


namespace App.Components.Utilities.Cache
{
    public class PrePreparedLocalCacheWithExpiration<T, Y> : PrePreparedLocalCache<T, Y>
    {
        double expireAfter;
        DateTime expirationDate;
        private readonly object __lock = new object();
        /// <summary>
        /// Creating the Cache with expiry date
        /// </summary>
        /// <param name="name">name of the cache</param>
        /// <param name="ExpireAfter"> Expire After (in seconds), ExpireAfter<=0 means never expire</param>
        public PrePreparedLocalCacheWithExpiration(string name, double ExpireAfter) : base(name)
            => SetExpirationDate(ExpireAfter);

        public PrePreparedLocalCacheWithExpiration(string name, Func<Dictionary<T, Y>> populate, double ExpireAfter) : base(name, populate) => SetExpirationDate(ExpireAfter);

        public PrePreparedLocalCacheWithExpiration(string name, Func<T, Y> LazyAdd, double ExpireAfter) : base(name, LazyAdd) => SetExpirationDate(ExpireAfter);
        

        public PrePreparedLocalCacheWithExpiration(string name, Func<Dictionary<T, Y>> populate, Func<T, Y> LazyAdd, double ExpireAfter) : base(name, populate, LazyAdd) => SetExpirationDate(ExpireAfter);

        private void SetExpirationDate(double ExpireAfter)
        {
            expirationDate = ExpireAfter <= 0 ? DateTime.MaxValue : DateTime.Now.AddSeconds(ExpireAfter);
            expireAfter = ExpireAfter;
        }


        protected override void Populate()
        {
            expirationDate = expireAfter <= 0 ? DateTime.MaxValue : DateTime.Now.AddSeconds(expireAfter);
            base.Populate();
        }


        public override bool TryGetValue(T key, out Y result)
        {
            if (DateTime.Now > expirationDate)
            {
                lock(__lock)
                {
                    if(DateTime.Now > expirationDate)
                    {
                        this.Clear();
                        this.Populate();
                    }
                }
            }
            return base.TryGetValue(key, out result);
        }
    }
}

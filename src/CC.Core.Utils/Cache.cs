using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CC.Core.Utilities
{
    public class Cache<KEY, VALUE> : IEnumerable<VALUE> where VALUE : class
    {
        private readonly object _locker = new object();
        private readonly IDictionary<KEY, VALUE> _values;
        private Func<VALUE, KEY> _getKey = delegate { throw new NotImplementedException(); };

        private Func<KEY, VALUE> _onMissing =
            key => { throw new KeyNotFoundException(string.Format("Key '{0}' could not be found", key)); };

        public Cache()
            : this(new Dictionary<KEY, VALUE>())
        {
        }

        public Cache(Func<KEY, VALUE> onMissing)
            : this(new Dictionary<KEY, VALUE>(), onMissing)
        {
        }

        public Cache(IDictionary<KEY, VALUE> dictionary, Func<KEY, VALUE> onMissing)
            : this(dictionary)
        {
            _onMissing = onMissing;
        }

        public Cache(IDictionary<KEY, VALUE> dictionary)
        {
            _values = dictionary;
        }

        public Func<KEY, VALUE> OnMissing
        {
            set { _onMissing = value; }
        }

        public Func<VALUE, KEY> GetKey
        {
            get { return _getKey; }
            set { _getKey = value; }
        }

        public int Count
        {
            get { return _values.Count; }
        }

        public VALUE First
        {
            get
            {
                using (IEnumerator<KeyValuePair<KEY, VALUE>> enumerator = _values.GetEnumerator())
                {
                    if (enumerator.MoveNext())
                        return enumerator.Current.Value;
                }
                return default (VALUE);
            }
        }

        public VALUE this[KEY key]
        {
            get { return Retrieve(key); }
            set { Store(key, value); }
        }

        #region IEnumerable<VALUE> Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<VALUE> GetEnumerator()
        {
            return _values.Values.GetEnumerator();
        }

        #endregion

        public void Store(KEY key, VALUE value)
        {
            if (_values.ContainsKey(key))
                _values[key] = value;
            else
                _values.Add(key, value);
        }

        public void Fill(KEY key, VALUE value)
        {
            if (_values.ContainsKey(key))
                return;
            _values.Add(key, value);
        }

        public VALUE Retrieve(KEY key)
        {
            if (!_values.ContainsKey(key))
            {
                lock (_locker)
                {
                    if (!_values.ContainsKey(key))
                    {
                        VALUE local_0 = _onMissing(key);
                        _values.Add(key, local_0);
                    }
                }
            }
            return _values[key];
        }

        public bool TryRetrieve(KEY key, out VALUE value)
        {
            value = default (VALUE);
            if (!_values.ContainsKey(key))
                return false;
            value = _values[key];
            return true;
        }

        public void Each(Action<VALUE> action)
        {
            foreach (var keyValuePair in _values)
                action(keyValuePair.Value);
        }

        public void Each(Action<KEY, VALUE> action)
        {
            foreach (var keyValuePair in _values)
                action(keyValuePair.Key, keyValuePair.Value);
        }

        public bool Has(KEY key)
        {
            return _values.ContainsKey(key);
        }

        public bool Exists(Predicate<VALUE> predicate)
        {
            bool returnValue = false;
            Each(delegate(VALUE value) { returnValue |= predicate(value); });
            return returnValue;
        }

        public VALUE Find(Predicate<VALUE> predicate)
        {
            foreach (var keyValuePair in _values)
            {
                if (predicate(keyValuePair.Value))
                    return keyValuePair.Value;
            }
            return default (VALUE);
        }

        public KEY[] GetAllKeys()
        {
            return _values.Keys.ToArray();
        }

        public VALUE[] GetAll()
        {
            return _values.Values.ToArray();
        }

        public void Remove(KEY key)
        {
            if (!_values.ContainsKey(key))
                return;
            _values.Remove(key);
        }

        public void ClearAll()
        {
            _values.Clear();
        }

        public bool WithValue(KEY key, Action<VALUE> callback)
        {
            if (!Has(key))
                return false;
            callback(this[key]);
            return true;
        }
    }
}
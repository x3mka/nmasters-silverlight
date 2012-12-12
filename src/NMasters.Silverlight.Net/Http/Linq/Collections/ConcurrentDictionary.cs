using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;

namespace NMasters.Silverlight.Net.Http.Linq.Collections
{
    /// <summary>
    /// This is a stand-in implementation until Robert McLaws' System.Threading.Tasks package fixes the problem
    /// with TaskExtensions.Unwrap for Silverlight 5 (the official and unofficial System.Thread.Tasks both include a TaskExtensions class in the same
    /// namespace).
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class ConcurrentDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private readonly object _lock = new object();
        private readonly Dictionary<TKey, TValue> _dictionary;

        public ConcurrentDictionary(IEqualityComparer<TKey> comparer)
        {
            _dictionary = new Dictionary<TKey, TValue>(comparer);
        }

        public ConcurrentDictionary()
        {
            _dictionary = new Dictionary<TKey, TValue>();
        }

        public void Add(TKey key, TValue value)
        {
            lock (_lock)
            {
                _dictionary.Add(key, value);
            }
        }

        public bool ContainsKey(TKey key)
        {
            lock (_lock)
            {
                return _dictionary.ContainsKey(key);
            }
        }

        public bool Remove(TKey key)
        {
            lock (_lock)
            {
                return _dictionary.Remove(key);
            }
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            lock (_lock)
            {
                return _dictionary.TryGetValue(key, out value);
            }
        }

        public bool TryAddValue(TKey key, TValue value)
        {
            lock (_lock)
            {
                if (_dictionary.ContainsKey(key))
                    return false;

                _dictionary.Add(key, value);
                return true;
            }
        }

        public TValue GetOrAdd(TKey key, TValue value)
        {
            lock (_lock)
            {
                if (!_dictionary.ContainsKey(key))
                    _dictionary.Add(key, value);
                return _dictionary[key];
            }
        }

        public TValue GetOrAdd(TKey key, Func<TKey, TValue> valueFactory)
        {
            lock (_lock)
            {
                return GetOrAdd(key, valueFactory(key));                    
            }
        }

        public TValue AddOrUpdate(TKey key, TValue addValue, Func<TKey, TValue, TValue> updateValueFactory)
        {
            lock (_lock)
            {                
                while (true)
                {
                    TValue oldValue;
                    if (TryGetValue(key, out oldValue))
                    //key exists, try to update 
                    {
                        var newValue = updateValueFactory(key, oldValue);
                        _dictionary[key] = newValue;
                        
                        return newValue;                        
                    }
                    
                    if (TryAddValue(key, addValue))
                    {
                        return addValue;
                    }                    
                }
            }
        } 


        public bool TryRemove(TKey key, out TValue value)
        {
            lock (_lock)
            {
                value = default(TValue);                

                if (!_dictionary.ContainsKey(key))
                    return false;

                value = _dictionary[key];
                _dictionary.Remove(key);
                return true;                
            }
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            lock (_lock)
            {
                return ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).Remove(item);
            }
        }

        public int Count
        {
            get
            {
                lock (_lock)
                {
                    return _dictionary.Count;
                }
            }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public TValue this[TKey key]
        {
            get
            {
                lock (_lock)
                {
                    return _dictionary[key];
                }
            }
            set
            {
                lock (_lock)
                {
                    _dictionary[key] = value;
                }
            }
        }


        public ICollection<TValue> Values
        {
            get
            {
                lock (_lock)
                {
                    return _dictionary.Values.ToList();
                }
            }
        }

        public ICollection<TKey> Keys
        {
            get
            {
                lock (_lock)
                {
                    return _dictionary.Keys;
                }
            }
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            lock (_lock)
            {
                var items = _dictionary.ToList();
                return items.GetEnumerator();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            lock (_lock)
            {
                _dictionary.Add(item.Key, item.Value);
            }
        }

        public void Clear()
        {
            lock (_lock)
            {
                _dictionary.Clear();
            }
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            lock (_lock)
            {
                return ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).Contains(item);
            }
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            lock (_lock)
            {
                ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).CopyTo(array, arrayIndex);
            }
        }
    }
}
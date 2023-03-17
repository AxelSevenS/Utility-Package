using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;


namespace SevenGame.Utility {

    #if UNITY_EDITOR

    internal interface ISerializableDictionary : ISerializationCallbackReceiver, IEnumerable {}

    [System.Serializable]
    public class SerializableDictionary<TKey, TValue> : ISerializableDictionary, IDictionary<TKey, TValue> {

        [SerializeField] private List<ValuePair<TKey, TValue>> _pairs = new List<ValuePair<TKey, TValue>>();
        private Dictionary<TKey, TValue> _dictionary = new Dictionary<TKey, TValue>();
        

        public ICollection<TKey> Keys => _dictionary.Keys;

        public ICollection<TValue> Values => _dictionary.Values;

        public int Count => _dictionary.Count;

        public bool IsReadOnly => false;

        public TValue this[TKey key] { 
            get => _dictionary[key];
            set => _dictionary[key] = value;
        }


        public bool ContainsKey(TKey key) {
            return _dictionary.ContainsKey(key);
        }

        bool IDictionary<TKey, TValue>.Remove(TKey key) {
            return _dictionary.Remove(key);
        }

        public bool TryGetValue(TKey key, out TValue value) {
            return _dictionary.TryGetValue(key, out value);
        }

        public void Add(KeyValuePair<TKey, TValue> item) {
            _dictionary.Add(item.Key, item.Value);
        }

        public void Add(TKey key, TValue value) {
            _dictionary.Add(key, value);
        }

        public bool Remove(KeyValuePair<TKey, TValue> item) {
            return _dictionary.Remove(item.Key);
        }

        public void Remove(TKey key) {
            _dictionary.Remove(key);
        }
        
        public void Remove(TKey key, TValue value) {
            Remove(new KeyValuePair<TKey, TValue>(key, value));
        }

        public void Clear() {
            _dictionary.Clear();
            _pairs.Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item) {
            return _dictionary.Contains(item);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) {
            array = _dictionary.ToArray();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return _dictionary.GetEnumerator();
        }

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator() {
            return _dictionary.GetEnumerator();
        }

        
        public void OnBeforeSerialize() {
            _pairs.Clear();
            foreach (var pair in _dictionary) {
                _pairs.Add(new ValuePair<TKey, TValue>(pair.Key, pair.Value));
            }
        }

        public void OnAfterDeserialize() {
            _dictionary = new Dictionary<TKey, TValue>();
            foreach (var pair in _pairs) {
                try {
                    _dictionary.Add(pair.Key, pair.Value);
                } catch (ArgumentException) {
                    Debug.LogWarning($"Duplicate key {pair.Key} found in {this.GetType().Name}");
                }
            }
        }
    }

    #else

    [System.Serializable]
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue> {}

    #endif

}

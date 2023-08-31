using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SevenGame.Utility {

    public static class SerializableDictionaryExtensions {

        public static SerializableDictionary<TKey, TValue> ToSerializableDictionary<TKey, TValue>(this Dictionary<TKey, TValue> dictionary) {
            SerializableDictionary<TKey, TValue> serializableDictionary = new();
            foreach (KeyValuePair<TKey, TValue> pair in dictionary) {
                serializableDictionary.Add(pair.Key, pair.Value);
            }
            return serializableDictionary;
        }

        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this SerializableDictionary<TKey, TValue> serializableDictionary) {
            Dictionary<TKey, TValue> dictionary = new();
            foreach (KeyValuePair<TKey, TValue> pair in serializableDictionary) {
                dictionary.Add(pair.Key, pair.Value);
            }
            return dictionary;
        }

    }

}

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using UnityEngine;

namespace SevenGame.Utility {

    [Serializable]
    public class SerializableStack<TValue> : ISerializationCallbackReceiver, IEnumerable, IEnumerable<TValue>, IReadOnlyCollection<TValue> {

        [SerializeField] private List<TValue> _values = new();
        private Stack<TValue> _stack = new();

        public int Count => _stack.Count;
        

        public SerializableStack() {
            _stack = new Stack<TValue>();
        }
        public SerializableStack(IEnumerable<TValue> collection) {
            _stack = new Stack<TValue>(collection);
        }
        public SerializableStack(int capacity) {
            _stack = new Stack<TValue>(capacity);
        }


        public void Clear() {
            _stack.Clear();
            _values.Clear();
        }

        public bool Contains(TValue item) {
            return _stack.Contains(item);
        }


        public void CopyTo(TValue[] array, int arrayIndex) {
            _stack.CopyTo(array, arrayIndex);
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return _stack.GetEnumerator();
        }

        IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator() {
            return _stack.GetEnumerator();
        }

        public TValue Peek() {
            return _stack.Peek();
        }

        public TValue Pop() {
            return _stack.Pop();
        }

        public void Push(TValue item) {
            _stack.Push(item);
        }

        public TValue[] ToArray() {
            return _stack.ToArray();
        }

        public void TrimExcess() {
            _stack.TrimExcess();
        }

        public bool TryPeek(out TValue result) {
            return _stack.TryPeek(out result);
        }

        public bool TryPop(out TValue result) {
            return _stack.TryPop(out result);
        }

        
        public void OnBeforeSerialize() {
            _values.Clear();
            foreach (TValue pair in _stack) {
                _values.Insert(0, pair);
            }
        }

        public void OnAfterDeserialize() {
            _stack = new Stack<TValue>();
            foreach (TValue stackValue in _values) {
                _stack.Push(stackValue);
            }
        }
    }
    
}

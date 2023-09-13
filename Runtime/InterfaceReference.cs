using System;

using UnityEngine;

using Object = UnityEngine.Object;


namespace SevenGame.Utility {

    [Serializable]
    public struct InterfaceReference<T> where T : class {
        [SerializeField] private Object _reference;
        private T _value;

        public T Value {
            get {
                if (_value == null && _reference != null) {
                    _value = _reference as T;
                }
                return _value;
            }
            set {
                _value = value;
                if (value is Object obj) {
                    _reference = obj;
                    // TODO?: add other checks for other Serialization methods
                } else {
                    Debug.LogWarning($"{typeof(T).Name} is not a serializable Object; it will not be serialized and will be lost on reload.");
                }
            }
        }
    }
}

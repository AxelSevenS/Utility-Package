using System;
using UnityEngine;


namespace SevenGame.Utility {

    [Serializable]
    public struct ValuePair<T1, T2> {
        [SerializeReference] public T1 Key;
        [SerializeReference] public T2 Value;

        public ValuePair(T1 Key, T2 Value){
            this.Key = Key;
            this.Value = Value;
        }
    }
}
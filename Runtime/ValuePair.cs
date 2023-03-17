using System.Runtime.CompilerServices;
using UnityEngine;

namespace SevenGame.Utility {
    
    internal interface IValuePair {}

    [System.Serializable]
    public class ValuePair<T1, T2> : IValuePair{
        [SerializeReference] public T1 Key;
        [SerializeReference] public T2 Value;

        public ValuePair(T1 Key, T2 Value){
            this.Key = Key;
            this.Value = Value;
        }
    }
}
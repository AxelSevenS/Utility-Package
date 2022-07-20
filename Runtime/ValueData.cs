using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace SevenGame.Utility {

    public interface ValueData<T> {
        T currentValue { get; set; }
        T lastValue { get; set; }
        void SetVal(T value);
    } 

    [System.Serializable]
    public struct VectorData : ValueData<Vector3> {

        public Vector3 currentValue { get; set; }
        public Vector3 lastValue { get; set; }
        
        public Timer zeroTimer;
        public Timer nonZeroTimer;

        public float x => currentValue.x;
        public float y => currentValue.y;
        public float z => currentValue.z;
        public float magnitude => currentValue.magnitude;
        public Vector3 normalized => currentValue.normalized;

        public static implicit operator Vector3(VectorData data) => data.currentValue;
        public static Vector3 operator *(VectorData a, float b) => a.currentValue * b;
        public static Vector3 operator *(float a, VectorData b) => a * b.currentValue;
        
        public void SetVal(Vector3 value){
            lastValue = currentValue;
            currentValue = value;
            
            zeroTimer.SetTime( currentValue.magnitude == 0 ? Time.time : zeroTimer );
            nonZeroTimer.SetTime( currentValue.magnitude != 0 ? Time.time : nonZeroTimer );
        }
    }

    [System.Serializable]
    public struct QuaternionData : ValueData<Quaternion> {
        public Quaternion currentValue { get; set; }
        public Quaternion lastValue { get; set; }

        public void SetVal(Quaternion value){
            lastValue = currentValue;
            currentValue = value;
        }

        public static implicit operator Quaternion(QuaternionData data) => data.currentValue;
        public static Vector3 operator *(QuaternionData a, Vector3 b) => a.currentValue * b;
        public static Vector3 operator *(QuaternionData a, VectorData b) => a.currentValue * b.currentValue;
        public static Quaternion operator *(QuaternionData a, QuaternionData b) => a.currentValue * b.currentValue;
        
    }
    
    [System.Serializable]
    public struct BoolData : ValueData<bool> {
        public bool currentValue { get; set; }
        public bool lastValue { get; set; }

        public Timer trueTimer;
        public Timer falseTimer;
        public bool started;
        public bool stopped;

        public static implicit operator bool(BoolData data) => data.currentValue;
        public void SetVal(bool value){
            lastValue = currentValue;
            currentValue = value;

            started = currentValue && !lastValue;
            stopped = !currentValue && lastValue;

            falseTimer.SetTime( !value ? Time.time : falseTimer );
            trueTimer.SetTime( value ? Time.time : trueTimer );
        }
    }

    [System.Serializable]
    public struct KeyInputData : ValueData<bool> {
        public bool currentValue { get; set; }
        public bool lastValue { get; set; }

        private const float HOLD_TIME = 0.2f;

        public Timer trueTimer;
        public Timer falseTimer;
        public bool started;
        public bool stopped;

        public bool tapped;
        public bool held;

        public static implicit operator bool(KeyInputData data) => data.currentValue;
        
        public void SetVal(bool value){
            lastValue = currentValue;
            currentValue = value;

            started = currentValue && !lastValue;
            stopped = !currentValue && lastValue;

            tapped = stopped && trueTimer < HOLD_TIME; 
            held = currentValue && trueTimer > HOLD_TIME; 

            falseTimer.SetTime( !value ? Time.time : falseTimer );
            trueTimer.SetTime( value ? Time.time : trueTimer );
        }

    }
}
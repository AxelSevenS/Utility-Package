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
        public float sqrMagnitude => currentValue.sqrMagnitude;
        public float magnitude => currentValue.magnitude;
        public Vector3 normalized => currentValue.normalized;

        public static implicit operator Vector3(VectorData data) => data.currentValue;
        public static Vector3 operator *(VectorData a, float b) => a.currentValue * b;
        public static Vector3 operator *(float a, VectorData b) => a * b.currentValue;
        
        public void SetVal(Vector3 value){
            lastValue = currentValue;
            currentValue = value;
            
            if ( currentValue.magnitude == 0 ) nonZeroTimer.SetTime(Time.time);
            else zeroTimer.SetTime(Time.time);
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

            if (currentValue) falseTimer.SetTime(Time.time);
            else trueTimer.SetTime(Time.time);
        }
    }

    [System.Serializable]
    public struct KeyInputData : ValueData<bool> {
        public bool currentValue { get; set; }
        public bool lastValue { get; set; }

        private const float HOLD_TIME = 0.15f;

        public Timer trueTimer;
        public Timer falseTimer;
        public bool started;
        public bool stopped;

        public bool tapped;
        public bool held;

        public static implicit operator bool(KeyInputData data) => data.currentValue;

        public static bool SimultaneousTap(KeyInputData a, KeyInputData b, float time = HOLD_TIME) {
            bool aTapped = a.trueTimer < time && a && b.started;
            bool bTapped = b.trueTimer < time && b && a.started;
            return aTapped || bTapped;
        }
        
        public void SetVal(bool value){
            lastValue = currentValue;
            currentValue = value;

            started = currentValue && !lastValue;
            stopped = !currentValue && lastValue;

            tapped = stopped && trueTimer < HOLD_TIME; 
            held = currentValue && trueTimer > HOLD_TIME; 

            if (currentValue) falseTimer.SetTime(Time.time);
            else trueTimer.SetTime(Time.time);
        }

    }
}
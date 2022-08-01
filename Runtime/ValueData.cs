using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace SevenGame.Utility {

    public interface ValueData<T> {
        void SetVal(T value);
    } 

    [System.Serializable]
    public struct Vector2Data : ValueData<Vector2> {

        public Vector2 currentValue;
        public Vector2 lastValue;
        
        public Timer zeroTimer;
        public Timer nonZeroTimer;

        public float x => currentValue.x;
        public float y => currentValue.y;
        public float sqrMagnitude => currentValue.sqrMagnitude;
        public float magnitude => currentValue.magnitude;
        public Vector2 normalized => currentValue.normalized;

        public static implicit operator Vector2(Vector2Data data) => data.currentValue;
        public static Vector2 operator *(Vector2Data a, float b) => a.currentValue * b;
        public static Vector2 operator *(float a, Vector2Data b) => a * b.currentValue;
        
        public void SetVal(Vector2 value){
            lastValue = currentValue;
            currentValue = value;
            
            if ( sqrMagnitude == 0 ) nonZeroTimer.Start();
            else zeroTimer.Start();
        }
    }

    [System.Serializable]
    public struct Vector3Data : ValueData<Vector3> {

        public Vector3 currentValue;
        public Vector3 lastValue;
        
        public Timer zeroTimer;
        public Timer nonZeroTimer;

        public float x => currentValue.x;
        public float y => currentValue.y;
        public float z => currentValue.z;
        public float sqrMagnitude => currentValue.sqrMagnitude;
        public float magnitude => currentValue.magnitude;
        public Vector3 normalized => currentValue.normalized;

        public static implicit operator Vector3(Vector3Data data) => data.currentValue;
        public static Vector3 operator *(Vector3Data a, float b) => a.currentValue * b;
        public static Vector3 operator *(float a, Vector3Data b) => a * b.currentValue;
        
        public void SetVal(Vector3 value){
            lastValue = currentValue;
            currentValue = value;
            
            if ( sqrMagnitude == 0 ) nonZeroTimer.Start();
            else zeroTimer.Start();
        }
    }

    [System.Serializable]
    public struct QuaternionData : ValueData<Quaternion> {
        public Quaternion currentValue;
        public Quaternion lastValue;
        
        public float x => currentValue.x;
        public float y => currentValue.y;
        public float z => currentValue.z;
        public float w => currentValue.w;

        public void SetVal(Quaternion value){
            lastValue = currentValue;
            currentValue = value;
        }

        public static implicit operator Quaternion(QuaternionData data) => data.currentValue;
        public static Vector3 operator *(QuaternionData a, Vector3 b) => a.currentValue * b;
        public static Vector3 operator *(QuaternionData a, Vector3Data b) => a.currentValue * b.currentValue;
        public static Quaternion operator *(QuaternionData a, QuaternionData b) => a.currentValue * b.currentValue;
        
    }
    
    [System.Serializable]
    public struct BoolData : ValueData<bool> {
        public bool currentValue;
        public bool lastValue;

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

            if (currentValue) falseTimer.Start();
            else trueTimer.Start();
        }
    }

    [System.Serializable]
    public struct KeyInputData : ValueData<bool> {
        public bool currentValue;
        public bool lastValue;

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

            if (currentValue) falseTimer.Start();
            else trueTimer.Start();
        }

    }
}
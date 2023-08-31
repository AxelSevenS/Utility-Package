using System;

using UnityEngine;

namespace SevenGame.Utility {

    public interface IValueData<T> {
        void SetVal(T value);
    } 

    [Serializable]
    public struct Vector2Data : IValueData<Vector2> {

        public Vector2 currentValue;
        public Vector2 lastValue;
        
        public Timer zeroTimer;
        public Timer nonZeroTimer;

        public readonly float x => currentValue.x;
        public readonly float y => currentValue.y;
        public readonly float sqrMagnitude => currentValue.sqrMagnitude;
        public readonly float magnitude => currentValue.magnitude;
        public readonly Vector2 normalized => currentValue.normalized;

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

    [Serializable]
    public struct Vector3Data : IValueData<Vector3> {

        public Vector3 currentValue;
        public Vector3 lastValue;
        
        public Timer zeroTimer;
        public Timer nonZeroTimer;

        public readonly float x => currentValue.x;
        public readonly float y => currentValue.y;
        public readonly float z => currentValue.z;
        public readonly float sqrMagnitude => currentValue.sqrMagnitude;
        public readonly float magnitude => currentValue.magnitude;
        public readonly Vector3 normalized => currentValue.normalized;

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

    [Serializable]
    public struct QuaternionData : IValueData<Quaternion> {
        public Quaternion currentValue;
        public Quaternion lastValue;
        
        public readonly float x => currentValue.x;
        public readonly float y => currentValue.y;
        public readonly float z => currentValue.z;
        public readonly float w => currentValue.w;

        public void SetVal(Quaternion value){
            lastValue = currentValue;
            currentValue = value;
        }

        public static implicit operator Quaternion(QuaternionData data) => data.currentValue;
        public static Vector3 operator *(QuaternionData a, Vector3 b) => a.currentValue * b;
        public static Vector3 operator *(QuaternionData a, Vector3Data b) => a.currentValue * b.currentValue;
        public static Quaternion operator *(QuaternionData a, QuaternionData b) => a.currentValue * b.currentValue;
        
    }
    
    [Serializable]
    public struct BoolData : IValueData<bool> {
        public bool currentValue;
        public bool lastValue;
        public readonly bool Started => currentValue && !lastValue;
        public readonly bool Stopped => !currentValue && lastValue;

        public Timer trueTimer;
        public Timer falseTimer;

        public static implicit operator bool(BoolData data) => data.currentValue;
        public void SetVal(bool value){
            if (currentValue) falseTimer.Start();
            else trueTimer.Start();

            lastValue = currentValue;
            currentValue = value;
        }
    }

    [Serializable]
    public struct KeyInputData : IValueData<bool> {

        private const float HOLD_TIME = 0.15f;


        public bool currentValue;
        public bool lastValue;
        public readonly bool Started => currentValue && !lastValue;
        public readonly bool Stopped => !currentValue && lastValue;

        public Timer trueTimer;
        public Timer falseTimer;

        public static implicit operator bool(KeyInputData data) => data.currentValue;


        public readonly bool Tapped(float time = HOLD_TIME) => Stopped && trueTimer < time;
        public readonly bool Held(float time = HOLD_TIME) => currentValue && trueTimer > time;

        public static bool SimultaneousTap(KeyInputData a, KeyInputData b, float time = HOLD_TIME) {
            bool aTapped = a.trueTimer < time && b.Started;
            bool bTapped = b.trueTimer < time && a.Started;
            return aTapped || bTapped;
        }

        public readonly bool SimultaneousTap(KeyInputData other, float time = HOLD_TIME) {
            return SimultaneousTap(this, other, time);
        }
        
        public void SetVal(bool value){
            if (currentValue) falseTimer.Start();
            else trueTimer.Start();

            lastValue = currentValue;
            currentValue = value;
        }

    }
}
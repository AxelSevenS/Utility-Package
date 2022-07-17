using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace SevenGame.Utility {

    
    [System.Serializable]
    public class ValueData<T> {

        protected const float HOLD_TIME = 0.2f;

        public T currentValue;
        public T lastValue;
        
        public static implicit operator T(ValueData<T> data) => data.currentValue;
        
        public virtual void SetVal(T updatedValue){
            lastValue = currentValue;
            currentValue = updatedValue;
        }
    }

    [System.Serializable]
    public class VectorData : ValueData<Vector3> {

        private float lastZeroTime;
        private float lastNonZeroTime;
        public float zeroTimer => Time.time - lastNonZeroTime;
        public float nonZeroTimer => Time.time - lastZeroTime;

        public float x => currentValue.x;
        public float y => currentValue.y;
        public float z => currentValue.z;
        public float magnitude => currentValue.magnitude;
        public Vector3 normalized => currentValue.normalized;

        public static Vector3 operator *(VectorData a, float b) => a.currentValue * b;
        public static Vector3 operator *(float a, VectorData b) => a * b.currentValue;
        
        public override void SetVal(Vector3 updatedValue){
            base.SetVal(updatedValue);
            lastZeroTime = updatedValue.magnitude == 0 ? Time.time : lastZeroTime;
            lastNonZeroTime = updatedValue.magnitude != 0 ? Time.time : lastNonZeroTime;
        }
    }

    [System.Serializable]
    public class QuaternionData : ValueData<Quaternion> {

        public static Vector3 operator *(QuaternionData a, Vector3 b) => a.currentValue * b;
        public static Vector3 operator *(QuaternionData a, VectorData b) => a.currentValue * b.currentValue;
        public static Quaternion operator *(QuaternionData a, QuaternionData b) => a.currentValue * b.currentValue;
        
    }
    
    [System.Serializable]
    public class BoolData : ValueData<bool> {

        private float lastTrueTime;
        private float lastFalseTime;
        public float trueTimer => Time.time - lastFalseTime;
        public float falseTimer => Time.time - lastTrueTime;
        public bool started => currentValue && !lastValue;
        public bool stopped => !currentValue && lastValue;
        public bool tapped;
        public bool held;

        
        public override void SetVal(bool updatedValue){
            base.SetVal(updatedValue);
            tapped = stopped && trueTimer < HOLD_TIME;
            held = currentValue && trueTimer > HOLD_TIME;
            lastFalseTime = !updatedValue ? Time.time : lastFalseTime;
            lastTrueTime = updatedValue ? Time.time : lastTrueTime;
        }

    }
}
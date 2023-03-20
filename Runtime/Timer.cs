using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace SevenGame.Utility {

    [System.Serializable]
    public struct Timer {
        public float startTime;
        public float duration => Time.unscaledTime - startTime;

        public static implicit operator float(Timer timer) => timer.duration;

        public void Start(){
            startTime = Time.unscaledTime;
        }

    }

    [System.Serializable]
    public struct TimeInterval {
        public float startTime;
        public float stopTime;
        public bool isDone => Time.unscaledTime >= stopTime;
        public float duration => stopTime - startTime;
        public float remainingDuration => isDone ? 0f : stopTime - Time.unscaledTime;
        public float progress => isDone ? 1f : (Time.unscaledTime - startTime) / duration;

        public static implicit operator float(TimeInterval timeUntil) => timeUntil.remainingDuration;

        public void SetTime(float time){
            startTime = Time.unscaledTime;
            stopTime = time;
        }
        public void SetDuration(float duration){
            startTime = Time.unscaledTime;
            stopTime = startTime + duration;
        }
    }

    [System.Serializable]
    public struct ScaledTimer {
        public float startTime;
        public float duration => Time.time - startTime;

        public static implicit operator float(ScaledTimer timer) => timer.duration;

        public void Start(){
            startTime = Time.time;
        }

    }

    [System.Serializable]
    public struct ScaledTimeInterval {
        public float startTime;
        public float stopTime;
        public bool isDone => Time.time >= stopTime;
        public float duration => stopTime - startTime;
        public float remainingDuration => isDone ? 0f : stopTime - Time.time;
        public float progress => isDone ? 1f : (Time.time - startTime) / duration;

        public static implicit operator float(ScaledTimeInterval timeUntil) => timeUntil.remainingDuration;

        public void SetTime(float time){
            startTime = Time.time;
            stopTime = time;
        }
        public void SetDuration(float duration){
            startTime = Time.time;
            stopTime = startTime + duration;
        }
    }
}
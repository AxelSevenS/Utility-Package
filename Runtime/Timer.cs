using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;


namespace SevenGame.Utility {

    [Serializable]
    public struct Timer {
        public float startTime;
        public readonly float duration => Time.unscaledTime - startTime;

        public static implicit operator float(Timer timer) => timer.duration;

        public void Start(){
            startTime = Time.unscaledTime;
        }

    }

    [Serializable]
    public struct TimeInterval {
        public float startTime;
        public float stopTime;
        public readonly bool isDone => Time.unscaledTime >= stopTime;
        public readonly float duration => stopTime - startTime;
        public readonly float remainingDuration => isDone ? 0f : stopTime - Time.unscaledTime;
        public readonly float progress => isDone ? 1f : (Time.unscaledTime - startTime) / duration;

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

    [Serializable]
    public struct ScaledTimer {
        public float startTime;
        public readonly float duration => Time.time - startTime;

        public static implicit operator float(ScaledTimer timer) => timer.duration;

        public void Start(){
            startTime = Time.time;
        }

    }

    [Serializable]
    public struct ScaledTimeInterval {
        public float startTime;
        public float stopTime;
        public readonly bool isDone => Time.time >= stopTime;
        public readonly float duration => stopTime - startTime;
        public readonly float remainingDuration => isDone ? 0f : stopTime - Time.time;
        public readonly float progress => isDone ? 1f : (Time.time - startTime) / duration;

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
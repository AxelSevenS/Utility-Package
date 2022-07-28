using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace SevenGame.Utility {

    public struct Timer {
        public float startTime;
        public float duration => Time.time - startTime;

        public static implicit operator float(Timer timer) => timer.duration;

        public void Start(){
            startTime = Time.time;
        }

    }

    public struct TimeUntil {
        public float stopTime;
        public bool isDone => Time.time >= stopTime;
        public float duration => isDone ? 0f : stopTime - Time.time;

        public static implicit operator float(TimeUntil timeUntil) => timeUntil.duration;

        public void SetTime(float time){
            stopTime = time;
        }
        public void SetDuration(float duration){
            stopTime = Time.time + duration;
        }

    }
}
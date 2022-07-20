using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace SevenGame.Utility {

    public struct Timer {
        public float startTime;
        public float duration => Time.time - startTime;

        public static implicit operator float(Timer timer) => timer.duration;
        public void SetTime(float time){
            startTime = time;
        }

    }
}
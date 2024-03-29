using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SevenGame.Utility {
    
    public static class Mathfs {

        public static Vector3 NullifyInDirection( this Vector3 vector, Vector3 direction) => Vector3.Dot(vector, direction) >= 0f ? Vector3.ProjectOnPlane(vector, direction) : vector;

    }
}
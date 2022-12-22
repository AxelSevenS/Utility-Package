using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SevenGame.Utility {
    
    public static class Mathfs{

        public static OrientedPoint Transform(this Transform transform, OrientedPoint orientedPoint){
            return new OrientedPoint(transform.TransformPoint(orientedPoint.position), transform.rotation * orientedPoint.rotation);
        }
        public static OrientedPoint InverseTransform(this Transform transform, OrientedPoint orientedPoint){
            return new OrientedPoint(transform.InverseTransformPoint(orientedPoint.position), Quaternion.Inverse(transform.rotation) * orientedPoint.rotation);
        }

        
        public static ControlPoint Transform(this Transform transform, ControlPoint orientedPoint){
            return new ControlPoint(transform.TransformPoint(orientedPoint.position)/* , transform.TransformDirection(orientedPoint.forward) */, orientedPoint.upAngle);
        }
        public static ControlPoint InverseTransform(this Transform transform, ControlPoint orientedPoint){
            return new ControlPoint(transform.InverseTransformPoint(orientedPoint.position)/* , transform.InverseTransformDirection(orientedPoint.forward) */, orientedPoint.upAngle);
        }



        public static Vector3 NullifyInDirection( this Vector3 vector, Vector3 direction) => Vector3.Dot(vector, direction) >= 0f ? Vector3.ProjectOnPlane(vector, direction) : vector;

        public static float CalculateWave(float waveStrength, float time, Vector3 coords, float frequency){
            return waveStrength * Mathf.Sin(time + (coords.x + coords.z) * frequency);
        }

    }
}
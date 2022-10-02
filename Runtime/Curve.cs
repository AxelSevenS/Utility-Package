using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SevenGame.Utility {

    [System.Serializable]
    public abstract class Curve {

        private const int LENGTH_PRECISION = 100;


        
        [SerializeField] protected float _length;
        [SerializeField] protected float[] _arcLengths;



        public float length {
            get {
                if (_length == 0f)
                    UpdateLength();
                return _length;
            }
            private set {
                _length = value;
            }
        }

        public float[] arcLengths {
            get {
                if (_arcLengths == null || _arcLengths.Length == 0)
                    UpdateLength();
                return _arcLengths;
            }
            private set {
                _arcLengths = value;
            }
        }



        public abstract void Reset();

        public abstract void Move(Vector3 direction);

        public abstract OrientedPoint GetPoint(float t);
        public virtual OrientedPoint GetPointUniform(float t) {
            float distance = t * length;

            int upperBound = 1;
            // find the bounds around distance in _arcLengths
            while (upperBound < LENGTH_PRECISION && arcLengths[upperBound] < distance) {
                upperBound++;
            }
            int lowerBound = upperBound - 1;

            float tBetweenBounds = Mathf.InverseLerp(_arcLengths[lowerBound], _arcLengths[upperBound], distance);

            const float PERCENTILE = 1f / (float)LENGTH_PRECISION;
            float uniformT = Mathf.Lerp(lowerBound * PERCENTILE, upperBound * PERCENTILE, tBetweenBounds);

            return GetPoint(uniformT);
        }
        
        public virtual void UpdateLength() {
            _arcLengths = new float[LENGTH_PRECISION + 1];
            _arcLengths[0] = 0;

            Vector3 lastPoint = GetPoint(0f).position;
            _length = 0;

            for (int i = 0; i < LENGTH_PRECISION + 1; i++) {
                float t = (float)i / LENGTH_PRECISION;
                Vector3 currentPoint = GetPoint(t).position;

                _length += (lastPoint - currentPoint).magnitude;
                _arcLengths[i] = _length;

                lastPoint = currentPoint;
            }
        }

        // public float SampleArcLengths(float t){
            // int count = arcLengths.Length; 
            // int precision = count - 1;
            // float lengthIndex = t * (precision);
            // int lowerBound = Mathf.FloorToInt(lengthIndex);
            // int upperBound = Mathf.FloorToInt(lengthIndex + 1);
            
            // if ( upperBound >= count )
                // return arcLengths[precision];
            // if ( lowerBound < 0 )
                // return arcLengths[0];
            // return Mathf.Lerp( arcLengths[lowerBound], arcLengths[upperBound], lengthIndex - lowerBound);
        // }
    }

    [System.Serializable]
    public class BezierQuadratic : Curve {

        public OrientedPoint controlPoint1;
        public OrientedPoint controlPoint2;
        public OrientedPoint handle;



        public BezierQuadratic(){
            controlPoint1 = new OrientedPoint();
            controlPoint2 = new OrientedPoint();
            handle = new OrientedPoint();
        }
        public BezierQuadratic(Vector3 cp1Pos, Vector3 cp2Pos, Vector3 hPos){
            controlPoint1 = new OrientedPoint(cp1Pos);
            controlPoint2 = new OrientedPoint(cp2Pos);
            handle = new OrientedPoint(hPos);
        }
        public BezierQuadratic(OrientedPoint cp1Pos, OrientedPoint cp2Pos, OrientedPoint hPos){
            controlPoint1 = cp1Pos;
            controlPoint2 = cp2Pos;
            handle = hPos;
        }
        public BezierQuadratic(Transform cp1Pos, Transform cp2Pos, Transform hPos){
            controlPoint1 = new OrientedPoint(cp1Pos);
            controlPoint2 = new OrientedPoint(cp2Pos);
            handle = new OrientedPoint(hPos);
        }



        public override void Reset(){
            controlPoint1 = new OrientedPoint();
            controlPoint2 = new OrientedPoint(Vector3.forward * 50f);
            handle = new OrientedPoint(Vector3.forward * 25f);

        }

        public override void Move(Vector3 direction){
            controlPoint1.position += direction;
            controlPoint2.position += direction;
            handle.position += direction;
        }

        public override OrientedPoint GetPoint(float t){
            Vector3 a = Vector3.Lerp(controlPoint1.position, handle.position, t);
            Vector3 b = Vector3.Lerp(handle.position, controlPoint2.position, t);

            Vector3 c = Vector3.Lerp(a, b, t);

            Vector3 tForward = (b - a).normalized;
            Vector3 tUp = Quaternion.Lerp(controlPoint1.rotation, controlPoint2.rotation, t) * Vector3.up;
            Quaternion rotation = tForward == Vector3.zero ? Quaternion.identity : Quaternion.LookRotation( tForward, tUp);

            return new OrientedPoint( c, rotation );
        }
    }

    [System.Serializable]
    public class BezierCubic : Curve {

        public OrientedPoint controlPoint1;
        public OrientedPoint controlPoint2;
        public OrientedPoint handle1;
        public OrientedPoint handle2;



        public override void Reset(){
            controlPoint1 = new OrientedPoint();
            controlPoint2 = new OrientedPoint(Vector3.forward * 75f);
            handle1 = new OrientedPoint(Vector3.forward * 25f);
            handle2 = new OrientedPoint(Vector3.forward * 50f);

        }

        public BezierCubic(){
            controlPoint1 = new OrientedPoint();
            controlPoint2 = new OrientedPoint();
            handle1 = new OrientedPoint();
            handle2 = new OrientedPoint();
        }
        public BezierCubic(Vector3 cp1Pos, Vector3 cp2Pos, Vector3 h1Pos, Vector3 h2Pos){
            controlPoint1 = new OrientedPoint(cp1Pos);
            controlPoint2 = new OrientedPoint(cp2Pos);
            handle1 = new OrientedPoint(h1Pos);
            handle2 = new OrientedPoint(h2Pos);
        }
        public BezierCubic(OrientedPoint cp1Pos, OrientedPoint cp2Pos, OrientedPoint h1Pos, OrientedPoint h2Pos){
            controlPoint1 = cp1Pos;
            controlPoint2 = cp2Pos;
            handle1 = h1Pos;
            handle2 = h2Pos;
        }
        public BezierCubic(Transform cp1Pos, Transform cp2Pos, Transform h1Pos, Transform h2Pos){
            controlPoint1 = new OrientedPoint(cp1Pos);
            controlPoint2 = new OrientedPoint(cp2Pos);
            handle1 = new OrientedPoint(h1Pos);
            handle2 = new OrientedPoint(h2Pos);
        }



        public override void Move(Vector3 direction){
            controlPoint1.position += direction;
            controlPoint2.position += direction;
            handle1.position += direction;
            handle2.position += direction;
        } 

        public override OrientedPoint GetPoint(float t){
            Vector3 a = Vector3.Lerp(controlPoint1.position, handle1.position, t);
            Vector3 b = Vector3.Lerp(handle1.position, handle2.position, t);
            Vector3 c = Vector3.Lerp(handle2.position, controlPoint2.position, t);

            Vector3 d = Vector3.Lerp(a, b, t);
            Vector3 e = Vector3.Lerp(b, c, t);
            Vector3 f = Vector3.Lerp( d, e, t );

            Vector3 tForward = (e - d).normalized;
            Vector3 tUp = Quaternion.Lerp(controlPoint1.rotation, controlPoint2.rotation, t) * Vector3.up;
            Quaternion rotation = tForward == Vector3.zero ? Quaternion.identity : Quaternion.LookRotation( tForward, tUp);

            return new OrientedPoint( f, rotation );
        }

        // public Vector3 GetVelocity(float t){
        //     float tPow2 = Mathf.Pow(t, 2f);
        //     Vector3 p0 = controlPoint1.position * ( (-3f * tPow2) + (6f * t) - 3f );
        //     Vector3 p1 = handle1.position * ( (9f * tPow2) - (12f * t) + 3f );
        //     Vector3 p2 = handle2.position * ( (-9f * tPow2) + (6f * t) );
        //     Vector3 p3 = controlPoint2.position * ( 3f * tPow2 );

        //     return p0 + p1 + p2 + p3;
        // }

        // public Vector3 GetAcceleration(float t){
        //     Vector3 p0 = controlPoint1.position * ( -6f * t + 6f );
        //     Vector3 p1 = handle1.position * ( 18f * t - 12f );
        //     Vector3 p2 = handle2.position * ( -18f * t + 6f );
        //     Vector3 p3 = controlPoint2.position * ( 6f * t );

        //     return p0 + p1 + p2 + p3;
        // }

        // public Vector3 GetCurvature(float t){
        //     Vector3 velo = GetVelocity(t);
        //     Vector3 accel = GetAcceleration(t);

        //     return ( Vector3.Dot() );
        // }
    }

    
    [System.Serializable]
    public class StraightLine : Curve {

        public OrientedPoint controlPoint1;
        public OrientedPoint controlPoint2;



        public StraightLine(){
            controlPoint1 = new OrientedPoint();
            controlPoint2 = new OrientedPoint();
        }
        public StraightLine(Vector3 cp1Pos, Vector3 cp2Pos){
            controlPoint1 = new OrientedPoint(cp1Pos);
            controlPoint2 = new OrientedPoint(cp2Pos);
        }
        public StraightLine(OrientedPoint cp1Pos, OrientedPoint cp2Pos){
            controlPoint1 = cp1Pos;
            controlPoint2 = cp2Pos;
        }
        public StraightLine(Transform cp1Pos, Transform cp2Pos){
            controlPoint1 = new OrientedPoint(cp1Pos);
            controlPoint2 = new OrientedPoint(cp2Pos);
        }



        public override void Reset(){
            controlPoint1 = new OrientedPoint();
            controlPoint2 = new OrientedPoint(Vector3.forward * 50f);

        }

        public override void Move(Vector3 direction){
            controlPoint1.position += direction;
            controlPoint2.position += direction;
        }

        public override OrientedPoint GetPoint(float t){
            Vector3 position = Vector3.Lerp(controlPoint1.position, controlPoint2.position, t);
            Vector3 tForward = (controlPoint2.position - controlPoint1.position).normalized;
            Vector3 tUp = Vector3.Lerp(controlPoint1.rotation * Vector3.up, controlPoint2.rotation * Vector3.up, t);
            Quaternion rotation = Quaternion.LookRotation( tForward, tUp);

            return new OrientedPoint( position , rotation );
        }

        public override OrientedPoint GetPointUniform(float t) {
            return GetPoint(t);
        }

        public override void UpdateLength() {

            _arcLengths = new float[2];
            _arcLengths[0] = 0;

            _length = 0;

            Vector3 firstPoint = GetPoint(0f).position;
            Vector3 lastPoint = GetPoint(1f).position;
            
            _length = (firstPoint - lastPoint).magnitude;
            _arcLengths[1] = _length;
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SevenGame.Utility {
    
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
}

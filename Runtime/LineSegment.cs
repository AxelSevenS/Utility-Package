using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SevenGame.Utility {
    
    [System.Serializable]
    public class LineSegment : Segment {

        public LineSegment(){
            controlPoint1 = new OrientedPoint();
            controlPoint2 = new OrientedPoint();
        }
        public LineSegment(Vector3 cp1Pos, Vector3 cp2Pos){
            controlPoint1 = new OrientedPoint(cp1Pos);
            controlPoint2 = new OrientedPoint(cp2Pos);
        }
        public LineSegment(OrientedPoint cp1Pos, OrientedPoint cp2Pos){
            controlPoint1 = cp1Pos;
            controlPoint2 = cp2Pos;
        }
        public LineSegment(Transform cp1Pos, Transform cp2Pos){
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
    }
}

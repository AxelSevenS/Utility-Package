using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace SevenGame.Utility {

    [System.Serializable]
    public struct ControlPoint {
        
        public Vector3 position;
        public Vector3 forward;
        public float upAngle;



        public Quaternion rotation => Quaternion.LookRotation(forward, Quaternion.AngleAxis(upAngle, forward) * Vector3.up);

        

        public static ControlPoint operator +(ControlPoint a, Vector3 b) => new ControlPoint(a.position + b, a.forward, a.upAngle);
        public static ControlPoint operator +(Vector3 a, ControlPoint b) => new ControlPoint(b.position + a, b.forward, b.upAngle);
        public static ControlPoint operator -(ControlPoint a, Vector3 b) => new ControlPoint(a.position - b, a.forward, a.upAngle);
        public static ControlPoint operator -(Vector3 a, ControlPoint b) => new ControlPoint(b.position - a, b.forward, b.upAngle);
        
        
        public static bool operator ==(ControlPoint obj1, ControlPoint obj2) => obj1.Equals(obj2);
        public static bool operator !=(ControlPoint obj1, ControlPoint obj2) => !obj1.Equals(obj2);



        public ControlPoint(ControlPoint op){
            this.position = op.position;
            this.forward = op.forward;
            this.upAngle = op.upAngle;
        }
        public ControlPoint(Vector3 pos, Vector3 forward, float angle){
            this.position = pos;
            this.forward = forward;
            this.upAngle = angle;
        }
        public ControlPoint(Vector3 pos){
            this.position = pos;
            this.forward = Vector3.forward;
            this.upAngle = 0f;
        }

        public void SetPosition(Vector3 pos){
            this.position = pos;
        }
        public void SetForward(Vector3 forward){
            this.forward = forward;
        }
        public void Set(float angle){
            this.upAngle = angle;
        }
        public void Set(Vector3 pos, Vector3 forw, float rot){
            this.position = pos;
            this.forward = forw;
            this.upAngle = rot;
        }
        public void Set(ControlPoint op){
            this.position = op.position;
            this.forward = op.forward;
            this.upAngle = op.upAngle;
        }

        public override bool Equals(object obj) {
            if (obj is ControlPoint op)
                return position == op.position && forward == op.forward && upAngle == op.upAngle;
            else
                return false;
        }

        public override int GetHashCode() => System.Tuple.Create(position, upAngle).GetHashCode();
    }
}

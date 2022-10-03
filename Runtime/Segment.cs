using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SevenGame.Utility {
    
    [System.Serializable]
    public abstract class Segment {

        public OrientedPoint controlPoint1;
        public OrientedPoint controlPoint2;

        [SerializeField] protected float _length;



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



        public abstract void Reset();

        public abstract void Move(Vector3 direction);

        public abstract OrientedPoint GetPoint(float t);
        public virtual OrientedPoint GetPointUniform(float t) {
            return GetPoint(t);
        }

        public virtual void UpdateLength() {

            _length = 0;

            Vector3 firstPoint = GetPoint(0f).position;
            Vector3 lastPoint = GetPoint(1f).position;
            
            _length = (firstPoint - lastPoint).magnitude;
        }
    }
}

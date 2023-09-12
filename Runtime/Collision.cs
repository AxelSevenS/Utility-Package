using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SevenGame.Utility {
    
    public static class Collision {

        public static void GetCapsuleInfo( this CapsuleCollider capsule, Vector3 position, float skinThickness, out Vector3 startPos, out Vector3 endPos, out float radius ){
            
            Transform capsuleTransform = capsule.transform;
            float skinThicknessOrEpsilon = Mathf.Max(skinThickness, Mathf.Epsilon);

            // depending on the collider's "direction" we set the direction of the capsule length; 
            // if capsule.direction is 0, the capsule will extend in the collider's right and left directions;
            Vector3 capsuleDirection;
            float heightScale;
            float radiusScale;
            switch (capsule.direction){
                default:
                case 0:
                    capsuleDirection = capsuleTransform.right;
                    heightScale = capsuleTransform.localScale.x;
                    radiusScale = Mathf.Max(capsuleTransform.localScale.y, capsuleTransform.localScale.z);
                    break;
                case 1:
                    capsuleDirection = capsuleTransform.up;
                    heightScale = capsuleTransform.localScale.y;
                    radiusScale = Mathf.Max(capsuleTransform.localScale.x, capsuleTransform.localScale.z);
                    break;
                case 2:
                    capsuleDirection = capsuleTransform.forward;
                    heightScale = capsuleTransform.localScale.z;
                    radiusScale = Mathf.Max(capsuleTransform.localScale.x, capsuleTransform.localScale.y);
                    break;
            }

            Vector3 capsulePosition = position + capsuleTransform.rotation * Vector3.Scale(capsule.center, capsuleTransform.localScale);
            float scaledRadius = capsule.radius * radiusScale;

            float scaledHalfHeight = capsule.height/2f * heightScale;
            Vector3 capsuleHalf = (scaledHalfHeight - scaledRadius) * capsuleDirection;

            startPos = capsulePosition + capsuleHalf; 
            endPos = capsulePosition - capsuleHalf;
            radius = scaledRadius - skinThicknessOrEpsilon;
        }

        public static Vector3 GetSize( this Collider collider ){
            return collider switch {
                CapsuleCollider capsule => capsule.GetSize(),
                BoxCollider box => box.GetSize(),
                SphereCollider sphere => sphere.GetSize(),
                _ => Vector3.zero
            };
        }

        public static Vector3 GetSize( this CapsuleCollider collider ) {
            Vector3[] directionArray = new Vector3[] { Vector3.right, Vector3.up, Vector3.forward };
            Vector3 result = new();
            for (int i = 0; i < 3; i ++) {
                if (i == collider.direction)
                    result += directionArray[i] * collider.height;
                else
                    result += 2 * collider.radius * directionArray[i];
            }
            return result;
        }

        public static Vector3 GetSize( this BoxCollider collider ) => collider.size;

        public static Vector3 GetSize( this SphereCollider collider ) => new(collider.radius, collider.radius, collider.radius);

        public static Vector3 GetCenter( this Collider collider ){
            return collider switch {
                CapsuleCollider capsule => capsule.center,
                BoxCollider box => box.center,
                SphereCollider sphere => sphere.center,
                _ => Vector3.zero
            };
        }



        public static bool ColliderCast( this Collider collider, Vector3 position, Vector3 direction, out RaycastHit hit, float skinThickness, LayerMask layerMask ){
            float skinThicknessOrEpsilon = Mathf.Max(skinThickness, Mathf.Epsilon);
            return collider switch {
                CapsuleCollider capsule => capsule.ColliderCast(position, direction, out hit, skinThicknessOrEpsilon, layerMask),
                SphereCollider sphere => sphere.ColliderCast(position, direction, out hit, skinThicknessOrEpsilon, layerMask),
                BoxCollider box => box.ColliderCast(position, direction, out hit, skinThicknessOrEpsilon, layerMask),
                _ => NullCast(out hit)
            };
        }

        public static bool ColliderCast(this CapsuleCollider collider, Vector3 position, Vector3 direction, out RaycastHit hit, float skinThickness, LayerMask layerMask) {
            collider.GetCapsuleInfo( position, skinThickness, out Vector3 startPos, out Vector3 endPos, out float radius );
            return Physics.CapsuleCast( startPos, endPos, radius, direction.normalized, out hit, direction.magnitude + skinThickness, layerMask );
        }

        public static bool ColliderCast(this SphereCollider collider, Vector3 position, Vector3 direction, out RaycastHit hit, float skinThickness, LayerMask layerMask) {
            return Physics.SphereCast( collider.transform.position + position, collider.radius - skinThickness, direction.normalized, out hit, direction.magnitude + skinThickness, layerMask );
        }

        public static bool ColliderCast(this BoxCollider collider, Vector3 position, Vector3 direction, out RaycastHit hit, float skinThickness, LayerMask layerMask) {
            Vector3 skinThicknessVector = new(skinThickness, skinThickness, skinThickness);
            return Physics.BoxCast( collider.transform.position + position + collider.transform.rotation * collider.center, (collider.size - skinThicknessVector)/2f, direction.normalized, out hit, collider.transform.rotation, direction.magnitude + skinThickness, layerMask );
        }

        public static bool NullCast(out RaycastHit hit) {
            hit = new RaycastHit();
            return false;
        }

        public static Collider[] ColliderOverlap( this Collider collider, Vector3 position, float skinThickness, LayerMask layerMask ){
            float skinThicknessOrEpsilon = Mathf.Max(skinThickness, Mathf.Epsilon);
            return collider switch {
                CapsuleCollider capsule => capsule.ColliderOverlap(position, skinThicknessOrEpsilon, layerMask),
                SphereCollider sphere => sphere.ColliderOverlap(position, skinThicknessOrEpsilon, layerMask),
                BoxCollider box => box.ColliderOverlap(position, skinThicknessOrEpsilon, layerMask),
                _ => NullOverlap()
            };
        }

        public static Collider[] ColliderOverlap(this CapsuleCollider collider, Vector3 position, float skinThickness, LayerMask layerMask) {
            collider.GetCapsuleInfo( position, skinThickness, out Vector3 startPos, out Vector3 endPos, out float radius );
            return Physics.OverlapCapsule( startPos, endPos, radius, layerMask );
        }

        public static Collider[] ColliderOverlap(this SphereCollider collider, Vector3 position, float skinThickness, LayerMask layerMask) {
            float skinnedRadius = collider.radius - Mathf.Min(skinThickness, collider.radius - 0.01f);
            return Physics.OverlapSphere( collider.transform.position + position, skinnedRadius, layerMask );
        }

        public static Collider[] ColliderOverlap(this BoxCollider collider, Vector3 position, float skinThickness, LayerMask layerMask) {
            Vector3 skinThicknessVector = new(skinThickness, skinThickness, skinThickness);
            return Physics.OverlapBox( collider.transform.position + position + collider.transform.rotation * collider.center, (collider.size - skinThicknessVector)/2f, collider.transform.rotation, layerMask );
        }

        public static Collider[] NullOverlap() => new Collider[0];
        
    }
}

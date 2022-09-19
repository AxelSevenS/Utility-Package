using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using SevenGame.Utility;

namespace SevenGame.Utility.Editor {

    [CanEditMultipleObjects]
    [CustomEditor(typeof(Spline))]
    public class SplineEditor : UnityEditor.Editor{

        private Spline targetSpline;

        private SerializedObject so;
        private SerializedProperty propSplineCurve;
        private SerializedProperty propControlPoint1;
        private SerializedProperty propControlPoint2;
        private SerializedProperty propHandle1;
        private SerializedProperty propHandle2;

        private SerializedProperty propLength;
        private SerializedProperty propArcLengths;
        
        private SerializedProperty propNextSegment;
        private SerializedProperty propPrevSegment;
        
        private SerializedProperty propMesh2D;
        private SerializedProperty propCount;
        private SerializedProperty propScale;



        private void OnUndoRedo(){
            targetSpline.UpdateOtherSegments();
        }
        
        public override void OnInspectorGUI(){
            /* DrawDefaultInspector(); */

		    EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField( propSplineCurve, new GUIContent("Spline") );

            GUILayout.Space( 15 );

            GUILayout.Label( "Spline Segments", EditorStyles.boldLabel );
            using ( new GUILayout.HorizontalScope( EditorStyles.helpBox ) ){
                EditorGUIUtility.labelWidth = 150;
                using ( new GUILayout.VerticalScope( EditorStyles.label ) ){
                    GUILayout.Label( "Previous Segment", EditorStyles.boldLabel );
                    EditorGUILayout.PropertyField( propPrevSegment, GUIContent.none );

                    GUILayout.Space( 5 );

                    if (targetSpline.prevSegment == null){
                        if (GUILayout.Button( "Add Previous Segment" )) targetSpline.AddPrev();
                    }else{
                        if (GUILayout.Button( "Remove Previous Segment" )) targetSpline.RemovePrev();
                    }
                }
                using ( new GUILayout.VerticalScope( EditorStyles.label ) ){
                    GUILayout.Label( "Next Segment", EditorStyles.boldLabel );
                    EditorGUILayout.PropertyField( propNextSegment, GUIContent.none );

                    GUILayout.Space( 5 );

                    if (targetSpline.nextSegment == null){
                        if (GUILayout.Button( "Add Next Segment" )) targetSpline.AddNext();
                    }else {
                        if (GUILayout.Button( "Remove Next Segment" )) targetSpline.RemoveNext();
                    }
                }
            }

            GUILayout.Space( 15 );

            GUILayout.Label( "Procedural Mesh", EditorStyles.boldLabel );
            using ( new GUILayout.VerticalScope( EditorStyles.helpBox ) ){
                EditorGUILayout.PropertyField( propMesh2D );

                if (targetSpline.mesh2D != null){

                    EditorGUILayout.PropertyField( propCount, new GUIContent("Repeat Count") );
                    EditorGUILayout.PropertyField( propScale, new GUIContent("Scale of Mesh") );
                }
            }

            if ( EditorGUI.EndChangeCheck() ){

                Undo.RecordObject( target, "Edited Spline Values" ); 

                so.ApplyModifiedProperties();

                targetSpline.UpdateOtherSegments();

            }
        }

        public void OnSceneGUI(){
            
		    EditorGUI.BeginChangeCheck();

            OrientedPoint newControlPoint1 = targetSpline.TransformPoint(targetSpline.controlPoint1);
            OrientedPoint newControlPoint2 = targetSpline.TransformPoint(targetSpline.controlPoint2);
            OrientedPoint newHandle1 = targetSpline.TransformPoint(targetSpline.handle1);
            OrientedPoint newHandle2 = targetSpline.TransformPoint(targetSpline.handle2);

            GUIStyle style = GUIStyle.none;
            style.fontSize = 15;
            style.alignment = TextAnchor.MiddleCenter;
            style.normal.textColor = Color.white;


            Handles.Label(newControlPoint1.position, "Control Point 1", style );
            Handles.Label(newControlPoint2.position, "Control Point 2", style );
            Handles.Label(newHandle1.position, "Handle 1", style );
            Handles.Label(newHandle2.position, "Handle 2", style );

            Handles.TransformHandle(ref newControlPoint1.position, ref newControlPoint1.rotation);
            Handles.TransformHandle(ref newControlPoint2.position, ref newControlPoint2.rotation);
            newHandle1.position = Handles.PositionHandle(newHandle1.position, Quaternion.identity);
            newHandle2.position = Handles.PositionHandle(newHandle2.position, Quaternion.identity);

            if ( EditorGUI.EndChangeCheck() ){

                Undo.RecordObject( target, "Edited Spline" ); 

                targetSpline.controlPoint1.Set(targetSpline.transform.InverseTransformPoint(newControlPoint1));
                targetSpline.controlPoint2.Set(targetSpline.transform.InverseTransformPoint(newControlPoint2));

                targetSpline.handle1.Set(targetSpline.transform.InverseTransformPoint(newHandle1));
                targetSpline.handle2.Set(targetSpline.transform.InverseTransformPoint(newHandle2));

                targetSpline.UpdateOtherSegments();

            }
        }

        [DrawGizmo(GizmoType.InSelectionHierarchy | GizmoType.Active)]
        private static void OnDrawGizmosSelected(Spline scr, GizmoType gizmoType) {

            OrientedPoint transformedControlPoint1 = scr.TransformPoint(scr.controlPoint1);
            OrientedPoint transformedControlPoint2 = scr.TransformPoint(scr.controlPoint2);
            OrientedPoint transformedHandle1 = scr.TransformPoint(scr.handle1);
            OrientedPoint transformedHandle2 = scr.TransformPoint(scr.handle2);
            Handles.DrawBezier( transformedControlPoint1.position, transformedControlPoint2.position, transformedHandle1.position, transformedHandle2.position, Color.white, EditorGUIUtility.whiteTexture, 1f );

            for (int i = 0; i < scr.ringCount; i++){
                OrientedPoint pointAlongTessel = scr.GetBezier( (float)i/((float)scr.ringCount - 1) );
                // pointAlongTessel.position = scr.transform.TransformPoint(pointAlongTessel.position);
                // Vector3 velocityAlongTessel = GetVelocity( (float)i/(float)ringCount );
                // Vector3 accelerationAlongTessel = GetAcceleration( (float)i/(float)ringCount );

                // Gizmos.color = Color.magenta;
                // Gizmos.DrawLine( pointAlongTessel.position, pointAlongTessel.position + velocityAlongTessel );

                // Gizmos.color = Color.blue;
                // Gizmos.DrawLine( pointAlongTessel.position, pointAlongTessel.position + accelerationAlongTessel );


                if (scr.mesh2D == null) continue;
                Gizmos.color = Color.red;

                for (int j = 0; j < scr.mesh2D.vertices.Length-1; j++)
                    Gizmos.DrawLine(pointAlongTessel.position + (pointAlongTessel.rotation * scr.mesh2D.vertices[j].point)*scr.scale, pointAlongTessel.position + (pointAlongTessel.rotation * scr.mesh2D.vertices[j+1].point)*scr.scale);
                Gizmos.DrawLine(pointAlongTessel.position + (pointAlongTessel.rotation * scr.mesh2D.vertices[scr.mesh2D.vertices.Length-1].point)*scr.scale, pointAlongTessel.position + (pointAlongTessel.rotation * scr.mesh2D.vertices[0].point)*scr.scale);
                
                // Handles.PositionHandle(point.pos, point.rot);
            }
            if ( scr.transform.hasChanged ){

                scr.transform.hasChanged = false; 
                scr.UpdateOtherSegments();
            }
        }

        
        
        private void OnEnable(){
            Undo.undoRedoPerformed += OnUndoRedo;

            so = serializedObject;
            propSplineCurve = so.FindProperty( "splineCurve" );
            propControlPoint1 = propSplineCurve.FindPropertyRelative( "controlPoint1" );
            propControlPoint2 = propSplineCurve.FindPropertyRelative( "controlPoint2" ); 
            propHandle1 = propSplineCurve.FindPropertyRelative( "handle1" );
            propHandle2 = propSplineCurve.FindPropertyRelative( "handle2" );

            propLength = propSplineCurve.FindPropertyRelative( "_length" );
            propArcLengths = propSplineCurve.FindPropertyRelative( "_arcLengths" );

            propNextSegment = so.FindProperty( "nextSegment" );
            propPrevSegment = so.FindProperty( "prevSegment" );

            propMesh2D = so.FindProperty( "mesh2D" );
            propCount = so.FindProperty( "ringCount" );
            propScale = so.FindProperty( "scale" );

            targetSpline = (Spline)target;
        }

        private void OnDisable(){
            Undo.undoRedoPerformed -= OnUndoRedo;
        }
    }
}
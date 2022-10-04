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
        private SerializedProperty propSegmentType;

        private SerializedProperty propLinearSegment;
        private SerializedProperty propBezierCubic;
        private SerializedProperty propBezierQuadratic;

        private SerializedProperty propLength;
        private SerializedProperty propArcLengths;

        private SerializedProperty propStoppingPointCheck;
        private SerializedProperty propStoppingPoint;
        
        private SerializedProperty propNextSegment;
        private SerializedProperty propPrevSegment;
        
        private SerializedProperty propMesh2D;
        private SerializedProperty propCount;
        private SerializedProperty propScale;

        private Spline.SegmentType segmentType => targetSpline.segmentType;
        private SerializedProperty propSegment {
            get {
                switch (segmentType) {
                    default:
                        return propLinearSegment;
                    case Spline.SegmentType.BezierCubic:
                        return propBezierCubic;
                    case Spline.SegmentType.BezierQuadratic:
                        return propBezierQuadratic;
                }
            }
        }



        private void OnUndoRedo(){
            targetSpline.UpdateOtherSegments();
        }
        
        public override void OnInspectorGUI(){

		    EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField( propSegmentType, new GUIContent("Segment Type") );

            if (EditorGUI.EndChangeCheck()) {
                targetSpline.segmentType = (Spline.SegmentType)propSegmentType.enumValueIndex;

                Undo.RecordObject(targetSpline, "Change Segment Type");
                targetSpline.UpdateOtherSegments();
            }

		    EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField( propSegment, new GUIContent("Segment") );

            GUILayout.Space( 15 );

            GUILayout.Label( "Spline Segments", EditorStyles.boldLabel );
            using ( new GUILayout.HorizontalScope( EditorStyles.helpBox ) ){
                EditorGUIUtility.labelWidth = 150;
                using ( new GUILayout.VerticalScope( EditorStyles.label ) ){
                    GUILayout.Label( "Previous Segment", EditorStyles.boldLabel );
                    EditorGUILayout.PropertyField( propPrevSegment, GUIContent.none );

                    GUILayout.Space( 5 );

                    if (targetSpline.previousSpline == null){
                        if (GUILayout.Button( "Add Previous Segment" )) targetSpline.AddPrev();
                    }else{
                        if (GUILayout.Button( "Remove Previous Segment" )) targetSpline.RemovePrev();
                    }
                }
                using ( new GUILayout.VerticalScope( EditorStyles.label ) ){
                    GUILayout.Label( "Next Segment", EditorStyles.boldLabel );
                    EditorGUILayout.PropertyField( propNextSegment, GUIContent.none );

                    GUILayout.Space( 5 );

                    if (targetSpline.nextSpline == null){
                        if (GUILayout.Button( "Add Next Segment" )) targetSpline.AddNext();
                    }else {
                        if (GUILayout.Button( "Remove Next Segment" )) targetSpline.RemoveNext();
                    }
                }
            }
            
            GUILayout.Space( 15 );
            
            EditorGUILayout.PropertyField( propStoppingPointCheck );
            if ( propStoppingPointCheck.boolValue ) {
                EditorGUILayout.PropertyField( propStoppingPoint );
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

        [DrawGizmo(GizmoType.InSelectionHierarchy | GizmoType.Active)]
        private static void OnDrawGizmosSelected(Spline scr, GizmoType gizmoType) {

            ControlPoint transformedControlPoint1 = scr.transform.Transform(scr.cp1);
            ControlPoint transformedControlPoint2 = scr.transform.Transform(scr.cp2);

            switch ( scr.segmentType ){
                default:
                    Gizmos.DrawLine( transformedControlPoint1.position, transformedControlPoint2.position );
                    break;
                case Spline.SegmentType.BezierCubic:
                    Vector3 transformedHandle1 = scr.transform.TransformPoint(scr.h1);
                    Vector3 transformedHandle2 = scr.transform.TransformPoint(scr.h2);
                    Handles.DrawBezier( transformedControlPoint1.position, transformedControlPoint2.position, transformedHandle1, transformedHandle2, Color.white, EditorGUIUtility.whiteTexture, 1f );
                    Gizmos.DrawLine( transformedControlPoint1.position, transformedHandle1 );
                    Gizmos.DrawLine( transformedControlPoint2.position, transformedHandle2 );
                    break;
                case Spline.SegmentType.BezierQuadratic:
                    Vector3 transformedHandle = scr.transform.TransformPoint(scr.h);
                    // Handles.DrawBezier( transformedControlPoint1.position, transformedControlPoint2.position, transformedHandle.position, transformedHandle.position, Color.white, EditorGUIUtility.whiteTexture, 1f );
                    Gizmos.DrawLine( transformedControlPoint1.position, transformedHandle );
                    Gizmos.DrawLine( transformedControlPoint2.position, transformedHandle );
                    break;
            }

            for (int i = 0; i < scr.ringCount; i++){
                OrientedPoint pointAlongTessel = scr.GetPointUniform( (float)i/((float)scr.ringCount - 1) );

                if (scr.mesh2D == null) continue;
                Gizmos.color = Color.red;

                for (int j = 0; j < scr.mesh2D.vertices.Length-1; j++)
                    Gizmos.DrawLine(pointAlongTessel.position + (pointAlongTessel.rotation * scr.mesh2D.vertices[j].point)*scr.scale, pointAlongTessel.position + (pointAlongTessel.rotation * scr.mesh2D.vertices[j+1].point)*scr.scale);
                Gizmos.DrawLine(pointAlongTessel.position + (pointAlongTessel.rotation * scr.mesh2D.vertices[scr.mesh2D.vertices.Length-1].point)*scr.scale, pointAlongTessel.position + (pointAlongTessel.rotation * scr.mesh2D.vertices[0].point)*scr.scale);
                
            }
            if ( scr.transform.hasChanged ){

                scr.transform.hasChanged = false; 
                scr.UpdateOtherSegments();
            }
        }

        public void OnSceneGUI(){

            ControlPoint controlPoint1 = targetSpline.transform.Transform(targetSpline.cp1);
            Vector3 tangent1 = targetSpline.GetTangent(0f);
            if ( DrawControlPointGUI( ref controlPoint1, tangent1, 0.25f, Color.red ) ) {
                Undo.RecordObject(targetSpline, "Edited Cubic Spline");
                targetSpline.cp1.Set(targetSpline.transform.InverseTransform(controlPoint1));
                targetSpline.UpdateOtherSegments();
            }

            ControlPoint controlPoint2 = targetSpline.transform.Transform(targetSpline.cp2);
            Vector3 tangent2 = targetSpline.GetTangent(1f);
            if ( DrawControlPointGUI( ref controlPoint2, tangent2, 0.25f, Color.red ) ) {
                Undo.RecordObject(targetSpline, "Edited Cubic Spline");
                targetSpline.cp2.Set(targetSpline.transform.InverseTransform(controlPoint2));
                targetSpline.UpdateOtherSegments();
            }

            switch ( segmentType ){
                case Spline.SegmentType.BezierQuadratic:

                    Vector3 handle = targetSpline.transform.TransformPoint(targetSpline.h);
                    if ( DrawHandleGUI( ref handle, 0.2f, Color.blue ) ) {
                        Undo.RecordObject(targetSpline, "Edited Cubic Spline");
                        targetSpline.h = targetSpline.transform.InverseTransformPoint(handle);
                        targetSpline.UpdateOtherSegments();
                    }
                    break;

                case Spline.SegmentType.BezierCubic:

                    Vector3 handle1 = targetSpline.transform.TransformPoint(targetSpline.h1);
                    if ( DrawHandleGUI( ref handle1, 0.2f, Color.blue ) ) {
                        Undo.RecordObject(targetSpline, "Edited Cubic Spline");
                        targetSpline.h1 = targetSpline.transform.InverseTransformPoint(handle1);
                        targetSpline.UpdateOtherSegments();
                    }

                    Vector3 handle2 = targetSpline.transform.TransformPoint(targetSpline.h2);
                    if ( DrawHandleGUI( ref handle2, 0.2f, Color.blue ) ) {
                        Undo.RecordObject(targetSpline, "Edited Cubic Spline");
                        targetSpline.h2 = targetSpline.transform.InverseTransformPoint(handle2);
                        targetSpline.UpdateOtherSegments();
                    }
                    break;
            }

        }

        


        private bool DrawControlPointGUI( ref ControlPoint point, Vector3 tangent, float size, Color color ) {

            float handleSize = HandleUtility.GetHandleSize(point.position);
            float moveHandleSize = handleSize * 0.2f;
            Handles.color = color;


            EditorGUI.BeginChangeCheck();


            point.position = Handles.FreeMoveHandle(point.position, Quaternion.identity, moveHandleSize, Vector3.zero, Handles.SphereHandleCap);
            // point.position = Handles.PositionHandle(point.position, Quaternion.identity);
            
            point.forward = tangent;

            Quaternion newRotation = Handles.Disc(point.rotation, point.position, tangent, handleSize * 0.5f, false, 0f);
            point.upAngle = Mathf.Abs(newRotation.eulerAngles.z);


            return EditorGUI.EndChangeCheck();
            
        }

        private bool DrawHandleGUI( ref Vector3 point, float size, Color color) {

            float handleSize = HandleUtility.GetHandleSize(point) * 0.2f;
            Handles.color = color;


            EditorGUI.BeginChangeCheck();

            point = Handles.FreeMoveHandle(point, Quaternion.identity, handleSize, Vector3.zero, Handles.SphereHandleCap);
            // point = Handles.PositionHandle(point, Quaternion.identity);

            return EditorGUI.EndChangeCheck();
            
        }





        
        
        private void OnEnable(){
            Undo.undoRedoPerformed += OnUndoRedo;

            targetSpline = (Spline)target;

            so = serializedObject;
            propSegmentType = so.FindProperty( "segmentType" );

            propLinearSegment = so.FindProperty( "_linearSegment" );
            propBezierCubic = so.FindProperty( "_bezierCubic" );
            propBezierQuadratic = so.FindProperty( "_bezierQuadratic" );

            propStoppingPointCheck = so.FindProperty( "hasStoppingPoint" );
            propStoppingPoint = so.FindProperty( "stoppingPoint" );

            propNextSegment = so.FindProperty( "nextSpline" );
            propPrevSegment = so.FindProperty( "previousSpline" );

            propMesh2D = so.FindProperty( "mesh2D" );
            propCount = so.FindProperty( "ringCount" );
            propScale = so.FindProperty( "scale" );
        }

        private void OnDisable(){
            Undo.undoRedoPerformed -= OnUndoRedo;
        }

    }
}
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

        private int selectedSegment = 0;



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

            Segment segment = scr.segment;

            if (segment is LineSegment lineSegment) {
                Gizmos.DrawLine( segment.controlPoint1.position, segment.controlPoint2.position );
            }
            if (segment is BezierCubic bezierCubic) {
                Handles.DrawBezier( segment.controlPoint1.position, segment.controlPoint2.position, bezierCubic.handle1, bezierCubic.handle2, Color.white, EditorGUIUtility.whiteTexture, 1f );
                Gizmos.DrawLine( segment.controlPoint1.position, bezierCubic.handle1 );
                Gizmos.DrawLine( segment.controlPoint2.position, bezierCubic.handle2 );
            }
            if (segment is BezierQuadratic bezierQuadratic) {
                // draw a Quadratic Bezier
                Handles.DrawBezier( segment.controlPoint1.position, segment.controlPoint2.position, bezierQuadratic.handle, bezierQuadratic.handle, Color.white, EditorGUIUtility.whiteTexture, 1f );
                Gizmos.DrawLine( segment.controlPoint1.position, bezierQuadratic.handle );
                Gizmos.DrawLine( segment.controlPoint2.position, bezierQuadratic.handle );
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

            ControlPoint controlPoint1 = targetSpline.segment.controlPoint1;
            Vector3 tangent1 = targetSpline.GetTangent(0f);
            if ( DrawControlPointGUI( ref controlPoint1, tangent1, 0.25f, Color.red, 201 ) ) {
                Undo.RecordObject(targetSpline, "Edited Spline");
                targetSpline.segment.controlPoint1.Set(controlPoint1);
                targetSpline.UpdateOtherSegments();
            }

            ControlPoint controlPoint2 = targetSpline.segment.controlPoint2;
            Vector3 tangent2 = targetSpline.GetTangent(1f);
            if ( DrawControlPointGUI( ref controlPoint2, tangent2, 0.25f, Color.red, 202 ) ) {
                Undo.RecordObject(targetSpline, "Edited Spline");
                targetSpline.segment.controlPoint2.Set(controlPoint2);
                targetSpline.UpdateOtherSegments();
            }

            if (targetSpline.segment is BezierCubic bezierCubic) {

                Vector3 handle1 = bezierCubic.handle1;
                if ( DrawHandleGUI( ref handle1, 0.2f, Color.blue, 203 ) ) {
                    Undo.RecordObject(targetSpline, "Edited Cubic Spline");
                    bezierCubic.handle1 = handle1;
                    targetSpline.UpdateOtherSegments();
                }

                Vector3 handle2 = bezierCubic.handle2;
                if ( DrawHandleGUI( ref handle2, 0.2f, Color.blue, 204 ) ) {
                    Undo.RecordObject(targetSpline, "Edited Cubic Spline");
                    bezierCubic.handle2 = handle2;
                    targetSpline.UpdateOtherSegments();
                }

            }

            if (targetSpline.segment is BezierQuadratic bezierQuadratic) {

                Vector3 handle = bezierQuadratic.handle;
                if ( DrawHandleGUI( ref handle, 0.2f, Color.blue, 205 ) ) {
                    Undo.RecordObject(targetSpline, "Edited Quadratic Spline");
                    bezierQuadratic.handle = handle;
                    targetSpline.UpdateOtherSegments();
                }

            }

        }

        


        private bool DrawControlPointGUI( ref ControlPoint point, Vector3 tangent, float size, Color color, int id ) {

            float handleSize = HandleUtility.GetHandleSize(point.position);
            float moveHandleSize = handleSize * 0.2f;
            Handles.color = color;


            EditorGUI.BeginChangeCheck();


            point.position = Handles.FreeMoveHandle(id, point.position, Quaternion.identity, moveHandleSize, Vector3.zero, Handles.SphereHandleCap);

            if (GUIUtility.hotControl == id || selectedSegment == id) {
                point.position = Handles.PositionHandle(point.position, Quaternion.identity);
                selectedSegment = id;
            }


            Quaternion cpRotation = Quaternion.LookRotation(tangent, Quaternion.AngleAxis(point.upAngle, tangent) * Vector3.up);

            Quaternion newRotation = Handles.Disc(cpRotation, point.position, tangent, handleSize * 0.5f, false, 0f);
            Handles.DrawLine(point.position, point.position + (newRotation * Vector3.up * handleSize * 0.5f));
            point.upAngle = Mathf.Abs(newRotation.eulerAngles.z);


            return EditorGUI.EndChangeCheck();
            
        }

        private bool DrawHandleGUI( ref Vector3 point, float size, Color color, int id ) {

            float handleSize = HandleUtility.GetHandleSize(point) * 0.2f;
            Handles.color = color;


            EditorGUI.BeginChangeCheck();


            point = Handles.FreeMoveHandle(id, point, Quaternion.identity, handleSize, Vector3.zero, Handles.SphereHandleCap);

            if (GUIUtility.hotControl == id || selectedSegment == id) {
                point = Handles.PositionHandle(point, Quaternion.identity);
                selectedSegment = id;
            }
            

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
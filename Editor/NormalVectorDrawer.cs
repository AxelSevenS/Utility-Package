using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SevenGame.Utility.Editor {

    [CustomPropertyDrawer(typeof( NormalVectorAttribute ), true)]
    public class NormalVectorDrawer : PropertyDrawer {

        public override void OnGUI( Rect position, SerializedProperty property, GUIContent label ) {
            EditorGUI.BeginProperty( position, label, property );
            
            EditorGUI.BeginChangeCheck();

            EditorGUI.PropertyField( position, property, label, true);

            if (EditorGUI.EndChangeCheck()) {
                switch (property.propertyType) {
                    case SerializedPropertyType.Vector2:
                        property.vector2Value = property.vector2Value.normalized;
                        break;
                    case SerializedPropertyType.Vector3:
                        property.vector3Value = property.vector3Value.normalized;
                        break;
                    case SerializedPropertyType.Vector4:
                        property.vector4Value = property.vector4Value.normalized;
                        break;
                }
            }
            
            EditorGUI.EndProperty();
        }
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label){
            return EditorGUI.GetPropertyHeight(property, label, true);
        }
    }
}

using UnityEngine;
using UnityEditor;

using SevenGame.Utility;

namespace SevenGame.Utility {

    [CustomPropertyDrawer(typeof( ReadOnlyAttribute ), true)]
    public class ReadOnlyDrawer : PropertyDrawer {

        public override void OnGUI( Rect position, SerializedProperty property, GUIContent label ) {
            EditorGUI.BeginProperty( position, label, property );

            EditorGUI.BeginDisabledGroup(true);
            EditorGUI.PropertyField( position, property, label, true);
            EditorGUI.EndDisabledGroup();
            
            EditorGUI.EndProperty();
        }
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label){
            return EditorGUI.GetPropertyHeight(property, label, true);
        }
    }
}
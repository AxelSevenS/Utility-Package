using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;
using UnityEditorInternal;


namespace SevenGame.Utility {

    [CustomPropertyDrawer(typeof(SerializableStack<>), true)]
    public class SerializableStackDrawer : PropertyDrawer {

        private SerializedProperty _values = null;
        private ReorderableList _list = null;

        private bool showContents = true;



        private SerializedProperty GetValues(SerializedProperty property){
            if (_values == null) {
                _values = property.FindPropertyRelative("_values");
            }
            return _values;
        }

        private ReorderableList GetReorderableList(SerializedProperty property){
            
            var valuesProperty = GetValues(property);

            if (_list == null) {
                _list = new ReorderableList(valuesProperty.serializedObject, valuesProperty, true, false, true, true);
                _list.drawHeaderCallback = (Rect rect) => {
                    EditorGUI.LabelField(rect, property.displayName);
                };
                _list.drawNoneElementCallback = (Rect rect) => {
                    EditorGUI.LabelField(rect, "Stack is Empty");
                };
                _list.drawElementCallback = (UnityEngine.Rect rect, int index, bool isActive, bool isFocused) => {
                    rect.width -= 10;
                    rect.x += 10;
                    EditorGUI.PropertyField(rect, valuesProperty.GetArrayElementAtIndex(index), true);
                };
            }
            return _list;
        }


        public override void OnGUI( Rect position, SerializedProperty property, GUIContent label ) {
            EditorGUI.BeginProperty( position, label, property );

            var pairsProperty = GetValues(property);
            var reorderableList = GetReorderableList(property);
            
            Rect foldoutPosition = new Rect(position.xMin, position.yMin, position.width, EditorGUIUtility.singleLineHeight);

            showContents = EditorGUI.BeginFoldoutHeaderGroup(foldoutPosition, showContents, property.displayName);
            if (showContents) {
                if (pairsProperty.arraySize != 0) {
                    var height = 0f;
                    for(var i = 0; i < pairsProperty.arraySize; i++) {
                        height = Mathf.Max(height, EditorGUI.GetPropertyHeight(pairsProperty.GetArrayElementAtIndex(i)));
                    }
                    reorderableList.elementHeight = height;
                }
                foldoutPosition.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                reorderableList.DoList(foldoutPosition);
            }
            EditorGUI.EndFoldoutHeaderGroup();

            EditorGUI.EndProperty();
        }
        
        public override float GetPropertyHeight (SerializedProperty property, GUIContent label) {
    
            float height = EditorGUIUtility.singleLineHeight;
            if (showContents)
                height += GetReorderableList(property).GetHeight() + EditorGUIUtility.standardVerticalSpacing;

            return height;
        }
    }
}
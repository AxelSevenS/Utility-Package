using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;
using UnityEditorInternal;


namespace SevenGame.Utility {

    [CustomPropertyDrawer(typeof(SerializableDictionary<,>), true)]
    public class SerializableDictionaryDrawer : PropertyDrawer {

        private SerializedProperty _pairs = null;
        private ReorderableList _list = null;

        private bool showContents = true;



        private SerializedProperty GetPairs(SerializedProperty property){
            return _pairs ??= property.FindPropertyRelative("_pairs");
        }

        private ReorderableList GetReorderableList(SerializedProperty property){
            
            SerializedProperty pairsProperty = GetPairs(property);

            return _list ??= new ReorderableList(pairsProperty.serializedObject, pairsProperty, true, false, true, true) {
                drawHeaderCallback = (Rect rect) => {
                    EditorGUI.LabelField(rect, property.displayName);
                },
                drawNoneElementCallback = (Rect rect) => {
                    EditorGUI.LabelField(rect, "Dictionary is Empty");
                },
                drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
                    rect.width -= 10;
                    rect.x += 10;
                    EditorGUI.PropertyField(rect, pairsProperty.GetArrayElementAtIndex(index), true);
                },
                onAddCallback = (ReorderableList list) => {
                    int index = list.serializedProperty.arraySize;
                    list.serializedProperty.arraySize++;
                    list.index = index;
                    SerializedProperty element = list.serializedProperty.GetArrayElementAtIndex(index);
                    SevenEditorUtility.ResetValue(element.FindPropertyRelative("Key"));
                }
            };
        }


        public override void OnGUI( Rect position, SerializedProperty property, GUIContent label ) {
            EditorGUI.BeginProperty( position, label, property );

            SerializedProperty pairsProperty = GetPairs(property);
            ReorderableList reorderableList = GetReorderableList(property);
            
            Rect foldoutPosition = new(position.xMin, position.yMin, position.width, EditorGUIUtility.singleLineHeight);

            showContents = EditorGUI.BeginFoldoutHeaderGroup(foldoutPosition, showContents, property.displayName);
            if (showContents) {
                if (pairsProperty.arraySize != 0) {
                    float height = 0f;
                    for(int i = 0; i < pairsProperty.arraySize; i++) {
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
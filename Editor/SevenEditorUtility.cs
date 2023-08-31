using System;

using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;
using System.Collections;
using System.Reflection;

namespace SevenGame.Utility {

    public class SevenEditorUtility {
        
        public static void ResetValue(SerializedProperty property) {
			if (property == null)
				throw new ArgumentNullException("property");

			switch (property.propertyType) {
				case SerializedPropertyType.Integer:
					property.intValue = 0;
					break;
				case SerializedPropertyType.Boolean:
					property.boolValue = false;
					break;
				case SerializedPropertyType.Float:
					property.floatValue = 0f;
					break;
				case SerializedPropertyType.String:
					property.stringValue = "";
					break;
				case SerializedPropertyType.Color:
					property.colorValue = Color.black;
					break;
				case SerializedPropertyType.ObjectReference:
					property.objectReferenceValue = null;
					break;
				case SerializedPropertyType.LayerMask:
					property.intValue = 0;
					break;
				case SerializedPropertyType.Enum:
					property.enumValueIndex = 0;
					break;
				case SerializedPropertyType.Vector2:
					property.vector2Value = default(Vector2);
					break;
				case SerializedPropertyType.Vector3:
					property.vector3Value = default(Vector3);
					break;
				case SerializedPropertyType.Vector4:
					property.vector4Value = default(Vector4);
					break;
				case SerializedPropertyType.Rect:
					property.rectValue = default(Rect);
					break;
				case SerializedPropertyType.ArraySize:
					property.intValue = 0;
					break;
				case SerializedPropertyType.Character:
					property.intValue = 0;
					break;
				case SerializedPropertyType.AnimationCurve:
					property.animationCurveValue = AnimationCurve.Linear(0f, 0f, 1f, 1f);
					break;
				case SerializedPropertyType.Bounds:
					property.boundsValue = default(Bounds);
					break;
				case SerializedPropertyType.Gradient:
					//!TODO: Amend when Unity add a public API for setting the gradient.
					break;
                case SerializedPropertyType.Quaternion:
                    property.quaternionValue = Quaternion.identity;
                    break;
                case SerializedPropertyType.ExposedReference:
                    property.exposedReferenceValue = null;
                    break;
                case SerializedPropertyType.FixedBufferSize:
                    property.intValue = 0;
                    break;
                case SerializedPropertyType.Vector2Int:
                    property.vector2IntValue = default(Vector2Int);
                    break;
                case SerializedPropertyType.Vector3Int:
                    property.vector3IntValue = default(Vector3Int);
                    break;
                case SerializedPropertyType.RectInt:
                    property.rectIntValue = default(RectInt);
                    break;
                case SerializedPropertyType.BoundsInt:
                    property.boundsIntValue = default(BoundsInt);
                    break;
                case SerializedPropertyType.ManagedReference:
                    property.managedReferenceValue = null;
                    break;
			}

			if (property.isArray) {
				property.arraySize = 0;
			}

			ResetChildPropertyValues(property);
		}

		private static void ResetChildPropertyValues(SerializedProperty element) {
			if (!element.hasChildren)
				return;

			var childProperty = element.Copy();
			int elementPropertyDepth = element.depth;
			bool enterChildren = true;

			while (childProperty.Next(enterChildren) && childProperty.depth > elementPropertyDepth) {
				enterChildren = false;
				ResetValue(childProperty);
			}
		}
        public static float lineSpace = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        public static object GetTargetObject(SerializedProperty prop) {

            SerializedObject serializedObject = prop.serializedObject;
            string path = prop.propertyPath;
        
            object propertyObject = serializedObject == null || serializedObject.targetObject == null ? null : serializedObject.targetObject;
            System.Type objectType = propertyObject == null ? null : propertyObject.GetType();
            
            if (!string.IsNullOrEmpty(path) && propertyObject != null) {
                string[] splitPath = path.Split('.');
                Type fieldType = null;

                //work through the given property path, node by node
                for (int i = 0; i < splitPath.Length; i++) {
                    string pathNode = splitPath[i];

                    //both arrays and lists implement the IList interface
                    if (fieldType != null && typeof(IList).IsAssignableFrom(fieldType)) {
                        //IList items are serialized like this: `Array.data[0]`
                        Debug.AssertFormat(pathNode.Equals("Array", StringComparison.Ordinal), serializedObject.targetObject, "Expected path node 'Array', but found '{0}'", pathNode);

                        //just skip the `Array` part of the path
                        pathNode = splitPath[++i];

                        //match the `data[0]` part of the path and extract the IList item index
                        Regex matchArrayElement = new Regex(@"^data\[(\d+)\]$");
                        Match elementMatch = matchArrayElement.Match(pathNode);
                        int index;
                        if (elementMatch.Success && int.TryParse(elementMatch.Groups[1].Value, out index)) {
                            IList objectArray = (IList)propertyObject;
                            bool validArrayEntry = objectArray != null && index < objectArray.Count;
                            propertyObject = validArrayEntry ? objectArray[index] : null;
                            objectType = fieldType.IsArray
                                ? fieldType.GetElementType()          //only set for arrays
                                : fieldType.GenericTypeArguments[0];  //type of `T` in List<T>
                        }
                        else {
                            Debug.LogErrorFormat(serializedObject.targetObject, "Unexpected path format for array item: '{0}'", pathNode);
                        }
                        //reset fieldType, so we don't end up in the IList branch again next iteration
                        fieldType = null;

                    } else {

                        FieldInfo field;
                        Type instanceType = objectType;
                        BindingFlags fieldBindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;
                        do {
                            field = instanceType.GetField(pathNode, fieldBindingFlags);

                            //b/c a private, serialized field of a subclass isn't directly retrievable,
                            fieldBindingFlags = BindingFlags.Instance | BindingFlags.NonPublic;
                            //if neccessary, work up the inheritance chain until we find it.
                            instanceType = instanceType.BaseType;
                        }
                        while (field == null && instanceType != typeof(object));

                        //store object info for next iteration or to return
                        propertyObject = field == null || propertyObject == null ? null : field.GetValue(propertyObject);
                        fieldType = field == null ? null : field.FieldType;
                        objectType = fieldType;
                    }
                }
            }
            
            return propertyObject;
            
        }

        public static T GetTargetObject<T>(SerializedProperty prop) where T : class {
            
            SerializedObject serializedObject = prop.serializedObject;
            string path = prop.propertyPath;

            Type fieldType = typeof(T);
        
            object propertyObject = serializedObject == null || serializedObject.targetObject == null ? null : serializedObject.targetObject;
            if ( !(propertyObject.GetType().IsSubclassOf(fieldType) || propertyObject is T) ) return null;

            T TObject = propertyObject as T;

            if (!string.IsNullOrEmpty(path) && TObject != null) {
                string[] splitPath = path.Split('.');

                //work through the given property path, node by node
                for (int i = 0; i < splitPath.Length; i++) {
                    string pathNode = splitPath[i];

                    //both arrays and lists implement the IList interface
                    if (fieldType != null && typeof(IList).IsAssignableFrom(fieldType)) {
                        //IList items are serialized like this: `Array.data[0]`
                        Debug.AssertFormat(pathNode.Equals("Array", StringComparison.Ordinal), serializedObject.targetObject, "Expected path node 'Array', but found '{0}'", pathNode);

                        //just skip the `Array` part of the path
                        pathNode = splitPath[++i];

                        //match the `data[0]` part of the path and extract the IList item index
                        Regex matchArrayElement = new Regex(@"^data\[(\d+)\]$");
                        Match elementMatch = matchArrayElement.Match(pathNode);
                        int index;
                        if (elementMatch.Success && int.TryParse(elementMatch.Groups[1].Value, out index)) {
                            IList objectArray = (IList)TObject;
                            bool validArrayEntry = objectArray != null && index < objectArray.Count;
                            TObject = validArrayEntry ? (T)objectArray[index] : null;
                        }
                        else {
                            Debug.LogErrorFormat(serializedObject.targetObject, "Unexpected path format for array item: '{0}'", pathNode);
                        }
                        //reset fieldType, so we don't end up in the IList branch again next iteration
                        fieldType = null;

                    } else {

                        FieldInfo field;
                        Type instanceType = typeof(T);
                        BindingFlags fieldBindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;
                        do {
                            field = instanceType.GetField(pathNode, fieldBindingFlags);

                            //b/c a private, serialized field of a subclass isn't directly retrievable,
                            fieldBindingFlags = BindingFlags.Instance | BindingFlags.NonPublic;
                            //if neccessary, work up the inheritance chain until we find it.
                            instanceType = instanceType.BaseType;
                        }
                        while (field == null && instanceType != typeof(object));

                        //store object info for next iteration or to return
                        TObject = field == null || TObject == null ? null : (T)field.GetValue(TObject);
                        fieldType = field == null ? null : field.FieldType;
                    }
                }
            }
            
            return TObject;
        }
    }
}
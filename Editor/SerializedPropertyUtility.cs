using System;

using UnityEngine;
using UnityEditor;

namespace SevenGame.Utility.Editor {
    public class SerializedPropertyUtility {
        
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
    }
}
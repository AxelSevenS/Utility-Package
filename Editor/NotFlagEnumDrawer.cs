using System;
using System.Reflection;

using UnityEditor;
using UnityEngine;

namespace SevenGame.Utility.Editor {
	
	[CustomPropertyDrawer(typeof(NotFlagEnumAttribute))]
	public class NotFlagEnumDrawer : PropertyDrawer {

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {

			if (property.propertyType != SerializedPropertyType.Enum) {
				Debug.LogError("NotFlagEnumAttribute can only be used on enums");
				return;
			}

			NotFlagEnumAttribute flagSettings = (NotFlagEnumAttribute)attribute;
			Enum targetEnum = (Enum)Enum.ToObject(fieldInfo.FieldType, property.intValue);

			string propName = string.IsNullOrEmpty(flagSettings.displayName) ? ObjectNames.NicifyVariableName(property.name) : flagSettings.displayName;

			EditorGUI.BeginChangeCheck();
			EditorGUI.BeginProperty(position, label, property);

            Enum newValue = EditorGUI.EnumPopup(position, propName, targetEnum);
			// int newValue = EditorGUI.MaskField(position, label, property.intValue, property.enumNames);

			if (!property.hasMultipleDifferentValues || EditorGUI.EndChangeCheck()) {
                int value = Convert.ToInt32(newValue);
                property.intValue = value;
            }

			EditorGUI.EndProperty();
		}
	}
}
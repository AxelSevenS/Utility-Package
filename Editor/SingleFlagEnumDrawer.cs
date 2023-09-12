using System;

using UnityEditor;
using UnityEngine;

namespace SevenGame.Utility {
	
	[CustomPropertyDrawer(typeof(SingleFlagEnumAttribute))]
	public class SingleFlagEnumDrawer : PropertyDrawer {

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {

			if (property.propertyType != SerializedPropertyType.Enum) {
				Debug.LogError($"{nameof(SingleFlagEnumAttribute)} can only be used on enums");
				return;
			}

			SingleFlagEnumAttribute flagSettings = (SingleFlagEnumAttribute)attribute;
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
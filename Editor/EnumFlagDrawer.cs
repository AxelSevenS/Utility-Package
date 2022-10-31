using System;
using System.Reflection;

using UnityEditor;
using UnityEngine;

namespace SevenGame.Utility {
	
	[CustomPropertyDrawer(typeof(EnumFlagAttribute))]
	public class EnumFlagDrawer : PropertyDrawer {

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {

			if (property.propertyType != SerializedPropertyType.Enum) {
				Debug.LogError("EnumFlagAttribute can only be used on enums");
				return;
			}

			EnumFlagAttribute flagSettings = (EnumFlagAttribute)attribute;
			Enum targetEnum = (Enum)Enum.ToObject(fieldInfo.FieldType, property.intValue);

			string propName = string.IsNullOrEmpty(flagSettings.displayName) ? ObjectNames.NicifyVariableName(property.name) : flagSettings.displayName;

			EditorGUI.BeginChangeCheck();
			EditorGUI.BeginProperty(position, label, property);

			int newValue = EditorGUI.MaskField(position, label, property.intValue, property.enumNames);

			if (!property.hasMultipleDifferentValues || EditorGUI.EndChangeCheck())
				property.intValue = newValue;

			EditorGUI.EndProperty();
		}
	}
}
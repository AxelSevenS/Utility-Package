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

			// string propName = flagSettings.displayName;
			// if (string.IsNullOrEmpty(propName))
			// 	propName = ObjectNames.NicifyVariableName(property.name);
			string propName = string.IsNullOrEmpty(flagSettings.displayName) ? ObjectNames.NicifyVariableName(property.name) : flagSettings.displayName;

			EditorGUI.BeginChangeCheck();
			EditorGUI.BeginProperty(position, label, property);

			Enum enumNew = EditorGUI.EnumFlagsField(position, propName, targetEnum);

			if (!property.hasMultipleDifferentValues || EditorGUI.EndChangeCheck())
				property.enumValueIndex = (int)Convert.ChangeType(enumNew, targetEnum.GetType());

			EditorGUI.EndProperty();
		}
	}
}
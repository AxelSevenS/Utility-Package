using System;

using UnityEngine;

namespace SevenGame.Utility {

	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public class NotFlagEnumAttribute : PropertyAttribute {

		// Used to draw an Enum Flag as a regular dropdown Enum; Use this to disallow the use of flags on an Enum, even if it's marked as a Flags Enum
		// Useful if you want a Flag Enum value to only have one value at a time.

		public string displayName;

		public NotFlagEnumAttribute() {}

		public NotFlagEnumAttribute(string name) {
			displayName = name;
		}
	}
}
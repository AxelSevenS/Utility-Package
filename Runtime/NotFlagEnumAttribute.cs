using System;

using UnityEngine;

namespace SevenGame.Utility {

	/// <summary>
	/// <para>
	/// Used to draw a Flags Enum as a regular unary Enum and to disallow the use of flags, even if it's marked as a Flags Enum.
	/// </para>
	/// <para>
	/// Useful if you want a Flag Enum value to only have one value at a time.
	/// </para>
	/// </summary>
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public class NotFlagEnumAttribute : PropertyAttribute {

		public string displayName;

		public NotFlagEnumAttribute() {}

		public NotFlagEnumAttribute(string name) {
			displayName = name;
		}
	}
}
using System;

using UnityEngine;

namespace SevenGame.Utility {

	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public class EnumFlagAttribute : PropertyAttribute {
		public string displayName;

		public EnumFlagAttribute() {}

		public EnumFlagAttribute(string name) {
			displayName = name;
		}
	}
}
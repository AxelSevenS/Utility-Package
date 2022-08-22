using UnityEngine;

namespace SevenGame.Utility {
	public class EnumFlagAttribute : PropertyAttribute {
		public string displayName;

		public EnumFlagAttribute() {}

		public EnumFlagAttribute(string name) {
			displayName = name;
		}
	}
}
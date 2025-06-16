using System;

namespace ParadoxNotion.Design;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class ColorAttribute : Attribute {
	public string hexColor;

	public ColorAttribute(string hexColor) {
		this.hexColor = hexColor;
	}
}
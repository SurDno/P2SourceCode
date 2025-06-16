using System;

namespace ParadoxNotion.Design;

[AttributeUsage(AttributeTargets.Field)]
public class PopupFieldAttribute : Attribute {
	public object[] values;
	public string staticPath;

	public PopupFieldAttribute(params object[] values) {
		this.values = values;
	}

	public PopupFieldAttribute(string staticPath) {
		this.staticPath = staticPath;
	}
}
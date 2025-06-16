using System;

namespace BehaviorDesigner.Runtime.Tasks;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class TaskIconAttribute : Attribute {
	private readonly string mIconPath;

	public string IconPath => mIconPath;

	public TaskIconAttribute(string iconPath) {
		mIconPath = iconPath;
	}
}
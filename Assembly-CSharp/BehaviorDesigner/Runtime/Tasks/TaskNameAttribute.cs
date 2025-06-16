using System;

namespace BehaviorDesigner.Runtime.Tasks;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class TaskNameAttribute : Attribute {
	private readonly string mName;

	public string Name => mName;

	public TaskNameAttribute(string name) {
		mName = name;
	}
}
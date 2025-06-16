using System;

namespace BehaviorDesigner.Runtime.Tasks;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class TaskCategoryAttribute : Attribute {
	private readonly string mCategory;

	public string Category => mCategory;

	public TaskCategoryAttribute(string category) {
		mCategory = category;
	}
}
using System;

namespace PLVirtualMachine.Common.Data;

[AttributeUsage(AttributeTargets.Field)]
public class FieldDataAttribute : Attribute {
	public readonly string Name;
	public readonly DataFieldType DataFieldType;

	public FieldDataAttribute(string name, DataFieldType dataFieldType = DataFieldType.None) {
		Name = name;
		DataFieldType = dataFieldType;
	}
}
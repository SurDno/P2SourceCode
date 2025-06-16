using System;

namespace PLVirtualMachine.Common.VMSpecialAttributes;

[AttributeUsage(AttributeTargets.Property)]
public class SpecialPropertyAttribute : Attribute {
	public readonly ESpecialPropertyName Name;

	public SpecialPropertyAttribute(ESpecialPropertyName specialName) {
		Name = specialName;
	}
}
using System;

namespace PLVirtualMachine.Common.VMSpecialAttributes;

[AttributeUsage(AttributeTargets.Event)]
public class SpecialEventAttribute : Attribute {
	public readonly ESpecialEventName Name;

	public SpecialEventAttribute(ESpecialEventName specialName) {
		Name = specialName;
	}
}
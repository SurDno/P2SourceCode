using System;
using System.Collections.Generic;
using Cofe.Meta;

namespace PLVirtualMachine.Common;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class VMFactoryAttribute : TypeAttribute {
	private Type type;
	private static Dictionary<Type, Type> types = new();

	public static bool TryGetValue(Type type, out Type result) {
		return types.TryGetValue(type, out result);
	}

	public VMFactoryAttribute(Type type) {
		this.type = type;
	}

	public override void ComputeType(Type type) {
		types.Add(this.type, type);
	}
}
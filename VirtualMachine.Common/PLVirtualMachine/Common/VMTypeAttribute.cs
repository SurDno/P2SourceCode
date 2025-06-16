using System;
using System.Collections.Generic;
using Cofe.Meta;

namespace PLVirtualMachine.Common;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, Inherited = false)]
public class VMTypeAttribute : TypeAttribute {
	public readonly string TypeName;
	private static Dictionary<string, Type> types = new();
	private static Dictionary<Type, string> names = new();

	public VMTypeAttribute(string typeName) {
		TypeName = typeName;
	}

	public override void ComputeType(Type type) {
		RegisterVMBaseType(TypeName, type);
	}

	private static void RegisterVMBaseType(string baseTypeName, Type vmType) {
		if (!types.ContainsKey(baseTypeName))
			types.Add(baseTypeName, vmType);
		else if (vmType.IsAssignableFrom(types[baseTypeName]))
			types[baseTypeName] = vmType;
		names.Add(vmType, baseTypeName);
	}

	public static bool TryGetValue(string name, out Type result) {
		return types.TryGetValue(name, out result);
	}

	public static bool TryGetValue(Type type, out string result) {
		return names.TryGetValue(type, out result);
	}
}
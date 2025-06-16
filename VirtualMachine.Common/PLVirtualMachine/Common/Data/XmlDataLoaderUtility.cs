using System;
using System.Collections.Generic;
using Cofe.Utility;
using Engine.Common.Types;

namespace PLVirtualMachine.Common.Data;

public static class XmlDataLoaderUtility {
	private static Dictionary<EDataType, Type> objectIdToType = new();

	static XmlDataLoaderUtility() {
		Init();
	}

	public static Type GetObjTypeById(ulong id) {
		var typeId = GuidUtility.GetTypeId(id);
		Type objTypeById;
		objectIdToType.TryGetValue((EDataType)typeId, out objTypeById);
		return objTypeById;
	}

	private static void Init() {
		ComputeTypes();
	}

	private static void ComputeTypes() {
		AssemblyUtility.ComputeAssemblies(typeof(TypeDataAttribute).Assembly, assembly => {
			foreach (var type in assembly.GetTypes())
				if (type.IsClass && type.IsDefined(typeof(TypeDataAttribute), false)) {
					var customAttribute =
						(TypeDataAttribute)type.GetCustomAttributes(typeof(TypeDataAttribute), false)[0];
					objectIdToType.Add(customAttribute.DataType, type);
				}
		});
	}
}
using System;
using System.Collections.Generic;
using Cofe.Utility;

namespace Inspectors;

public static class DerivedTypeService {
	private static Dictionary<Type, Type[]> types = new();

	public static Type[] GetDerivedTypes(Type baseType) {
		Type[] array;
		if (!types.TryGetValue(baseType, out array)) {
			var list = new List<Type>();
			if (!baseType.IsAbstract)
				list.Add(baseType);
			if (baseType.IsClass || baseType.IsInterface)
				AssemblyUtility.ComputeAssemblies(baseType.Assembly, assembly => {
					foreach (var type in assembly.GetTypes())
						if (type.IsClass && !type.IsAbstract && !type.Name.EndsWith("_Generated") &&
						    TypeUtility.IsAssignableFrom(baseType, type))
							list.Add(type);
				});
			list.Sort((a, b) => a.Name.CompareTo(b.Name));
			array = list.ToArray();
			types.Add(baseType, array);
		}

		return array;
	}
}
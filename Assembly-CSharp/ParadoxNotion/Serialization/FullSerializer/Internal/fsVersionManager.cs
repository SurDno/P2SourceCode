using System;
using System.Collections.Generic;
using System.Reflection;

namespace ParadoxNotion.Serialization.FullSerializer.Internal;

public static class fsVersionManager {
	private static readonly Dictionary<Type, fsOption<fsVersionedType>> _cache = new();

	public static fsResult GetVersionImportPath(
		string currentVersion,
		fsVersionedType targetVersion,
		out List<fsVersionedType> path) {
		path = new List<fsVersionedType>();
		if (!GetVersionImportPathRecursive(path, currentVersion, targetVersion))
			return fsResult.Fail("There is no migration path from \"" + currentVersion + "\" to \"" +
			                     targetVersion.VersionString + "\"");
		path.Add(targetVersion);
		return fsResult.Success;
	}

	private static bool GetVersionImportPathRecursive(
		List<fsVersionedType> path,
		string currentVersion,
		fsVersionedType current) {
		for (var index = 0; index < current.Ancestors.Length; ++index) {
			var ancestor = current.Ancestors[index];
			if (ancestor.VersionString == currentVersion ||
			    GetVersionImportPathRecursive(path, currentVersion, ancestor)) {
				path.Add(ancestor);
				return true;
			}
		}

		return false;
	}

	public static fsOption<fsVersionedType> GetVersionedType(Type type) {
		fsOption<fsVersionedType> versionedType1;
		if (!_cache.TryGetValue(type, out versionedType1)) {
			var attribute = fsPortableReflection.GetAttribute<fsObjectAttribute>(type);
			if (attribute != null &&
			    (!string.IsNullOrEmpty(attribute.VersionString) || attribute.PreviousModels != null)) {
				if (attribute.PreviousModels != null && string.IsNullOrEmpty(attribute.VersionString))
					throw new Exception("fsObject attribute on " + type +
					                    " contains a PreviousModels specifier - it must also include a VersionString modifier");
				var fsVersionedTypeArray =
					new fsVersionedType[attribute.PreviousModels != null ? attribute.PreviousModels.Length : 0];
				for (var index = 0; index < fsVersionedTypeArray.Length; ++index) {
					var versionedType2 = GetVersionedType(attribute.PreviousModels[index]);
					fsVersionedTypeArray[index] = !versionedType2.IsEmpty
						? versionedType2.Value
						: throw new Exception("Unable to create versioned type for ancestor " + versionedType2 +
						                      "; please add an [fsObject(VersionString=\"...\")] attribute");
				}

				var type1 = new fsVersionedType {
					Ancestors = fsVersionedTypeArray,
					VersionString = attribute.VersionString,
					ModelType = type
				};
				VerifyUniqueVersionStrings(type1);
				VerifyConstructors(type1);
				versionedType1 = fsOption.Just(type1);
			}

			_cache[type] = versionedType1;
		}

		return versionedType1;
	}

	private static void VerifyConstructors(fsVersionedType type) {
		var declaredConstructors = type.ModelType.GetDeclaredConstructors();
		for (var index1 = 0; index1 < type.Ancestors.Length; ++index1) {
			var modelType = type.Ancestors[index1].ModelType;
			var flag = false;
			for (var index2 = 0; index2 < declaredConstructors.Length; ++index2) {
				var parameters = declaredConstructors[index2].GetParameters();
				if (parameters.Length == 1 && parameters[0].ParameterType == modelType) {
					flag = true;
					break;
				}
			}

			if (!flag)
				throw new fsMissingVersionConstructorException(type.ModelType, modelType);
		}
	}

	private static void VerifyUniqueVersionStrings(fsVersionedType type) {
		var dictionary = new Dictionary<string, Type>();
		var fsVersionedTypeQueue = new Queue<fsVersionedType>();
		fsVersionedTypeQueue.Enqueue(type);
		while (fsVersionedTypeQueue.Count > 0) {
			var fsVersionedType = fsVersionedTypeQueue.Dequeue();
			if (dictionary.ContainsKey(fsVersionedType.VersionString) &&
			    dictionary[fsVersionedType.VersionString] != fsVersionedType.ModelType)
				throw new fsDuplicateVersionNameException(dictionary[fsVersionedType.VersionString],
					fsVersionedType.ModelType, fsVersionedType.VersionString);
			dictionary[fsVersionedType.VersionString] = fsVersionedType.ModelType;
			foreach (var ancestor in fsVersionedType.Ancestors)
				fsVersionedTypeQueue.Enqueue(ancestor);
		}
	}
}
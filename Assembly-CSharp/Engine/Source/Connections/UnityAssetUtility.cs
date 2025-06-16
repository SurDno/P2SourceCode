using System;
using AssetDatabases;
using Cofe.Utility;
using Object = UnityEngine.Object;

namespace Engine.Source.Connections;

public static class UnityAssetUtility {
	public static T GetValue<T>(Guid id) where T : Object {
		var path = AssetDatabaseService.Instance.GetPath(id);
		return path.IsNullOrEmpty() ? default : AssetDatabaseService.Instance.Load<T>(path);
	}

	public static T GetValue<T>(Guid id, string name) where T : Object {
		var path = AssetDatabaseService.Instance.GetPath(id);
		if (path.IsNullOrEmpty())
			return default;
		if (name.IsNullOrEmpty())
			return AssetDatabaseService.Instance.Load<T>(path);
		foreach (var @object in AssetDatabaseService.Instance.LoadAll(path))
			if (@object.name == name) {
				var obj = @object as T;
				if (obj != null)
					return obj;
			}

		return default;
	}
}
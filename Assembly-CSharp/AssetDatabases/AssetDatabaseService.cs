using Cofe.Utility;
using UnityEngine;

namespace AssetDatabases;

public static class AssetDatabaseService {
	private static IAssetDatabase instance;

	public static IAssetDatabase Instance {
		get {
			if (instance == null) {
				instance = new AssetDatabaseBuild();
				Debug.Log(ObjectInfoUtility.GetStream().Append("IAssetDatabase").Append(" : ")
					.Append(TypeUtility.GetTypeName(instance.GetType())));
			}

			return instance;
		}
	}
}
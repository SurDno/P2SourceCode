using System;
using System.IO;
using AssetDatabases;
using Cofe.Meta;
using Engine.Common.Generator;
using Inspectors;
using UnityEngine;

namespace Engine.Source.Settings.External;

public class ExternalSettingsInstance<T> where T : ExternalSettingsInstance<T> {
	[DataReadProxy] [DataWriteProxy] [CopyableProxy()] [Inspected(Mutable = true, Mode = ExecuteMode.EditAndRuntime)]
	public int Version;

	private static T instance;

	public static T Instance {
		get {
			if (instance == null) {
				MetaService.Initialise("[Engine]");
				var flag = !Application.isEditor;
				instance = LoadFromResources("Assets/Data/Settings/Resources/" + typeof(T).Name + ".xml");
				if (flag) {
					var str = "{DataPath}/Settings/".Replace("{DataPath}", Application.persistentDataPath) +
					          typeof(T).Name + ".xml";
					var obj = default(T);
					if (File.Exists(str))
						try {
							obj = LoadFromFile(str);
						} catch (Exception ex) {
							Debug.LogError("Error load settings : " + str + " , ex : " + ex);
						}

					if (obj == null || obj.Version != instance.Version) {
						var fileInfo = new FileInfo(str);
						if (!fileInfo.Directory.Exists)
							fileInfo.Directory.Create();
						SerializeUtility.Serialize(str, instance);
					} else
						instance = obj;
				}
			}

			return instance;
		}
	}

	private static T LoadFromFile(string path) {
		return SerializeUtility.Deserialize<T>(path);
	}

	private static T LoadFromResources(string path) {
		var textAsset = AssetDatabaseService.Instance.Load<TextAsset>(path);
		if (!(textAsset != null) || textAsset.bytes == null)
			return default;
		using (var memoryStream = new MemoryStream(textAsset.bytes)) {
			return SerializeUtility.Deserialize<T>(memoryStream, path);
		}
	}
}
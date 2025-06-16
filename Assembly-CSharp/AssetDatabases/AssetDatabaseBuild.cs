using System;
using System.Collections.Generic;
using System.Reflection;
using Cofe.Utility;
using Engine.Common.Comparers;
using Scripts.AssetDatabaseService;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace AssetDatabases;

public class AssetDatabaseBuild : IAssetDatabase {
	private Dictionary<Guid, string> paths = new(GuidComparer.Instance);
	private Dictionary<string, Guid> ids = new();

	public void RegisterAssets() {
		var assetDatabaseMapData =
			AssetDatabaseUtility.LoadFromFile<AssetDatabaseMapData>(
				PlatformUtility.GetPath("Data/Database/AssetDatabaseData.xml"));
		var count = assetDatabaseMapData.Items.Count;
		paths = new Dictionary<Guid, string>(count, GuidComparer.Instance);
		ids = new Dictionary<string, Guid>(count);
		foreach (var databaseMapItemData in assetDatabaseMapData.Items) {
			var key = new Guid(databaseMapItemData.Id);
			paths.Add(key, databaseMapItemData.Name);
			ids.Add(databaseMapItemData.Name, key);
		}
	}

	public int GetAllAssetPathsCount() {
		return paths.Count;
	}

	public IEnumerable<string> GetAllAssetPaths() {
		return paths.Values;
	}

	public Guid GetId(string path) {
		Guid id;
		ids.TryGetValue(path, out id);
		return id;
	}

	public string GetPath(Guid id) {
		string str;
		return paths.TryGetValue(id, out str) ? str : "";
	}

	public T Load<T>(string path) where T : Object {
		if (!path.IsNullOrEmpty()) {
			var resourcePath = AssetDatabaseUtility.ConvertToResourcePath(path);
			if (!resourcePath.IsNullOrEmpty())
				return Resources.Load<T>(resourcePath);
		}

		return default;
	}

	public Object[] LoadAll(string path) {
		if (!path.IsNullOrEmpty()) {
			var resourcePath = AssetDatabaseUtility.ConvertToResourcePath(path);
			if (!resourcePath.IsNullOrEmpty())
				return Resources.LoadAll(resourcePath);
		}

		return null;
	}

	public IAsyncLoad LoadAsync<T>(string path) where T : Object {
		if (!path.IsNullOrEmpty()) {
			var resourcePath = AssetDatabaseUtility.ConvertToResourcePath(path);
			if (!resourcePath.IsNullOrEmpty()) {
				var operation = Resources.LoadAsync<T>(resourcePath);
				if (operation != null)
					return new AsyncLoadFromResources(operation);
			}
		}

		Debug.LogError(MethodBase.GetCurrentMethod().Name + " wrong async operator : " + path);
		return null;
	}

	public IAsyncLoad LoadSceneAsync(string path) {
		SceneManager.LoadSceneAsync(path, LoadSceneMode.Additive);
		return new AsyncLoadScene(path);
	}

	public void Unload(Object obj) {
		if (!(obj != null))
			return;
		Resources.UnloadAsset(obj);
		obj = null;
	}
}
using System;
using AssetDatabases;
using Cofe.Serializations.Converters;
using Engine.Common;
using Engine.Common.Services;
using UnityEngine;

namespace Engine.Source.Connections;

[Serializable]
public class SceneAssetLink {
	[SerializeField] private string id;

	public Guid Id => DefaultConverter.ParseGuid(id);

	public string Path => AssetDatabaseService.Instance.GetPath(Id);

	public IScene GetTemplate() {
		var guid = DefaultConverter.ParseGuid(id);
		return ServiceLocator.GetService<ITemplateService>().GetTemplate<IScene>(guid);
	}

	public void SetTemplate(IScene obj) {
		id = obj == null || !(obj.Id != Guid.Empty) ? "" : obj.Id.ToString();
	}

	public override int GetHashCode() {
		return id != null ? id.GetHashCode() : 0;
	}

	public override bool Equals(object obj) {
		if (obj == null)
			return false;
		var sceneAssetLink = obj as SceneAssetLink;
		return (object)sceneAssetLink != null && id == sceneAssetLink.id;
	}

	public static bool operator ==(SceneAssetLink a, SceneAssetLink b) {
		if (a == (object)b)
			return true;
		return (object)a != null && (object)b != null && a.Equals(b);
	}

	public static bool operator !=(SceneAssetLink a, SceneAssetLink b) {
		return !(a == b);
	}
}
using AssetDatabases;
using Cofe.Serializations.Converters;
using Engine.Common;
using Engine.Common.Services;
using System;
using UnityEngine;

namespace Engine.Source.Connections
{
  [Serializable]
  public class SceneAssetLink
  {
    [SerializeField]
    private string id;

    public Guid Id => DefaultConverter.ParseGuid(this.id);

    public string Path => AssetDatabaseService.Instance.GetPath(this.Id);

    public IScene GetTemplate()
    {
      Guid guid = DefaultConverter.ParseGuid(this.id);
      return ServiceLocator.GetService<ITemplateService>().GetTemplate<IScene>(guid);
    }

    public void SetTemplate(IScene obj)
    {
      this.id = obj == null || !(obj.Id != Guid.Empty) ? "" : obj.Id.ToString();
    }

    public override int GetHashCode() => this.id != null ? this.id.GetHashCode() : 0;

    public override bool Equals(object obj)
    {
      if (obj == null)
        return false;
      SceneAssetLink sceneAssetLink = obj as SceneAssetLink;
      return (object) sceneAssetLink != null && this.id == sceneAssetLink.id;
    }

    public static bool operator ==(SceneAssetLink a, SceneAssetLink b)
    {
      if ((object) a == (object) b)
        return true;
      return (object) a != null && (object) b != null && a.Equals((object) b);
    }

    public static bool operator !=(SceneAssetLink a, SceneAssetLink b) => !(a == b);
  }
}

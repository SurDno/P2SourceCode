using AssetDatabases;
using Engine.Common;
using Engine.Common.Commons;
using Engine.Impl.Services.Factories;
using Engine.Source.Settings.External;
using ParadoxNotion;
using UnityEngine;

namespace Engine.Source.Commons
{
  [Factory(typeof (IBlueprintObject))]
  public class BlueprintObject : EngineObject, IBlueprintObject, IObject, IFactoryProduct
  {
    private GameObject gameObject;

    public GameObject GameObject
    {
      get
      {
        if ((Object) this.gameObject == (Object) null)
        {
          string path = AssetDatabaseService.Instance.GetPath(this.Id);
          ReflectionTools.ContextObject = (object) path;
          this.gameObject = AssetDatabaseService.Instance.Load<GameObject>(path);
        }
        return this.gameObject;
      }
    }

    public void ConstructComplete()
    {
      if (!ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.PreloadBlueprints)
        return;
      GameObject gameObject = this.GameObject;
    }
  }
}

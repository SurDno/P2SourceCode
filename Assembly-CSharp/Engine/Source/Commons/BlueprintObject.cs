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
        if (gameObject == null)
        {
          string path = AssetDatabaseService.Instance.GetPath(Id);
          ReflectionTools.ContextObject = path;
          gameObject = AssetDatabaseService.Instance.Load<GameObject>(path);
        }
        return gameObject;
      }
    }

    public void ConstructComplete()
    {
      if (!ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.PreloadBlueprints)
        return;
      GameObject gameObject = GameObject;
    }
  }
}

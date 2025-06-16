// Decompiled with JetBrains decompiler
// Type: Engine.Source.Commons.BlueprintObject
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using AssetDatabases;
using Engine.Common;
using Engine.Common.Commons;
using Engine.Impl.Services.Factories;
using Engine.Source.Settings.External;
using ParadoxNotion;
using UnityEngine;

#nullable disable
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

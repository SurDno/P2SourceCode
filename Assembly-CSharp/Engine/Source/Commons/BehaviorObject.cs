// Decompiled with JetBrains decompiler
// Type: Engine.Source.Commons.BehaviorObject
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using AssetDatabases;
using BehaviorDesigner.Runtime;
using Engine.Common;
using Engine.Common.Commons;
using Engine.Impl.Services.Factories;
using Engine.Source.Settings.External;
using UnityEngine;

#nullable disable
namespace Engine.Source.Commons
{
  [Factory(typeof (IBehaviorObject))]
  public class BehaviorObject : EngineObject, IBehaviorObject, IObject, IFactoryProduct
  {
    private ExternalBehaviorTree externalBehaviorTree;

    public ExternalBehaviorTree ExternalBehaviorTree
    {
      get
      {
        if ((Object) this.externalBehaviorTree == (Object) null)
          this.externalBehaviorTree = AssetDatabaseService.Instance.Load<ExternalBehaviorTree>(AssetDatabaseService.Instance.GetPath(this.Id));
        return this.externalBehaviorTree;
      }
    }

    public void ConstructComplete()
    {
      if (!ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.PreloadBehaviors)
        return;
      XmlDeserializationCache.GetOrCreateData(this.ExternalBehaviorTree.BehaviorSource.TaskData.XmlData);
    }
  }
}

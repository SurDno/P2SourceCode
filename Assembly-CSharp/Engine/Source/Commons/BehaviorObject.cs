using AssetDatabases;
using BehaviorDesigner.Runtime;
using Engine.Common;
using Engine.Common.Commons;
using Engine.Impl.Services.Factories;
using Engine.Source.Settings.External;
using UnityEngine;

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

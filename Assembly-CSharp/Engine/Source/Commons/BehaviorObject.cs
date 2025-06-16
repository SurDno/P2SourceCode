using AssetDatabases;
using BehaviorDesigner.Runtime;
using Engine.Common;
using Engine.Common.Commons;
using Engine.Impl.Services.Factories;
using Engine.Source.Settings.External;

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
        if (externalBehaviorTree == null)
          externalBehaviorTree = AssetDatabaseService.Instance.Load<ExternalBehaviorTree>(AssetDatabaseService.Instance.GetPath(Id));
        return externalBehaviorTree;
      }
    }

    public void ConstructComplete()
    {
      if (!ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.PreloadBehaviors)
        return;
      XmlDeserializationCache.GetOrCreateData(ExternalBehaviorTree.BehaviorSource.TaskData.XmlData);
    }
  }
}

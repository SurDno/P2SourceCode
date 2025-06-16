using BehaviorDesigner.Runtime;
using Engine.Common;
using Engine.Source.Settings.External;
using System;

namespace Engine.Source.Services
{
  [GameService(new Type[] {typeof (BehaviorService)})]
  public class BehaviorService : IInitialisable
  {
    private static bool initialized;

    public void Initialise()
    {
      if (BehaviorService.initialized)
        return;
      if (ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.PreloadBehaviors)
      {
        for (int index = 0; index < ScriptableObjectInstance<ResourceFromCodeData>.Instance.AdditionalAIToPreload.Length; ++index)
          XmlDeserializationCache.GetOrCreateData(ScriptableObjectInstance<ResourceFromCodeData>.Instance.AdditionalAIToPreload[index].BehaviorSource.TaskData.XmlData);
      }
      BehaviorService.initialized = true;
    }

    public void Terminate() => ObjectPool.Clear();
  }
}

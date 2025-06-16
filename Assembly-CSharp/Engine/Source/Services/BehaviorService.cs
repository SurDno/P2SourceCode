using BehaviorDesigner.Runtime;
using Engine.Common;
using Engine.Source.Settings.External;

namespace Engine.Source.Services
{
  [GameService(typeof (BehaviorService))]
  public class BehaviorService : IInitialisable
  {
    private static bool initialized;

    public void Initialise()
    {
      if (initialized)
        return;
      if (ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.PreloadBehaviors)
      {
        for (int index = 0; index < ScriptableObjectInstance<ResourceFromCodeData>.Instance.AdditionalAIToPreload.Length; ++index)
          XmlDeserializationCache.GetOrCreateData(ScriptableObjectInstance<ResourceFromCodeData>.Instance.AdditionalAIToPreload[index].BehaviorSource.TaskData.XmlData);
      }
      initialized = true;
    }

    public void Terminate() => ObjectPool.Clear();
  }
}

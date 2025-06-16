// Decompiled with JetBrains decompiler
// Type: Engine.Source.Services.BehaviorService
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using BehaviorDesigner.Runtime;
using Engine.Common;
using Engine.Source.Settings.External;
using System;

#nullable disable
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

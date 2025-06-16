using System;
using Engine.Common;
using Engine.Common.Services;
using Engine.Source.Commons;

namespace Engine.Source.Connections
{
  public static class TemplateUtility
  {
    public static T GetTemplate<T>(Guid id) where T : class, IObject
    {
      if (InstanceByRequest<EngineApplication>.Instance.IsInitialized)
        return ServiceLocator.GetService<ITemplateService>().GetTemplate<T>(id);
      throw new Exception();
    }
  }
}

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Cofe.Meta;
using Cofe.Utility;
using Engine.Common;
using Engine.Common.Services;
using Engine.Source.Services;
using Scripts.Utility;

namespace Engine.Source.Commons
{
  public static class GameServices
  {
    private static List<IInitialisable> services;

    public static void Initialize()
    {
      GameServiceAttribute.CreateServices();
      foreach (object service in ServiceLocator.GetServices())
      {
        UnityEngine.Debug.Log((object) ObjectInfoUtility.GetStream().Append("[Engine]").Append(" ").Append(TypeUtility.GetTypeName(service.GetType())));
        MetaService.GetContainer(service.GetType()).GetHandler(FromLocatorAttribute.Id).Compute(service, null);
      }
      services = GameServiceAttribute.GetServices().Select(o => o as IInitialisable).Where(o => o != null).ToList();
      services = ServiceLocatorUtility.SortServices<IInitialisable, DependAttribute>(services);
      Stopwatch stopwatch = new Stopwatch();
      for (int index = 0; index < services.Count; ++index)
      {
        IInitialisable service = services[index];
        UnityEngine.Debug.Log((object) ObjectInfoUtility.GetStream().Append("[Engine]").Append(" Initialise game service : ").Append(TypeUtility.GetTypeName(service.GetType())));
        stopwatch.Restart();
        service.Initialise();
        stopwatch.Stop();
        UnityEngine.Debug.Log((object) ObjectInfoUtility.GetStream().Append("[Engine]").Append(" Initialise game service complete : ").Append(TypeUtility.GetTypeName(service.GetType())).Append(" , elapsed : ").Append(stopwatch.Elapsed));
      }
    }

    public static void Terminate()
    {
      for (int index = services.Count - 1; index >= 0; --index)
        services[index].Terminate();
      services = null;
      GameServiceAttribute.DestroyServices();
    }
  }
}

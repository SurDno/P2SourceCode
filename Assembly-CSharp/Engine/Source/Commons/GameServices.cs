using Cofe.Meta;
using Cofe.Utility;
using Engine.Common;
using Engine.Common.Services;
using Engine.Source.Services;
using Scripts.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Engine.Source.Commons
{
  public static class GameServices
  {
    private static List<IInitialisable> services = (List<IInitialisable>) null;

    public static void Initialize()
    {
      GameServiceAttribute.CreateServices();
      foreach (object service in ServiceLocator.GetServices())
      {
        UnityEngine.Debug.Log((object) ObjectInfoUtility.GetStream().Append("[Engine]").Append(" ").Append(TypeUtility.GetTypeName(service.GetType())));
        MetaService.GetContainer(service.GetType()).GetHandler(FromLocatorAttribute.Id).Compute(service, (object) null);
      }
      GameServices.services = GameServiceAttribute.GetServices().Select<object, IInitialisable>((Func<object, IInitialisable>) (o => o as IInitialisable)).Where<IInitialisable>((Func<IInitialisable, bool>) (o => o != null)).ToList<IInitialisable>();
      GameServices.services = ServiceLocatorUtility.SortServices<IInitialisable, DependAttribute>(GameServices.services);
      Stopwatch stopwatch = new Stopwatch();
      for (int index = 0; index < GameServices.services.Count; ++index)
      {
        IInitialisable service = GameServices.services[index];
        UnityEngine.Debug.Log((object) ObjectInfoUtility.GetStream().Append("[Engine]").Append(" Initialise game service : ").Append(TypeUtility.GetTypeName(service.GetType())));
        stopwatch.Restart();
        service.Initialise();
        stopwatch.Stop();
        UnityEngine.Debug.Log((object) ObjectInfoUtility.GetStream().Append("[Engine]").Append(" Initialise game service complete : ").Append(TypeUtility.GetTypeName(service.GetType())).Append(" , elapsed : ").Append((object) stopwatch.Elapsed));
      }
    }

    public static void Terminate()
    {
      for (int index = GameServices.services.Count - 1; index >= 0; --index)
        GameServices.services[index].Terminate();
      GameServices.services = (List<IInitialisable>) null;
      GameServiceAttribute.DestroyServices();
    }
  }
}

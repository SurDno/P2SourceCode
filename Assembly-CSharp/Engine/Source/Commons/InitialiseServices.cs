// Decompiled with JetBrains decompiler
// Type: Engine.Source.Commons.InitialiseServices
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Meta;
using Cofe.Utility;
using Engine.Common;
using Engine.Common.Services;
using Engine.Source.Services;
using Scripts.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

#nullable disable
namespace Engine.Source.Commons
{
  public static class InitialiseServices
  {
    private static List<IInitialisable> services = (List<IInitialisable>) null;

    public static bool IsInitialised { get; private set; }

    public static IEnumerator Initialise()
    {
      if (InitialiseServices.IsInitialised)
        throw new Exception();
      Stopwatch sw = new Stopwatch();
      sw.Restart();
      InitialiseEngineProgressService progressService = ServiceLocator.GetService<InitialiseEngineProgressService>();
      foreach (object service in ServiceLocator.GetServices())
      {
        UnityEngine.Debug.Log((object) ObjectInfoUtility.GetStream().Append("[Engine]").Append(" ").Append(TypeUtility.GetTypeName(service.GetType())));
        MetaService.GetContainer(service.GetType()).GetHandler(FromLocatorAttribute.Id).Compute(service, (object) null);
      }
      InitialiseServices.services = RuntimeServiceAttribute.GetServices().Select<object, IInitialisable>((Func<object, IInitialisable>) (o => o as IInitialisable)).Where<IInitialisable>((Func<IInitialisable, bool>) (o => o != null)).ToList<IInitialisable>();
      InitialiseServices.services = ServiceLocatorUtility.SortServices<IInitialisable, DependAttribute>(InitialiseServices.services);
      int step = 500;
      int maxCount = InitialiseServices.services.Count * step;
      foreach (IInitialisable service in InitialiseServices.services)
      {
        if (service is IAsyncInitializable async)
          maxCount += async.AsyncCount;
        async = (IAsyncInitializable) null;
      }
      progressService.Begin(maxCount);
      sw.Stop();
      UnityEngine.Debug.Log((object) ObjectInfoUtility.GetStream().Append("[Engine]").Append(" Prepare initialise, count : ").Append(InitialiseServices.services.Count).Append(" , elapsed : ").Append((object) sw.Elapsed));
      for (int index = 0; index < InitialiseServices.services.Count; ++index)
      {
        IInitialisable service = InitialiseServices.services[index];
        UnityEngine.Debug.Log((object) ObjectInfoUtility.GetStream().Append("[Engine]").Append(" Initialise service : ").Append(TypeUtility.GetTypeName(service.GetType())));
        progressService.Progress += step;
        progressService.Update(nameof (InitialiseServices), TypeUtility.GetTypeName(service.GetType()));
        sw.Restart();
        if (service is IAsyncInitializable async)
        {
          yield return (object) async.AsyncInitialize();
        }
        else
        {
          try
          {
            service.Initialise();
          }
          catch (Exception ex)
          {
            UnityEngine.Debug.LogException(ex);
          }
        }
        sw.Stop();
        UnityEngine.Debug.Log((object) ObjectInfoUtility.GetStream().Append("[Engine]").Append(" Initialise service complete : ").Append(TypeUtility.GetTypeName(service.GetType())).Append(" , elapsed : ").Append((object) sw.Elapsed));
        yield return (object) null;
        service = (IInitialisable) null;
        async = (IAsyncInitializable) null;
      }
      progressService.End();
      InitialiseServices.IsInitialised = true;
    }

    public static void Terminate()
    {
      InitialiseServices.IsInitialised = InitialiseServices.IsInitialised ? false : throw new Exception();
      for (int index = InitialiseServices.services.Count - 1; index >= 0; --index)
      {
        IInitialisable service = InitialiseServices.services[index];
        try
        {
          service.Terminate();
        }
        catch (Exception ex)
        {
          UnityEngine.Debug.LogException(ex);
        }
      }
      InitialiseServices.services = (List<IInitialisable>) null;
    }
  }
}

using SRF.Components;
using SRF.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace SRF.Service
{
  [AddComponentMenu("SRF/Service/Service Manager")]
  public class SRServiceManager : SRAutoSingleton<SRServiceManager>
  {
    public const bool EnableLogging = false;
    public static int LoadingCount = 0;
    private readonly SRList<SRServiceManager.Service> _services = new SRList<SRServiceManager.Service>();
    private List<SRServiceManager.ServiceStub> _serviceStubs;
    private static bool _hasQuit;

    public static bool IsLoading => SRServiceManager.LoadingCount > 0;

    public static T GetService<T>() where T : class
    {
      if (!(SRServiceManager.GetServiceInternal(typeof (T)) is T serviceInternal) && !SRServiceManager._hasQuit)
        Debug.LogWarning((object) "Service {0} not found. (HasQuit: {1})".Fmt((object) typeof (T).Name, (object) SRServiceManager._hasQuit));
      return serviceInternal;
    }

    public static object GetService(System.Type t)
    {
      object serviceInternal = SRServiceManager.GetServiceInternal(t);
      if (serviceInternal == null && !SRServiceManager._hasQuit)
        Debug.LogWarning((object) "Service {0} not found. (HasQuit: {1})".Fmt((object) t.Name, (object) SRServiceManager._hasQuit));
      return serviceInternal;
    }

    private static object GetServiceInternal(System.Type t)
    {
      if (SRServiceManager._hasQuit || !Application.isPlaying)
        return (object) null;
      SRList<SRServiceManager.Service> services = SRAutoSingleton<SRServiceManager>.Instance._services;
      for (int index = 0; index < services.Count; ++index)
      {
        SRServiceManager.Service service = services[index];
        if (t.IsAssignableFrom(service.Type))
        {
          if (service.Object != null)
            return service.Object;
          SRServiceManager.UnRegisterService(t);
          break;
        }
      }
      return SRAutoSingleton<SRServiceManager>.Instance.AutoCreateService(t);
    }

    public static bool HasService<T>() where T : class => SRServiceManager.HasService(typeof (T));

    public static bool HasService(System.Type t)
    {
      if (SRServiceManager._hasQuit || !Application.isPlaying)
        return false;
      SRList<SRServiceManager.Service> services = SRAutoSingleton<SRServiceManager>.Instance._services;
      for (int index = 0; index < services.Count; ++index)
      {
        SRServiceManager.Service service = services[index];
        if (t.IsAssignableFrom(service.Type))
          return service.Object != null;
      }
      return false;
    }

    public static void RegisterService<T>(object service) where T : class
    {
      SRServiceManager.RegisterService(typeof (T), service);
    }

    private static void RegisterService(System.Type t, object service)
    {
      if (SRServiceManager._hasQuit)
        return;
      if (SRServiceManager.HasService(t))
      {
        if (SRServiceManager.GetServiceInternal(t) != service)
          throw new Exception("Service already registered for type " + t.Name);
      }
      else
      {
        SRServiceManager.UnRegisterService(t);
        if (!t.IsInstanceOfType(service))
          throw new ArgumentException("service {0} must be assignable from type {1}".Fmt((object) service.GetType(), (object) t));
        SRAutoSingleton<SRServiceManager>.Instance._services.Add(new SRServiceManager.Service()
        {
          Object = service,
          Type = t
        });
      }
    }

    public static void UnRegisterService<T>() where T : class
    {
      SRServiceManager.UnRegisterService(typeof (T));
    }

    private static void UnRegisterService(System.Type t)
    {
      if (SRServiceManager._hasQuit || !SRAutoSingleton<SRServiceManager>.HasInstance || !SRServiceManager.HasService(t))
        return;
      SRList<SRServiceManager.Service> services = SRAutoSingleton<SRServiceManager>.Instance._services;
      for (int index = services.Count - 1; index >= 0; --index)
      {
        if (services[index].Type == t)
          services.RemoveAt(index);
      }
    }

    protected override void Awake()
    {
      SRServiceManager._hasQuit = false;
      base.Awake();
      UnityEngine.Object.DontDestroyOnLoad((UnityEngine.Object) this.CachedGameObject);
      this.CachedGameObject.hideFlags = HideFlags.NotEditable;
    }

    protected void UpdateStubs()
    {
      if (this._serviceStubs != null)
        return;
      this._serviceStubs = new List<SRServiceManager.ServiceStub>();
      List<System.Type> typeList = new List<System.Type>();
      foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
      {
        if (!assembly.IsDynamic)
        {
          string fullName = assembly.FullName;
          if (!fullName.StartsWith("mscorlib") && !fullName.StartsWith("System") && !fullName.StartsWith("UnityEngine") && !fullName.StartsWith("Mono.") && !fullName.StartsWith("Boo.") && !fullName.StartsWith("UnityEditor") && !fullName.StartsWith("Unity.") && !fullName.StartsWith("UnityScript") && !fullName.StartsWith("nunit.") && !fullName.StartsWith("I18N") && !fullName.StartsWith("ICSharpCode") && !fullName.StartsWith("Newtonsoft.Json"))
          {
            try
            {
              bool flag = false;
              foreach (byte num in assembly.GetName().GetPublicKeyToken())
              {
                if (num > (byte) 0)
                {
                  flag = true;
                  break;
                }
              }
              if (!flag)
                typeList.AddRange((IEnumerable<System.Type>) assembly.GetExportedTypes());
            }
            catch (Exception ex)
            {
              Debug.LogError((object) "[SRServiceManager] Error loading assembly {0}".Fmt((object) assembly.FullName), (UnityEngine.Object) this);
              Debug.LogException(ex);
            }
          }
        }
      }
      foreach (System.Type type in typeList)
        this.ScanType(type);
      this._serviceStubs.Select<SRServiceManager.ServiceStub, string>((Func<SRServiceManager.ServiceStub, string>) (p => "\t{0}".Fmt((object) p))).ToArray<string>();
    }

    protected object AutoCreateService(System.Type t)
    {
      this.UpdateStubs();
      foreach (SRServiceManager.ServiceStub serviceStub in this._serviceStubs)
      {
        if (!(serviceStub.InterfaceType != t))
        {
          object service;
          if (serviceStub.Constructor != null)
          {
            service = serviceStub.Constructor();
          }
          else
          {
            System.Type implType = serviceStub.Type;
            if (implType == (System.Type) null)
              implType = serviceStub.Selector();
            service = SRServiceManager.DefaultServiceConstructor(t, implType);
          }
          if (!SRServiceManager.HasService(t))
            SRServiceManager.RegisterService(t, service);
          return service;
        }
      }
      return (object) null;
    }

    protected void OnApplicationQuit() => SRServiceManager._hasQuit = true;

    private static object DefaultServiceConstructor(System.Type serviceIntType, System.Type implType)
    {
      if (typeof (MonoBehaviour).IsAssignableFrom(implType))
        return (object) new GameObject("_S_" + serviceIntType.Name).AddComponent(implType);
      return typeof (ScriptableObject).IsAssignableFrom(implType) ? (object) ScriptableObject.CreateInstance(implType) : Activator.CreateInstance(implType);
    }

    private void ScanType(System.Type type)
    {
      ServiceAttribute attribute = SRReflection.GetAttribute<ServiceAttribute>((MemberInfo) type);
      if (attribute != null)
        this._serviceStubs.Add(new SRServiceManager.ServiceStub()
        {
          Type = type,
          InterfaceType = attribute.ServiceType
        });
      SRServiceManager.ScanTypeForConstructors(type, this._serviceStubs);
      SRServiceManager.ScanTypeForSelectors(type, this._serviceStubs);
    }

    private static void ScanTypeForSelectors(System.Type t, List<SRServiceManager.ServiceStub> stubs)
    {
      foreach (MethodInfo staticMethod in SRServiceManager.GetStaticMethods(t))
      {
        ServiceSelectorAttribute attrib = SRReflection.GetAttribute<ServiceSelectorAttribute>((MemberInfo) staticMethod);
        if (attrib != null)
        {
          if (staticMethod.ReturnType != typeof (System.Type))
            Debug.LogError((object) "ServiceSelector must have return type of Type ({0}.{1}())".Fmt((object) t.Name, (object) staticMethod.Name));
          else if (staticMethod.GetParameters().Length != 0)
          {
            Debug.LogError((object) "ServiceSelector must have no parameters ({0}.{1}())".Fmt((object) t.Name, (object) staticMethod.Name));
          }
          else
          {
            SRServiceManager.ServiceStub serviceStub = stubs.FirstOrDefault<SRServiceManager.ServiceStub>((Func<SRServiceManager.ServiceStub, bool>) (p => p.InterfaceType == attrib.ServiceType));
            if (serviceStub == null)
            {
              serviceStub = new SRServiceManager.ServiceStub()
              {
                InterfaceType = attrib.ServiceType
              };
              stubs.Add(serviceStub);
            }
            serviceStub.Selector = (Func<System.Type>) Delegate.CreateDelegate(typeof (Func<System.Type>), staticMethod);
          }
        }
      }
    }

    private static void ScanTypeForConstructors(System.Type t, List<SRServiceManager.ServiceStub> stubs)
    {
      foreach (MethodInfo staticMethod in SRServiceManager.GetStaticMethods(t))
      {
        MethodInfo method = staticMethod;
        ServiceConstructorAttribute attrib = SRReflection.GetAttribute<ServiceConstructorAttribute>((MemberInfo) method);
        if (attrib != null)
        {
          if (method.ReturnType != attrib.ServiceType)
            Debug.LogError((object) "ServiceConstructor must have return type of {2} ({0}.{1}())".Fmt((object) t.Name, (object) method.Name, (object) attrib.ServiceType));
          else if (method.GetParameters().Length != 0)
          {
            Debug.LogError((object) "ServiceConstructor must have no parameters ({0}.{1}())".Fmt((object) t.Name, (object) method.Name));
          }
          else
          {
            SRServiceManager.ServiceStub serviceStub = stubs.FirstOrDefault<SRServiceManager.ServiceStub>((Func<SRServiceManager.ServiceStub, bool>) (p => p.InterfaceType == attrib.ServiceType));
            if (serviceStub == null)
            {
              serviceStub = new SRServiceManager.ServiceStub()
              {
                InterfaceType = attrib.ServiceType
              };
              stubs.Add(serviceStub);
            }
            serviceStub.Constructor = (Func<object>) (() => method.Invoke((object) null, (object[]) null));
          }
        }
      }
    }

    private static MethodInfo[] GetStaticMethods(System.Type t)
    {
      return t.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
    }

    [Serializable]
    private class Service
    {
      public object Object;
      public System.Type Type;
    }

    [Serializable]
    private class ServiceStub
    {
      public Func<object> Constructor;
      public System.Type InterfaceType;
      public Func<System.Type> Selector;
      public System.Type Type;

      public override string ToString()
      {
        string str = this.InterfaceType.Name + " (";
        if (this.Type != (System.Type) null)
          str = str + "Type: " + (object) this.Type;
        else if (this.Selector != null)
          str = str + "Selector: " + (object) this.Selector;
        else if (this.Constructor != null)
          str = str + "Constructor: " + (object) this.Constructor;
        return str + ")";
      }
    }
  }
}

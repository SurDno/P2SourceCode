using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;
using Cofe.Serializations.Data;
using Cofe.Utility;
using Engine.Common;
using Engine.Impl.UI;
using Engine.Impl.UI.Menu;
using Engine.Source.Saves;
using Engine.Source.Services;
using Engine.Source.Services.Saves;
using Engine.Source.UI;
using Engine.Source.UI.Menu.Main;
using Inspectors;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Engine.Impl.Services
{
  [RuntimeService(typeof (UIService))]
  public class UIService : IInitialisable, ISavesController
  {
    [Inspected]
    private Dictionary<Type, UIWindow> layers = new Dictionary<Type, UIWindow>();
    [Inspected]
    private List<UIWindow> windows = new List<UIWindow>();
    [Inspected]
    private UIControl uiBehaviour;
    private UIRoot uiRoot;

    [Inspected]
    public bool IsTransition { get; private set; }

    public bool IsInitialize { get; private set; }

    [Inspected]
    public SmallLoading SmallLoading
    {
      get
      {
        if (!IsInitialize)
          throw new Exception(TypeUtility.GetTypeName(GetType()) + "." + MethodBase.GetCurrentMethod().Name);
        return uiRoot.SmallLoading;
      }
    }

    [Inspected]
    public GameObject VirtualCursor
    {
      get
      {
        if (!IsInitialize)
          throw new Exception(TypeUtility.GetTypeName(GetType()) + "." + MethodBase.GetCurrentMethod().Name);
        return uiRoot.VirtualCursor;
      }
    }

    public bool Visible
    {
      get
      {
        if (!IsInitialize)
          throw new Exception(TypeUtility.GetTypeName(GetType()) + "." + MethodBase.GetCurrentMethod().Name);
        return uiBehaviour.gameObject.activeSelf;
      }
      set
      {
        if (!IsInitialize)
          throw new Exception(TypeUtility.GetTypeName(GetType()) + "." + MethodBase.GetCurrentMethod().Name);
        uiBehaviour.gameObject.SetActive(value);
      }
    }

    [Inspected]
    public UIWindow Active
    {
      get
      {
        if (!IsInitialize)
          throw new Exception(TypeUtility.GetTypeName(GetType()) + "." + MethodBase.GetCurrentMethod().Name);
        return windows.LastOrDefault();
      }
    }

    public UIWindow Get(Type type)
    {
      UIWindow uiWindow;
      layers.TryGetValue(type, out uiWindow);
      return uiWindow;
    }

    public T Get<T>() where T : class, IWindow => Get(typeof (T)) as T;

    public UIWindow Swap(Type type)
    {
      Debug.Log(ObjectInfoUtility.GetStream().Append("Try swap window : ").Append(TypeUtility.GetTypeName(type)).Append(" , info : ").Append(TypeUtility.GetTypeName(GetType())).Append(":").Append(MethodBase.GetCurrentMethod().Name).Append("\n").GetStackTrace());
      if (!IsInitialize)
        throw new Exception(TypeUtility.GetTypeName(GetType()) + "." + MethodBase.GetCurrentMethod().Name);
      if (windows.Count == 0)
      {
        Debug.LogError("Windows not found , info : " + GetType().Name + ":" + MethodBase.GetCurrentMethod().Name);
        return null;
      }
      UIWindow openWindow;
      if (!layers.TryGetValue(type, out openWindow))
        throw new Exception(TypeUtility.GetTypeName(GetType()) + "." + MethodBase.GetCurrentMethod().Name);
      if (windows.Contains(openWindow))
      {
        Debug.LogError("Already pushed window : " + openWindow.GetType().Name);
        return null;
      }
      if (IsTransition)
      {
        Debug.LogError("Transition already, info : " + GetType().Name + ":" + MethodBase.GetCurrentMethod().Name);
        return null;
      }
      UIWindow active = Active;
      windows.RemoveAt(windows.Count - 1);
      windows.Add(openWindow);
      CoroutineService.Instance.Route(TranslateRoute(active, openWindow));
      return openWindow;
    }

    public T Swap<T>() where T : class, IWindow => Swap(typeof (T)) as T;

    public UIWindow Push(Type type)
    {
      Debug.Log(ObjectInfoUtility.GetStream().Append("Try push window : ").Append(TypeUtility.GetTypeName(type)).Append(" , info : ").Append(TypeUtility.GetTypeName(GetType())).Append(":").Append(MethodBase.GetCurrentMethod().Name).Append("\n").GetStackTrace());
      if (!IsInitialize)
        throw new Exception(TypeUtility.GetTypeName(GetType()) + "." + MethodBase.GetCurrentMethod().Name);
      UIWindow openWindow;
      if (!layers.TryGetValue(type, out openWindow))
        throw new Exception(TypeUtility.GetTypeName(GetType()) + "." + MethodBase.GetCurrentMethod().Name);
      if (windows.Contains(openWindow))
      {
        Debug.LogError("Already pushed window : " + openWindow.GetType().Name);
        return null;
      }
      if (IsTransition)
      {
        Debug.LogError("Transition already, info : " + GetType().Name + ":" + MethodBase.GetCurrentMethod().Name);
        return null;
      }
      UIWindow active = Active;
      windows.Add(openWindow);
      CoroutineService.Instance.Route(TranslateRoute(active, openWindow));
      return openWindow;
    }

    public T Push<T>() where T : class, IWindow => Push(typeof (T)) as T;

    public void Pop()
    {
      Debug.Log(ObjectInfoUtility.GetStream().Append("Try pop window , info : ").Append(TypeUtility.GetTypeName(GetType())).Append(":").Append(MethodBase.GetCurrentMethod().Name).Append("\n").GetStackTrace());
      if (!IsInitialize)
        throw new Exception(TypeUtility.GetTypeName(GetType()) + "." + MethodBase.GetCurrentMethod().Name);
      if (windows.Count == 0)
        Debug.LogError("Windows not found , info : " + GetType().Name + ":" + MethodBase.GetCurrentMethod().Name);
      else if (IsTransition)
      {
        Debug.LogError("Transition already, info : " + GetType().Name + ":" + MethodBase.GetCurrentMethod().Name);
      }
      else
      {
        UIWindow active1 = Active;
        windows.RemoveAt(windows.Count - 1);
        UIWindow active2 = Active;
        CoroutineService.Instance.Route(TranslateRoute(active1, active2));
      }
    }

    public void RegisterLayer<T>(T window) where T : class, IWindow
    {
      if (!IsInitialize)
        throw new Exception(TypeUtility.GetTypeName(GetType()) + "." + MethodBase.GetCurrentMethod().Name);
      layers.Add(typeof (T), (object) window as UIWindow);
    }

    public void Initialise()
    {
      IsInitialize = true;
      uiRoot = Object.FindObjectOfType<UIRoot>();
      uiRoot.Initialize();
      uiBehaviour = uiRoot.Root.GetComponent<UIControl>();
      uiBehaviour.Transform.pivot = Vector2.zero;
      uiBehaviour.Transform.anchorMin = Vector2.zero;
      uiBehaviour.Transform.anchorMax = Vector2.one;
      uiRoot.SmallLoading.gameObject.SetActive(false);
      int childCount = uiBehaviour.transform.childCount;
      for (int index = 0; index < childCount; ++index)
      {
        UIWindow component = uiBehaviour.transform.GetChild(index).GetComponent<UIWindow>();
        if (component != null)
          component.Initialize();
      }
      UnityEngine.Camera component1 = uiRoot.GetComponent<UnityEngine.Camera>();
      Canvas[] componentsInChildren = uiBehaviour.GetComponentsInChildren<Canvas>(true);
      for (int index = 0; index < componentsInChildren.Length; ++index)
      {
        componentsInChildren[index].renderMode = RenderMode.ScreenSpaceCamera;
        componentsInChildren[index].worldCamera = component1;
        componentsInChildren[index].planeDistance = 0.0f;
      }
    }

    public void Terminate()
    {
      foreach (KeyValuePair<Type, UIWindow> layer in layers)
      {
        if (layer.Value.IsEnabled)
          layer.Value.IsEnabled = false;
      }
      layers.Clear();
      IsInitialize = false;
    }

    private IEnumerator TranslateRoute(UIWindow closeWindow, UIWindow openWindow)
    {
      IsTransition = true;
      Func<UIWindow, UIWindow, IEnumerator> transition = UITransitionFactory.GetTransition(closeWindow, openWindow);
      IEnumerator iterator = transition(closeWindow, openWindow);
      while (iterator.MoveNext())
        yield return iterator.Current;
      IsTransition = false;
      Debug.Log(ObjectInfoUtility.GetStream().Append("End translate window , info : ").Append(TypeUtility.GetTypeName(GetType())).Append(":").Append(MethodBase.GetCurrentMethod().Name).Append("\n").GetStackTrace());
    }

    public IEnumerator Load(IErrorLoadingHandler errorHandler)
    {
      yield break;
    }

    public IEnumerator Load(XmlElement element, string context, IErrorLoadingHandler errorHandler)
    {
      yield break;
    }

    public void Unload()
    {
      MonoBehaviourInstance<UiEffectsController>.Instance.SetVisible(false, UiEffectType.Fade, 0.0f);
    }

    public void Save(IDataWriter element, string context)
    {
    }
  }
}

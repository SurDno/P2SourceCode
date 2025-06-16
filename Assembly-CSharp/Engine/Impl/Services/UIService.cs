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
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;
using UnityEngine;

namespace Engine.Impl.Services
{
  [RuntimeService(new System.Type[] {typeof (UIService)})]
  public class UIService : IInitialisable, ISavesController
  {
    [Inspected]
    private Dictionary<System.Type, UIWindow> layers = new Dictionary<System.Type, UIWindow>();
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
        if (!this.IsInitialize)
          throw new Exception(TypeUtility.GetTypeName(this.GetType()) + "." + MethodBase.GetCurrentMethod().Name);
        return this.uiRoot.SmallLoading;
      }
    }

    [Inspected]
    public GameObject VirtualCursor
    {
      get
      {
        if (!this.IsInitialize)
          throw new Exception(TypeUtility.GetTypeName(this.GetType()) + "." + MethodBase.GetCurrentMethod().Name);
        return this.uiRoot.VirtualCursor;
      }
    }

    public bool Visible
    {
      get
      {
        if (!this.IsInitialize)
          throw new Exception(TypeUtility.GetTypeName(this.GetType()) + "." + MethodBase.GetCurrentMethod().Name);
        return this.uiBehaviour.gameObject.activeSelf;
      }
      set
      {
        if (!this.IsInitialize)
          throw new Exception(TypeUtility.GetTypeName(this.GetType()) + "." + MethodBase.GetCurrentMethod().Name);
        this.uiBehaviour.gameObject.SetActive(value);
      }
    }

    [Inspected]
    public UIWindow Active
    {
      get
      {
        if (!this.IsInitialize)
          throw new Exception(TypeUtility.GetTypeName(this.GetType()) + "." + MethodBase.GetCurrentMethod().Name);
        return this.windows.LastOrDefault<UIWindow>();
      }
    }

    public UIWindow Get(System.Type type)
    {
      UIWindow uiWindow;
      this.layers.TryGetValue(type, out uiWindow);
      return uiWindow;
    }

    public T Get<T>() where T : class, IWindow => this.Get(typeof (T)) as T;

    public UIWindow Swap(System.Type type)
    {
      Debug.Log((object) ObjectInfoUtility.GetStream().Append("Try swap window : ").Append(TypeUtility.GetTypeName(type)).Append(" , info : ").Append(TypeUtility.GetTypeName(this.GetType())).Append(":").Append(MethodBase.GetCurrentMethod().Name).Append("\n").GetStackTrace());
      if (!this.IsInitialize)
        throw new Exception(TypeUtility.GetTypeName(this.GetType()) + "." + MethodBase.GetCurrentMethod().Name);
      if (this.windows.Count == 0)
      {
        Debug.LogError((object) ("Windows not found , info : " + this.GetType().Name + ":" + MethodBase.GetCurrentMethod().Name));
        return (UIWindow) null;
      }
      UIWindow openWindow;
      if (!this.layers.TryGetValue(type, out openWindow))
        throw new Exception(TypeUtility.GetTypeName(this.GetType()) + "." + MethodBase.GetCurrentMethod().Name);
      if (this.windows.Contains(openWindow))
      {
        Debug.LogError((object) ("Already pushed window : " + ((object) openWindow).GetType().Name));
        return (UIWindow) null;
      }
      if (this.IsTransition)
      {
        Debug.LogError((object) ("Transition already, info : " + this.GetType().Name + ":" + MethodBase.GetCurrentMethod().Name));
        return (UIWindow) null;
      }
      UIWindow active = this.Active;
      this.windows.RemoveAt(this.windows.Count - 1);
      this.windows.Add(openWindow);
      CoroutineService.Instance.Route(this.TranslateRoute(active, openWindow));
      return openWindow;
    }

    public T Swap<T>() where T : class, IWindow => this.Swap(typeof (T)) as T;

    public UIWindow Push(System.Type type)
    {
      Debug.Log((object) ObjectInfoUtility.GetStream().Append("Try push window : ").Append(TypeUtility.GetTypeName(type)).Append(" , info : ").Append(TypeUtility.GetTypeName(this.GetType())).Append(":").Append(MethodBase.GetCurrentMethod().Name).Append("\n").GetStackTrace());
      if (!this.IsInitialize)
        throw new Exception(TypeUtility.GetTypeName(this.GetType()) + "." + MethodBase.GetCurrentMethod().Name);
      UIWindow openWindow;
      if (!this.layers.TryGetValue(type, out openWindow))
        throw new Exception(TypeUtility.GetTypeName(this.GetType()) + "." + MethodBase.GetCurrentMethod().Name);
      if (this.windows.Contains(openWindow))
      {
        Debug.LogError((object) ("Already pushed window : " + ((object) openWindow).GetType().Name));
        return (UIWindow) null;
      }
      if (this.IsTransition)
      {
        Debug.LogError((object) ("Transition already, info : " + this.GetType().Name + ":" + MethodBase.GetCurrentMethod().Name));
        return (UIWindow) null;
      }
      UIWindow active = this.Active;
      this.windows.Add(openWindow);
      CoroutineService.Instance.Route(this.TranslateRoute(active, openWindow));
      return openWindow;
    }

    public T Push<T>() where T : class, IWindow => this.Push(typeof (T)) as T;

    public void Pop()
    {
      Debug.Log((object) ObjectInfoUtility.GetStream().Append("Try pop window , info : ").Append(TypeUtility.GetTypeName(this.GetType())).Append(":").Append(MethodBase.GetCurrentMethod().Name).Append("\n").GetStackTrace());
      if (!this.IsInitialize)
        throw new Exception(TypeUtility.GetTypeName(this.GetType()) + "." + MethodBase.GetCurrentMethod().Name);
      if (this.windows.Count == 0)
        Debug.LogError((object) ("Windows not found , info : " + this.GetType().Name + ":" + MethodBase.GetCurrentMethod().Name));
      else if (this.IsTransition)
      {
        Debug.LogError((object) ("Transition already, info : " + this.GetType().Name + ":" + MethodBase.GetCurrentMethod().Name));
      }
      else
      {
        UIWindow active1 = this.Active;
        this.windows.RemoveAt(this.windows.Count - 1);
        UIWindow active2 = this.Active;
        CoroutineService.Instance.Route(this.TranslateRoute(active1, active2));
      }
    }

    public void RegisterLayer<T>(T window) where T : class, IWindow
    {
      if (!this.IsInitialize)
        throw new Exception(TypeUtility.GetTypeName(this.GetType()) + "." + MethodBase.GetCurrentMethod().Name);
      this.layers.Add(typeof (T), (object) window as UIWindow);
    }

    public void Initialise()
    {
      this.IsInitialize = true;
      this.uiRoot = UnityEngine.Object.FindObjectOfType<UIRoot>();
      this.uiRoot.Initialize();
      this.uiBehaviour = this.uiRoot.Root.GetComponent<UIControl>();
      this.uiBehaviour.Transform.pivot = Vector2.zero;
      this.uiBehaviour.Transform.anchorMin = Vector2.zero;
      this.uiBehaviour.Transform.anchorMax = Vector2.one;
      this.uiRoot.SmallLoading.gameObject.SetActive(false);
      int childCount = this.uiBehaviour.transform.childCount;
      for (int index = 0; index < childCount; ++index)
      {
        UIWindow component = this.uiBehaviour.transform.GetChild(index).GetComponent<UIWindow>();
        if ((UnityEngine.Object) component != (UnityEngine.Object) null)
          component.Initialize();
      }
      UnityEngine.Camera component1 = this.uiRoot.GetComponent<UnityEngine.Camera>();
      Canvas[] componentsInChildren = this.uiBehaviour.GetComponentsInChildren<Canvas>(true);
      for (int index = 0; index < componentsInChildren.Length; ++index)
      {
        componentsInChildren[index].renderMode = RenderMode.ScreenSpaceCamera;
        componentsInChildren[index].worldCamera = component1;
        componentsInChildren[index].planeDistance = 0.0f;
      }
    }

    public void Terminate()
    {
      foreach (KeyValuePair<System.Type, UIWindow> layer in this.layers)
      {
        if (layer.Value.IsEnabled)
          layer.Value.IsEnabled = false;
      }
      this.layers.Clear();
      this.IsInitialize = false;
    }

    private IEnumerator TranslateRoute(UIWindow closeWindow, UIWindow openWindow)
    {
      this.IsTransition = true;
      Func<UIWindow, UIWindow, IEnumerator> transition = UITransitionFactory.GetTransition(closeWindow, openWindow);
      IEnumerator iterator = transition(closeWindow, openWindow);
      while (iterator.MoveNext())
        yield return iterator.Current;
      this.IsTransition = false;
      Debug.Log((object) ObjectInfoUtility.GetStream().Append("End translate window , info : ").Append(TypeUtility.GetTypeName(this.GetType())).Append(":").Append(MethodBase.GetCurrentMethod().Name).Append("\n").GetStackTrace());
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

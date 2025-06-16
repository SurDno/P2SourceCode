using System;
using System.Diagnostics;
using UnityEngine;

namespace SRF.Components
{
  public abstract class SRSingleton<T> : SRMonoBehaviour where T : SRSingleton<T>
  {
    private static T _instance;

    public static T Instance
    {
      [DebuggerStepThrough] get
      {
        return !((UnityEngine.Object) SRSingleton<T>._instance == (UnityEngine.Object) null) ? SRSingleton<T>._instance : throw new InvalidOperationException("No instance of {0} present in scene".Fmt((object) typeof (T).Name));
      }
    }

    public static bool HasInstance
    {
      [DebuggerStepThrough] get => (UnityEngine.Object) SRSingleton<T>._instance != (UnityEngine.Object) null;
    }

    private void Register()
    {
      if ((UnityEngine.Object) SRSingleton<T>._instance != (UnityEngine.Object) null)
      {
        UnityEngine.Debug.LogWarning((object) "More than one singleton object of type {0} exists.".Fmt((object) typeof (T).Name));
        if (this.GetComponents<Component>().Length == 2)
          UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
        else
          UnityEngine.Object.Destroy((UnityEngine.Object) this);
      }
      else
        SRSingleton<T>._instance = (T) this;
    }

    protected virtual void Awake() => this.Register();

    protected virtual void OnEnable()
    {
      if (!((UnityEngine.Object) SRSingleton<T>._instance == (UnityEngine.Object) null))
        return;
      this.Register();
    }

    private void OnApplicationQuit() => SRSingleton<T>._instance = default (T);
  }
}

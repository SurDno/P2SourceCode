using System;
using System.Diagnostics;

namespace SRF.Components
{
  public abstract class SRSingleton<T> : SRMonoBehaviour where T : SRSingleton<T>
  {
    private static T _instance;

    public static T Instance
    {
      [DebuggerStepThrough] get
      {
        return !((UnityEngine.Object) _instance == (UnityEngine.Object) null) ? _instance : throw new InvalidOperationException("No instance of {0} present in scene".Fmt(typeof (T).Name));
      }
    }

    public static bool HasInstance
    {
      [DebuggerStepThrough] get => (UnityEngine.Object) _instance != (UnityEngine.Object) null;
    }

    private void Register()
    {
      if ((UnityEngine.Object) _instance != (UnityEngine.Object) null)
      {
        UnityEngine.Debug.LogWarning((object) "More than one singleton object of type {0} exists.".Fmt(typeof (T).Name));
        if (this.GetComponents<Component>().Length == 2)
          UnityEngine.Object.Destroy((UnityEngine.Object) gameObject);
        else
          UnityEngine.Object.Destroy((UnityEngine.Object) this);
      }
      else
        _instance = (T) this;
    }

    protected virtual void Awake() => Register();

    protected virtual void OnEnable()
    {
      if (!((UnityEngine.Object) _instance == (UnityEngine.Object) null))
        return;
      Register();
    }

    private void OnApplicationQuit() => _instance = default (T);
  }
}

using System;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace SRF.Components
{
  public abstract class SRSingleton<T> : SRMonoBehaviour where T : SRSingleton<T>
  {
    private static T _instance;

    public static T Instance
    {
      [DebuggerStepThrough] get => !(_instance == null) ? _instance : throw new InvalidOperationException("No instance of {0} present in scene".Fmt(typeof (T).Name));
    }

    public static bool HasInstance
    {
      [DebuggerStepThrough] get => _instance != null;
    }

    private void Register()
    {
      if (_instance != null)
      {
        Debug.LogWarning("More than one singleton object of type {0} exists.".Fmt(typeof (T).Name));
        if (GetComponents<Component>().Length == 2)
          Destroy(gameObject);
        else
          Destroy(this);
      }
      else
        _instance = (T) this;
    }

    protected virtual void Awake() => Register();

    protected virtual void OnEnable()
    {
      if (!(_instance == null))
        return;
      Register();
    }

    private void OnApplicationQuit() => _instance = default (T);
  }
}

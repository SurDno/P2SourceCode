using System.Diagnostics;
using UnityEngine;

namespace SRF.Components
{
  public abstract class SRAutoSingleton<T> : SRMonoBehaviour where T : SRAutoSingleton<T>
  {
    private static T _instance;

    public static T Instance
    {
      [DebuggerStepThrough] get
      {
        if ((Object) SRAutoSingleton<T>._instance == (Object) null && Application.isPlaying)
          new GameObject("_" + typeof (T).Name).AddComponent<T>();
        return SRAutoSingleton<T>._instance;
      }
    }

    public static bool HasInstance => (Object) SRAutoSingleton<T>._instance != (Object) null;

    protected virtual void Awake()
    {
      if ((Object) SRAutoSingleton<T>._instance != (Object) null)
        UnityEngine.Debug.LogWarning((object) "More than one singleton object of type {0} exists.".Fmt((object) typeof (T).Name));
      else
        SRAutoSingleton<T>._instance = (T) this;
    }

    private void OnApplicationQuit() => SRAutoSingleton<T>._instance = default (T);
  }
}

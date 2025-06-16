using System.Diagnostics;

namespace SRF.Components
{
  public abstract class SRAutoSingleton<T> : SRMonoBehaviour where T : SRAutoSingleton<T>
  {
    private static T _instance;

    public static T Instance
    {
      [DebuggerStepThrough] get
      {
        if ((Object) _instance == (Object) null && Application.isPlaying)
          new GameObject("_" + typeof (T).Name).AddComponent<T>();
        return _instance;
      }
    }

    public static bool HasInstance => (Object) _instance != (Object) null;

    protected virtual void Awake()
    {
      if ((Object) _instance != (Object) null)
        UnityEngine.Debug.LogWarning((object) "More than one singleton object of type {0} exists.".Fmt(typeof (T).Name));
      else
        _instance = (T) this;
    }

    private void OnApplicationQuit() => _instance = default (T);
  }
}

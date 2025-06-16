using UnityEngine;

namespace RootMotion
{
  public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
  {
    private static T sInstance = default (T);

    public static T instance => Singleton<T>.sInstance;

    protected virtual void Awake()
    {
      if ((Object) Singleton<T>.sInstance != (Object) null)
        Debug.LogError((object) (this.name + "error: already initialized"), (Object) this);
      Singleton<T>.sInstance = (T) this;
    }
  }
}

namespace RootMotion
{
  public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
  {
    private static T sInstance;

    public static T instance => sInstance;

    protected virtual void Awake()
    {
      if ((Object) sInstance != (Object) null)
        Debug.LogError((object) (this.name + "error: already initialized"), (Object) this);
      sInstance = (T) this;
    }
  }
}

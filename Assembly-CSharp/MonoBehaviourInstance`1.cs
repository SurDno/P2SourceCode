public abstract class MonoBehaviourInstance<T> : MonoBehaviour where T : MonoBehaviourInstance<T>
{
  public static T Instance { get; private set; }

  protected virtual void Awake()
  {
    if ((Object) Instance != (Object) null)
      Debug.LogError((object) "Instance already created", (Object) this);
    Instance = (T) this;
  }
}

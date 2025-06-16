using UnityEngine;

public abstract class MonoBehaviourInstance<T> : MonoBehaviour where T : MonoBehaviourInstance<T>
{
  public static T Instance { get; private set; }

  protected virtual void Awake()
  {
    if ((Object) MonoBehaviourInstance<T>.Instance != (Object) null)
      Debug.LogError((object) "Instance already created", (Object) this);
    MonoBehaviourInstance<T>.Instance = (T) this;
  }
}

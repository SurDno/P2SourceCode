using UnityEngine;

public abstract class MonoBehaviourInstance<T> : MonoBehaviour where T : MonoBehaviourInstance<T>
{
  public static T Instance { get; private set; }

  protected virtual void Awake()
  {
    if (Instance != null)
      Debug.LogError("Instance already created", this);
    Instance = (T) this;
  }
}

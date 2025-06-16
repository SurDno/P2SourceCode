using UnityEngine;

namespace Scripts.Behaviours.LoadControllers
{
  public abstract class BaseLoadByDistance : MonoBehaviour
  {
    public abstract float LoadDistance { get; }

    public abstract float UnloadDistance { get; }
  }
}

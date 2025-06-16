using UnityEngine;

namespace SoundPropagation
{
  public class SPFieldPoint : MonoBehaviour
  {
    [SerializeField]
    private SPFieldSource sourcePrefab;

    public Vector3 Position { get; private set; }

    private void OnEnable()
    {
      if (sourcePrefab == null)
        return;
      Position = transform.position;
      SPFieldSource.AddPoint(sourcePrefab, this);
    }

    private void OnDisable()
    {
      if (sourcePrefab == null)
        return;
      SPFieldSource.RemovePoint(sourcePrefab, this);
    }
  }
}

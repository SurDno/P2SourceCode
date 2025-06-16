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
      if ((Object) this.sourcePrefab == (Object) null)
        return;
      this.Position = this.transform.position;
      SPFieldSource.AddPoint(this.sourcePrefab, this);
    }

    private void OnDisable()
    {
      if ((Object) this.sourcePrefab == (Object) null)
        return;
      SPFieldSource.RemovePoint(this.sourcePrefab, this);
    }
  }
}

namespace SoundPropagation
{
  public class SPFieldPoint : MonoBehaviour
  {
    [SerializeField]
    private SPFieldSource sourcePrefab;

    public Vector3 Position { get; private set; }

    private void OnEnable()
    {
      if ((Object) sourcePrefab == (Object) null)
        return;
      Position = this.transform.position;
      SPFieldSource.AddPoint(sourcePrefab, this);
    }

    private void OnDisable()
    {
      if ((Object) sourcePrefab == (Object) null)
        return;
      SPFieldSource.RemovePoint(sourcePrefab, this);
    }
  }
}

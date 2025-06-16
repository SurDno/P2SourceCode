public class StatsViewAnchor : MonoBehaviour
{
  private static StatsView viewInstance = null;
  [SerializeField]
  private StatsView prefab;
  [SerializeField]
  private bool fullVersion;

  private void OnEnable()
  {
    if ((Object) viewInstance == (Object) null)
      viewInstance = Object.Instantiate<StatsView>(prefab, this.transform, false);
    else
      viewInstance.transform.SetParent(this.transform, false);
    viewInstance.SetFullVersion(fullVersion);
  }
}

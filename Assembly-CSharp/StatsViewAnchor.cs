using UnityEngine;

public class StatsViewAnchor : MonoBehaviour
{
  private static StatsView viewInstance = (StatsView) null;
  [SerializeField]
  private StatsView prefab;
  [SerializeField]
  private bool fullVersion;

  private void OnEnable()
  {
    if ((Object) StatsViewAnchor.viewInstance == (Object) null)
      StatsViewAnchor.viewInstance = Object.Instantiate<StatsView>(this.prefab, this.transform, false);
    else
      StatsViewAnchor.viewInstance.transform.SetParent(this.transform, false);
    StatsViewAnchor.viewInstance.SetFullVersion(this.fullVersion);
  }
}

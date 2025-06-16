public class SubtitlesViewAnchor : MonoBehaviour
{
  private static SubtitlesView viewInstance = null;
  [SerializeField]
  private SubtitlesView prefab;

  private void OnEnable()
  {
    if ((Object) viewInstance == (Object) null)
      viewInstance = Object.Instantiate<SubtitlesView>(prefab, this.transform, false);
    else
      viewInstance.transform.SetParent(this.transform, false);
  }
}

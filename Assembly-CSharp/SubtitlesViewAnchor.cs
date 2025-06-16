using UnityEngine;

public class SubtitlesViewAnchor : MonoBehaviour
{
  private static SubtitlesView viewInstance = (SubtitlesView) null;
  [SerializeField]
  private SubtitlesView prefab;

  private void OnEnable()
  {
    if ((Object) SubtitlesViewAnchor.viewInstance == (Object) null)
      SubtitlesViewAnchor.viewInstance = Object.Instantiate<SubtitlesView>(this.prefab, this.transform, false);
    else
      SubtitlesViewAnchor.viewInstance.transform.SetParent(this.transform, false);
  }
}

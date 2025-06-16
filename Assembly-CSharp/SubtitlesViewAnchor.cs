using UnityEngine;

public class SubtitlesViewAnchor : MonoBehaviour
{
  private static SubtitlesView viewInstance;
  [SerializeField]
  private SubtitlesView prefab;

  private void OnEnable()
  {
    if (viewInstance == null)
      viewInstance = Instantiate(prefab, transform, false);
    else
      viewInstance.transform.SetParent(transform, false);
  }
}

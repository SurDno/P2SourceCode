using UnityEngine;

[CreateAssetMenu(menuName = "Data/LOD Group Settings")]
public class LODGroupSettings : ScriptableObject
{
  [SerializeField]
  private float[] thresholds;
  [SerializeField]
  private bool crossFade = true;
  [SerializeField]
  private bool animateCrossFading = true;
  [SerializeField]
  private float[] fadeTransitionWidths;

  public void Apply(LODGroup lodGroup)
  {
    lodGroup.fadeMode = crossFade ? LODFadeMode.CrossFade : LODFadeMode.None;
    lodGroup.animateCrossFading = animateCrossFading;
    LOD[] loDs = lodGroup.GetLODs();
    for (int index = 0; index < loDs.Length; ++index)
    {
      if (index < thresholds.Length)
        loDs[index].screenRelativeTransitionHeight = thresholds[index];
      else if (loDs[index].screenRelativeTransitionHeight >= (double) thresholds[thresholds.Length - 1])
        loDs[index].screenRelativeTransitionHeight = loDs[index - 1].screenRelativeTransitionHeight * 0.5f;
      if (index < fadeTransitionWidths.Length)
        loDs[index].fadeTransitionWidth = fadeTransitionWidths[index];
    }
    lodGroup.SetLODs(loDs);
  }
}

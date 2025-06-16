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
    lodGroup.fadeMode = this.crossFade ? LODFadeMode.CrossFade : LODFadeMode.None;
    lodGroup.animateCrossFading = this.animateCrossFading;
    LOD[] loDs = lodGroup.GetLODs();
    for (int index = 0; index < loDs.Length; ++index)
    {
      if (index < this.thresholds.Length)
        loDs[index].screenRelativeTransitionHeight = this.thresholds[index];
      else if ((double) loDs[index].screenRelativeTransitionHeight >= (double) this.thresholds[this.thresholds.Length - 1])
        loDs[index].screenRelativeTransitionHeight = loDs[index - 1].screenRelativeTransitionHeight * 0.5f;
      if (index < this.fadeTransitionWidths.Length)
        loDs[index].fadeTransitionWidth = this.fadeTransitionWidths[index];
    }
    lodGroup.SetLODs(loDs);
  }
}

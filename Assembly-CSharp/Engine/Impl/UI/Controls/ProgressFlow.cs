using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Engine.Impl.UI.Controls
{
  [DisallowMultipleComponent]
  [ExecuteInEditMode]
  public class ProgressFlow : ProgressView
  {
    [SerializeField]
    [FormerlySerializedAs("Invert")]
    private bool invert;
    [SerializeField]
    [FormerlySerializedAs("Overlap")]
    private float overlap = 1f;

    public override void SkipAnimation()
    {
    }

    protected override void ApplyProgress()
    {
      Image component = GetComponent<Image>();
      if (component == null)
        return;
      float num1 = Mathf.Clamp01((float) ((1.0 - Progress) * (1.0 + overlap)));
      float num2 = Mathf.Clamp01(Progress * (1f + overlap));
      Color color = component.color;
      if (invert)
      {
        color.r = num2;
        color.g = num1;
      }
      else
      {
        color.r = num1;
        color.g = num2;
      }
      component.color = color;
    }
  }
}

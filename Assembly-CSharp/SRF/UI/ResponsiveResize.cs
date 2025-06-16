using System;
using UnityEngine;
using UnityEngine.UI;

namespace SRF.UI
{
  [ExecuteInEditMode]
  [RequireComponent(typeof (RectTransform))]
  [AddComponentMenu("SRF/UI/Responsive (Enable)")]
  public class ResponsiveResize : ResponsiveBase
  {
    public ResponsiveResize.Element[] Elements = new ResponsiveResize.Element[0];

    protected override void Refresh()
    {
      Rect rect = this.RectTransform.rect;
      for (int index1 = 0; index1 < this.Elements.Length; ++index1)
      {
        ResponsiveResize.Element element = this.Elements[index1];
        if (!((UnityEngine.Object) element.Target == (UnityEngine.Object) null))
        {
          float num = float.MinValue;
          float size = -1f;
          for (int index2 = 0; index2 < element.SizeDefinitions.Length; ++index2)
          {
            ResponsiveResize.Element.SizeDefinition sizeDefinition = element.SizeDefinitions[index2];
            if ((double) sizeDefinition.ThresholdWidth <= (double) rect.width && (double) sizeDefinition.ThresholdWidth > (double) num)
            {
              num = sizeDefinition.ThresholdWidth;
              size = sizeDefinition.ElementWidth;
            }
          }
          if ((double) size > 0.0)
          {
            element.Target.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size);
            LayoutElement component = element.Target.GetComponent<LayoutElement>();
            if ((UnityEngine.Object) component != (UnityEngine.Object) null)
              component.preferredWidth = size;
          }
        }
      }
    }

    [Serializable]
    public struct Element
    {
      public ResponsiveResize.Element.SizeDefinition[] SizeDefinitions;
      public RectTransform Target;

      [Serializable]
      public struct SizeDefinition
      {
        [Tooltip("Width to apply when over the threshold width")]
        public float ElementWidth;
        [Tooltip("Threshold over which this width will take effect")]
        public float ThresholdWidth;
      }
    }
  }
}

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
    public Element[] Elements = new Element[0];

    protected override void Refresh()
    {
      Rect rect = RectTransform.rect;
      for (int index1 = 0; index1 < Elements.Length; ++index1)
      {
        Element element = Elements[index1];
        if (!(element.Target == null))
        {
          float num = float.MinValue;
          float size = -1f;
          for (int index2 = 0; index2 < element.SizeDefinitions.Length; ++index2)
          {
            Element.SizeDefinition sizeDefinition = element.SizeDefinitions[index2];
            if (sizeDefinition.ThresholdWidth <= (double) rect.width && sizeDefinition.ThresholdWidth > (double) num)
            {
              num = sizeDefinition.ThresholdWidth;
              size = sizeDefinition.ElementWidth;
            }
          }
          if (size > 0.0)
          {
            element.Target.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size);
            LayoutElement component = element.Target.GetComponent<LayoutElement>();
            if (component != null)
              component.preferredWidth = size;
          }
        }
      }
    }

    [Serializable]
    public struct Element
    {
      public SizeDefinition[] SizeDefinitions;
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

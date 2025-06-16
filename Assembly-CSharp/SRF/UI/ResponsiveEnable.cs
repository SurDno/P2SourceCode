using System;
using UnityEngine;

namespace SRF.UI
{
  [ExecuteInEditMode]
  [RequireComponent(typeof (RectTransform))]
  [AddComponentMenu("SRF/UI/Responsive (Enable)")]
  public class ResponsiveEnable : ResponsiveBase
  {
    public ResponsiveEnable.Entry[] Entries = new ResponsiveEnable.Entry[0];

    protected override void Refresh()
    {
      Rect rect = this.RectTransform.rect;
      for (int index1 = 0; index1 < this.Entries.Length; ++index1)
      {
        ResponsiveEnable.Entry entry = this.Entries[index1];
        bool flag = true;
        switch (entry.Mode)
        {
          case ResponsiveEnable.Modes.EnableAbove:
            if ((double) entry.ThresholdHeight > 0.0)
              flag = (double) rect.height >= (double) entry.ThresholdHeight & flag;
            if ((double) entry.ThresholdWidth > 0.0)
            {
              flag = (double) rect.width >= (double) entry.ThresholdWidth & flag;
              break;
            }
            break;
          case ResponsiveEnable.Modes.EnableBelow:
            if ((double) entry.ThresholdHeight > 0.0)
              flag = (double) rect.height <= (double) entry.ThresholdHeight & flag;
            if ((double) entry.ThresholdWidth > 0.0)
            {
              flag = (double) rect.width <= (double) entry.ThresholdWidth & flag;
              break;
            }
            break;
          default:
            throw new IndexOutOfRangeException();
        }
        if (entry.GameObjects != null)
        {
          for (int index2 = 0; index2 < entry.GameObjects.Length; ++index2)
          {
            GameObject gameObject = entry.GameObjects[index2];
            if ((UnityEngine.Object) gameObject != (UnityEngine.Object) null)
              gameObject.SetActive(flag);
          }
        }
        if (entry.Components != null)
        {
          for (int index3 = 0; index3 < entry.Components.Length; ++index3)
          {
            Behaviour component = entry.Components[index3];
            if ((UnityEngine.Object) component != (UnityEngine.Object) null)
              component.enabled = flag;
          }
        }
      }
    }

    public enum Modes
    {
      EnableAbove,
      EnableBelow,
    }

    [Serializable]
    public struct Entry
    {
      public Behaviour[] Components;
      public GameObject[] GameObjects;
      public ResponsiveEnable.Modes Mode;
      public float ThresholdHeight;
      public float ThresholdWidth;
    }
  }
}

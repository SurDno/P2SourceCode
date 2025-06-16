using System;

namespace SRF.UI
{
  [ExecuteInEditMode]
  [RequireComponent(typeof (RectTransform))]
  [AddComponentMenu("SRF/UI/Responsive (Enable)")]
  public class ResponsiveEnable : ResponsiveBase
  {
    public Entry[] Entries = new Entry[0];

    protected override void Refresh()
    {
      Rect rect = RectTransform.rect;
      for (int index1 = 0; index1 < Entries.Length; ++index1)
      {
        Entry entry = Entries[index1];
        bool flag = true;
        switch (entry.Mode)
        {
          case Modes.EnableAbove:
            if (entry.ThresholdHeight > 0.0)
              flag = (double) rect.height >= entry.ThresholdHeight & flag;
            if (entry.ThresholdWidth > 0.0)
            {
              flag = (double) rect.width >= entry.ThresholdWidth & flag;
            }
            break;
          case Modes.EnableBelow:
            if (entry.ThresholdHeight > 0.0)
              flag = (double) rect.height <= entry.ThresholdHeight & flag;
            if (entry.ThresholdWidth > 0.0)
            {
              flag = (double) rect.width <= entry.ThresholdWidth & flag;
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
      public Modes Mode;
      public float ThresholdHeight;
      public float ThresholdWidth;
    }
  }
}

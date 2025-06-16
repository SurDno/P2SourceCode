using System;
using System.Collections.Generic;

[Serializable]
public class POIAnimationSetupAngle : POIAnimationSetupBase
{
  [SerializeField]
  public POIAnimationSetupElementSlow SetupLeft = new POIAnimationSetupElementSlow();
  [SerializeField]
  public POIAnimationSetupElementSlow SetupMiddle = new POIAnimationSetupElementSlow();
  [SerializeField]
  public POIAnimationSetupElementSlow SetupRight = new POIAnimationSetupElementSlow();
  [SerializeField]
  private List<POIAnimationSetupElementBase> elements;

  public override List<POIAnimationSetupElementBase> Elements
  {
    get
    {
      if (!CheckElements())
      {
        elements = new List<POIAnimationSetupElementBase>();
        elements.Add(SetupLeft);
        elements.Add(SetupMiddle);
        elements.Add(SetupRight);
      }
      return elements;
    }
  }

  private bool CheckElements()
  {
    if (elements == null || elements.Count == 0)
      return false;
    foreach (POIAnimationSetupElementBase element in elements)
    {
      if (!(element is POIAnimationSetupElementSlow))
        return false;
    }
    return true;
  }
}

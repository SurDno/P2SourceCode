using System;
using System.Collections.Generic;
using UnityEngine;

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
      if (!this.CheckElements())
      {
        this.elements = new List<POIAnimationSetupElementBase>();
        this.elements.Add((POIAnimationSetupElementBase) this.SetupLeft);
        this.elements.Add((POIAnimationSetupElementBase) this.SetupMiddle);
        this.elements.Add((POIAnimationSetupElementBase) this.SetupRight);
      }
      return this.elements;
    }
  }

  private bool CheckElements()
  {
    if (this.elements == null || this.elements.Count == 0)
      return false;
    foreach (POIAnimationSetupElementBase element in this.elements)
    {
      if (!(element is POIAnimationSetupElementSlow))
        return false;
    }
    return true;
  }
}

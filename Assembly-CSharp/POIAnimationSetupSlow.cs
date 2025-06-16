using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class POIAnimationSetupSlow : POIAnimationSetupBase
{
  [SerializeField]
  public List<POIAnimationSetupElementSlow> ElementsNew = new List<POIAnimationSetupElementSlow>();

  public override List<POIAnimationSetupElementBase> Elements
  {
    get
    {
      return new List<POIAnimationSetupElementBase>(ElementsNew);
    }
  }
}

using System;
using System.Collections.Generic;

[Serializable]
public class POIAnimationSetupQuick : POIAnimationSetupBase
{
  [SerializeField]
  public List<POIAnimationSetupElementQuick> ElementsNew = new List<POIAnimationSetupElementQuick>();

  public override List<POIAnimationSetupElementBase> Elements
  {
    get
    {
      return new List<POIAnimationSetupElementBase>(ElementsNew);
    }
  }
}

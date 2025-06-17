using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class POIAnimationSetupSlow : POIAnimationSetupBase
{
  [SerializeField]
  public List<POIAnimationSetupElementSlow> ElementsNew = [];

  public override List<POIAnimationSetupElementBase> Elements => [..ElementsNew];
}

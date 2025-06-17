using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class POIAnimationSetupQuick : POIAnimationSetupBase
{
  [SerializeField]
  public List<POIAnimationSetupElementQuick> ElementsNew = [];

  public override List<POIAnimationSetupElementBase> Elements => [..ElementsNew];
}

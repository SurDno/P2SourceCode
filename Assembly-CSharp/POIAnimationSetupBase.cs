using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class POIAnimationSetupBase
{
  [SerializeField]
  public AnimationClip EnterAnimationClip;
  [SerializeField]
  public AnimationClip ExitAnimationClip;
  [SerializeField]
  [FormerlySerializedAs("Elements")]
  public List<POIAnimationSetupElementBase> ElementsOld = [];

  public virtual List<POIAnimationSetupElementBase> Elements { get; }
}

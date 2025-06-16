using System;
using System.Collections.Generic;

[Serializable]
public class POIAnimationSetupElementSlow : POIAnimationSetupElementBase
{
  [SerializeField]
  public AnimationClip EnterAnimationClip;
  [SerializeField]
  public AnimationClip ExitAnimationClip;
  [SerializeField]
  public List<AnimationClip> MiddleAnimationClips;
}

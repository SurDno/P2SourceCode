using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class POIAnimationSetupElementSlow : POIAnimationSetupElementBase {
	[SerializeField] public AnimationClip EnterAnimationClip;
	[SerializeField] public AnimationClip ExitAnimationClip;
	[SerializeField] public List<AnimationClip> MiddleAnimationClips;
}
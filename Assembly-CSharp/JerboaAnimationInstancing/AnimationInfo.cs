using System.Collections.Generic;
using UnityEngine;

namespace JerboaAnimationInstancing;

public class AnimationInfo {
	public string animationName;
	public int animationNameHash;
	public int totalFrame;
	public int fps;
	public int animationIndex;
	public int textureIndex;
	public bool rootMotion;
	public WrapMode wrapMode;
	public Vector3[] velocity;
	public Vector3[] angularVelocity;
	public List<AnimationEvent> eventList;
}
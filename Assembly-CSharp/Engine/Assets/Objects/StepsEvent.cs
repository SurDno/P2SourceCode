using System;
using UnityEngine;

namespace Engine.Assets.Objects;

[Serializable]
public class StepsEvent {
	public string Name;
	public float ActionIntensity;
	public bool HaveReaction;
	public float ReactionIntensity;
	public AudioClip[] Clips;
}
using System;
using UnityEngine;

namespace Engine.Assets.Objects;

[Serializable]
public class StepsReaction {
	public float Intensity;
	public float MaxThesholdIntensity;
	public float MinThesholdIntensity;
	public PhysicMaterial Material;
	public AudioClip[] Clips;
}
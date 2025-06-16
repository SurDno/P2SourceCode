using System;
using Engine.Source.Connections;
using UnityEngine;

[Serializable]
public class CraftRecipe {
	[SerializeField] public IEntitySerializable Component1;
	[SerializeField] public IEntitySerializable Component2;
	[SerializeField] public IEntitySerializable Component3;
	[SerializeField] public IEntitySerializable Result;
	[SerializeField] public float SuccessChance;
	[SerializeField] public float Time;
}
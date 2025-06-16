using Engine.Common;
using Inspectors;
using UnityEngine;

namespace Engine.Source.Components.Crowds;

public class HerbRootsPointInfo {
	[Inspected] public Vector3 CenterPoint;
	[Inspected] public IEntity Entity;
}
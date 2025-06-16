using Engine.Common;
using Inspectors;
using UnityEngine;

namespace Engine.Source.Components.Crowds;

public class OutdoorPointInfo : PointInfo {
	[Inspected] public int Radius;
	[Inspected] public Vector3 CenterPoint;
	[Inspected] public IEntity Template;
	[Inspected] public bool NotReady;
}
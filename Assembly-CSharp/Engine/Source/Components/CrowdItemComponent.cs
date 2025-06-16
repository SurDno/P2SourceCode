using Engine.Common;
using Engine.Common.Components;
using Engine.Common.Components.Movable;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;
using Engine.Source.Components.Crowds;
using Inspectors;

namespace Engine.Source.Components;

[Factory(typeof(ICrowdItemComponent))]
[GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
public class CrowdItemComponent : EngineComponent, ICrowdItemComponent, IComponent {
	[Inspected] public IEntity Crowd { get; private set; }

	public AreaEnum Area => Point == null ? AreaEnum.Unknown : Point.Area;

	[Inspected] public PointInfo Point { get; private set; }

	public void AttachToCrowd(IEntity crowd, PointInfo point) {
		Crowd = crowd;
		Point = point;
	}

	public void DetachFromCrowd() {
		Crowd = null;
		Point = null;
	}
}
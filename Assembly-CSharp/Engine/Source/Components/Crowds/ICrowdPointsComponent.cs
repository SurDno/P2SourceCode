using System.Collections.Generic;
using Engine.Common;
using Engine.Common.Components.Movable;

namespace Engine.Source.Components.Crowds;

public interface ICrowdPointsComponent : IComponent {
	IEnumerable<CrowdPointInfo> Points { get; }

	void GetEnabledPoints(AreaEnum area, int count, List<CrowdPointInfo> result);
}
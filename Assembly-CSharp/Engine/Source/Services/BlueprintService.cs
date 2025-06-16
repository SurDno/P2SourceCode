using System;
using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Services;

namespace Engine.Source.Services;

[GameService(typeof(IBlueprintService))]
public class BlueprintService : IBlueprintService {
	public void Start(IBlueprintObject bp, IEntity owner, Action complete) {
		BlueprintServiceUtility.Start(bp, owner, complete, null);
	}
}
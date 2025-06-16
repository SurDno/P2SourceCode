using System.Collections.Generic;
using Engine.Source.Components;
using Inspectors;

namespace Engine.Source.Commons.Abilities;

public class OutsideAbilityTargets {
	[Inspected] public List<EffectsComponent> Targets = new();
}
using Engine.Source.Components;
using Inspectors;
using System.Collections.Generic;

namespace Engine.Source.Commons.Abilities
{
  public class OutsideAbilityTargets
  {
    [Inspected]
    public List<EffectsComponent> Targets = new List<EffectsComponent>();
  }
}

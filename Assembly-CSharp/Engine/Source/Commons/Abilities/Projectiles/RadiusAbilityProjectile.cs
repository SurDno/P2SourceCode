using Engine.Common;
using Engine.Common.Components;
using Engine.Common.Generator;
using Engine.Common.Services;
using Engine.Impl.Services.Factories;
using Engine.Source.Components;
using Engine.Source.Services;
using Engine.Source.Services.Detectablies;
using Inspectors;
using System;

namespace Engine.Source.Commons.Abilities.Projectiles
{
  [Factory]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class RadiusAbilityProjectile : IAbilityProjectile
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected(Mutable = true, Mode = ExecuteMode.EditAndRuntime)]
    protected float radius;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected(Mutable = true, Mode = ExecuteMode.EditAndRuntime)]
    protected bool ignoreSelf;

    public float Radius => this.radius;

    public void ComputeTargets(IEntity self, IEntity item, OutsideAbilityTargets targets)
    {
      IEntity selfEntity = self;
      if (this.ignoreSelf)
      {
        ParentComponent component = self.GetComponent<ParentComponent>();
        if (component != null)
          selfEntity = component.GetRootParent();
      }
      targets.Targets.Clear();
      DetectorUtility.GetCandidats(ServiceLocator.GetService<DetectorService>().Detectablies, self.GetComponent<DetectorComponent>(), self.GetComponent<ILocationItemComponent>(), this.radius, (Action<DetectableCandidatInfo>) (target =>
      {
        EffectsComponent component = target.Detectable.Owner.GetComponent<EffectsComponent>();
        if (component == null || this.ignoreSelf && component.Owner == selfEntity)
          return;
        targets.Targets.Add(component);
      }));
    }
  }
}

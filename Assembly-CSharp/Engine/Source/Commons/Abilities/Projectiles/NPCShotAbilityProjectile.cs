using System.Collections.Generic;
using Engine.Common;
using Engine.Common.Components.Parameters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Components;
using Inspectors;

namespace Engine.Source.Commons.Abilities.Projectiles
{
  [Factory]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class NPCShotAbilityProjectile : IAbilityProjectile
  {
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy()]
    [Inspected(Mutable = true, Mode = ExecuteMode.EditAndRuntime)]
    protected BlockTypeEnum blocked = BlockTypeEnum.None;

    public void ComputeTargets(IEntity self, IEntity item, OutsideAbilityTargets targets)
    {
      targets.Targets = new List<EffectsComponent>();
      GameObject gameObject = ((IEntityView) self).GameObject;
      if ((Object) gameObject == (Object) null)
        return;
      EnemyBase component1 = gameObject.GetComponent<EnemyBase>();
      if ((Object) component1 == (Object) null)
        return;
      EnemyBase enemy = component1.Enemy;
      if ((Object) enemy == (Object) null)
        return;
      EngineGameObject component2 = enemy.gameObject.GetComponent<EngineGameObject>();
      if ((Object) component2 == (Object) null)
        return;
      IEntity owner = component2.Owner;
      if (owner == null || !CheckBlocked(owner))
        return;
      EffectsComponent component3 = owner.GetComponent<EffectsComponent>();
      if (component3 == null)
        return;
      targets.Targets.Add(component3);
    }

    private bool CheckBlocked(IEntity target)
    {
      if (blocked == BlockTypeEnum.None)
        return true;
      ParametersComponent component = target.GetComponent<ParametersComponent>();
      if (component != null)
      {
        IParameter<BlockTypeEnum> byName = component.GetByName<BlockTypeEnum>(ParameterNameEnum.BlockType);
        if (byName != null)
          return blocked == byName.Value;
      }
      return true;
    }
  }
}

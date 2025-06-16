using Engine.Common;
using Engine.Common.Components;
using Engine.Common.Components.Parameters;
using Engine.Common.Generator;
using Engine.Common.Services;
using Engine.Impl.Services.Factories;
using Engine.Source.Components;
using Engine.Source.Services;
using Engine.Source.Services.Detectablies;
using Inspectors;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Engine.Source.Commons.Abilities.Projectiles
{
  [Factory]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class PlayerCloseCombatAbilityProjectile : IAbilityProjectile
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
    protected float angle;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected(Mutable = true, Mode = ExecuteMode.EditAndRuntime)]
    protected float maximumXOffset = 0.4f;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected(Mutable = true, Mode = ExecuteMode.EditAndRuntime)]
    protected int aims = 1;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected(Mutable = true, Mode = ExecuteMode.EditAndRuntime)]
    protected BlockTypeEnum blocked = BlockTypeEnum.None;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected(Mutable = true, Mode = ExecuteMode.EditAndRuntime)]
    protected HitOrientationTypeEnum orientation = HitOrientationTypeEnum.None;
    private IEntity self;

    public float Radius => this.radius;

    public float Angle => this.angle;

    public float MaximumXOffset => this.maximumXOffset;

    public int Aims => this.aims;

    public void ComputeTargets(IEntity self, IEntity item, OutsideAbilityTargets targets)
    {
      this.self = self;
      List<EffectsComponent> candidatsEffects = new List<EffectsComponent>();
      targets.Targets = new List<EffectsComponent>();
      GameObject gameObject = ((IEntityView) self).GameObject;
      DetectorUtility.GetCandidats(ServiceLocator.GetService<DetectorService>().Detectablies, self.GetComponent<DetectorComponent>(), self.GetComponent<ILocationItemComponent>(), this.radius, (Action<DetectableCandidatInfo>) (target =>
      {
        Vector3 vector3 = gameObject.transform.InverseTransformDirection(target.GameObject.transform.position - gameObject.transform.position);
        if ((double) vector3.z > (double) this.radius || (double) vector3.z < 0.0 || (double) Mathf.Abs(vector3.x) > (double) this.maximumXOffset)
          return;
        IEntity owner = target.Detectable.Owner;
        if (owner == null)
          return;
        EffectsComponent component = owner.GetComponent<EffectsComponent>();
        if (component == null)
          return;
        candidatsEffects.Add(component);
      }));
      if (candidatsEffects.Count <= this.Aims)
      {
        targets.Targets.AddRange((IEnumerable<EffectsComponent>) candidatsEffects.FindAll((Predicate<EffectsComponent>) (x => this.CheckBlocked(x.Owner) && this.CheckHitOrientation(x.Owner) && this.CheckCombatIgnored(x.Owner) && this.CheckDead(x.Owner))));
      }
      else
      {
        candidatsEffects.Sort(new Comparison<EffectsComponent>(this.SortByRange));
        targets.Targets.AddRange((IEnumerable<EffectsComponent>) candidatsEffects.GetRange(0, this.Aims).FindAll((Predicate<EffectsComponent>) (x => this.CheckBlocked(x.Owner) && this.CheckHitOrientation(x.Owner) && this.CheckCombatIgnored(x.Owner) && this.CheckDead(x.Owner))));
      }
    }

    private bool CheckCombatIgnored(IEntity target)
    {
      ParametersComponent component = target.GetComponent<ParametersComponent>();
      if (component != null)
      {
        IParameter<bool> byName = component.GetByName<bool>(ParameterNameEnum.IsCombatIgnored);
        if (byName != null)
          return !byName.Value;
      }
      return true;
    }

    private bool CheckDead(IEntity target)
    {
      ParametersComponent component = target.GetComponent<ParametersComponent>();
      if (component != null)
      {
        IParameter<bool> byName = component.GetByName<bool>(ParameterNameEnum.Dead);
        if (byName != null)
          return !byName.Value;
      }
      return true;
    }

    private bool CheckBlocked(IEntity target)
    {
      if (this.blocked == BlockTypeEnum.None)
        return true;
      ParametersComponent component = target.GetComponent<ParametersComponent>();
      if (component != null)
      {
        IParameter<BlockTypeEnum> byName = component.GetByName<BlockTypeEnum>(ParameterNameEnum.BlockType);
        if (byName != null)
          return this.blocked == byName.Value;
      }
      return true;
    }

    private bool CheckHitOrientation(IEntity target)
    {
      if (this.orientation == HitOrientationTypeEnum.None)
        return true;
      GameObject gameObject1 = ((IEntityView) this.self)?.GameObject;
      GameObject gameObject2 = ((IEntityView) target)?.GameObject;
      if ((UnityEngine.Object) gameObject1 == (UnityEngine.Object) null || (UnityEngine.Object) gameObject2 == (UnityEngine.Object) null)
        return false;
      bool flag = (double) Vector3.Dot((gameObject1.transform.position - gameObject2.transform.position).normalized, -gameObject2.transform.forward) > 0.0;
      if (this.orientation == HitOrientationTypeEnum.Back)
        return flag;
      return this.orientation != HitOrientationTypeEnum.Front || !flag;
    }

    private int SortByRange(EffectsComponent p1, EffectsComponent p2)
    {
      Vector3? position1 = ((IEntityView) p1?.Owner)?.Position;
      Vector3? position2 = ((IEntityView) p2?.Owner)?.Position;
      if (!position1.HasValue || !position2.HasValue)
        return 0;
      Vector3 position3 = ((IEntityView) this.self).Position;
      Vector3 vector3_1 = position3;
      Vector3? nullable1 = position1;
      Vector3 vector3_2 = (nullable1.HasValue ? new Vector3?(vector3_1 - nullable1.GetValueOrDefault()) : new Vector3?()).Value;
      double magnitude1 = (double) vector3_2.magnitude;
      vector3_2 = position3;
      Vector3? nullable2 = position2;
      double magnitude2 = (double) (nullable2.HasValue ? new Vector3?(vector3_2 - nullable2.GetValueOrDefault()) : new Vector3?()).Value.magnitude;
      return magnitude1 < magnitude2 ? -1 : 1;
    }
  }
}

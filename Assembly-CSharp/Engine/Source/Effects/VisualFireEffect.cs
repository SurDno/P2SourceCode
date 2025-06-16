// Decompiled with JetBrains decompiler
// Type: Engine.Source.Effects.VisualFireEffect
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Utility;
using Engine.Common;
using Engine.Common.Components.Parameters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;
using Engine.Source.Commons.Abilities;
using Engine.Source.Commons.Effects;
using Engine.Source.Components;
using Inspectors;
using System;
using System.Linq;
using UnityEngine;

#nullable disable
namespace Engine.Source.Effects
{
  [Factory]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class VisualFireEffect : IEffect
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected ParameterEffectQueueEnum queue = ParameterEffectQueueEnum.None;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected bool single = false;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected bool realTime = false;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected float duration = 0.0f;
    private float fadeOutTime = 2f;
    private float startTime;
    private RendererBurn effect;
    private IParameter<bool> burningParameter;

    public string Name => this.GetType().Name;

    [Inspected]
    public AbilityItem AbilityItem { get; set; }

    public IEntity Target { get; set; }

    public ParameterEffectQueueEnum Queue => this.queue;

    private bool TargetIgnored(IEntity target)
    {
      ParametersComponent component = target.GetComponent<ParametersComponent>();
      IParameter<bool> byName1 = component?.GetByName<bool>(ParameterNameEnum.IsCombatIgnored);
      if (byName1 != null && byName1.Value)
        return true;
      IParameter<float> byName2 = component?.GetByName<float>(ParameterNameEnum.FireArmor);
      return byName2 != null && (double) byName2.Value >= 1.0;
    }

    public bool Prepare(float currentRealTime, float currentGameTime)
    {
      if (this.TargetIgnored(this.Target))
        return true;
      float num = this.realTime ? currentRealTime : currentGameTime;
      if (this.single && !this.Name.IsNullOrEmpty() && this.Target.GetComponent<EffectsComponent>().Effects.FirstOrDefault<IEffect>((Func<IEffect, bool>) (o => o.Name == this.Name)) is VisualFireEffect visualFireEffect)
      {
        visualFireEffect.startTime = num;
        return false;
      }
      this.startTime = num;
      Renderer biggestRenderer = RendererUtility.GetBiggestRenderer(((IEntityView) this.Target).GameObject);
      GameObject rendererBurn = ScriptableObjectInstance<ResourceFromCodeData>.Instance.RendererBurn;
      if ((UnityEngine.Object) rendererBurn != (UnityEngine.Object) null)
      {
        this.effect = UnityFactory.Instantiate<RendererBurn>(rendererBurn, "[Effects]");
        this.effect.BurningRenderer = biggestRenderer;
        this.effect.Strength = 1f;
      }
      this.burningParameter = this.Target.GetComponent<ParametersComponent>().GetByName<bool>(ParameterNameEnum.IsBurning);
      if (this.burningParameter != null)
        this.burningParameter.Value = true;
      return true;
    }

    public bool Compute(float currentRealTime, float currentGameTime)
    {
      if (this.TargetIgnored(this.Target))
        return false;
      float num1 = this.realTime ? currentRealTime : currentGameTime;
      float num2 = this.duration - (num1 - this.startTime);
      this.effect.Strength = (double) num2 >= (double) this.fadeOutTime ? 1f : num2 / this.fadeOutTime;
      if ((double) num1 - (double) this.startTime > (double) this.duration)
      {
        if (this.burningParameter != null)
          this.burningParameter.Value = false;
        return false;
      }
      return this.burningParameter == null || this.burningParameter.Value;
    }

    public void Cleanup()
    {
      if (!((UnityEngine.Object) this.effect != (UnityEngine.Object) null))
        return;
      UnityFactory.Destroy((MonoBehaviour) this.effect);
      this.effect = (RendererBurn) null;
    }
  }
}

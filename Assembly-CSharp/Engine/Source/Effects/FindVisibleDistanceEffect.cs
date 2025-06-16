using Engine.Common;
using Engine.Common.Components.Parameters;
using Engine.Common.Generator;
using Engine.Common.Services;
using Engine.Impl.Services.Factories;
using Engine.Services.Engine;
using Engine.Source.Commons.Abilities;
using Engine.Source.Commons.Effects;
using Engine.Source.Components;
using Engine.Source.Services;
using Inspectors;
using UnityEngine;

namespace Engine.Source.Effects
{
  [Factory]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class FindVisibleDistanceEffect : IEffect
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
    [Inspected(Mutable = true, Mode = ExecuteMode.EditAndRuntime)]
    protected bool enable = true;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected DurationTypeEnum durationType = DurationTypeEnum.None;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected bool realTime;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected float duration;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected float interval;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected ParameterNameEnum VisibileDistanceParameterName;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected ParameterNameEnum FlashlightOnParameterName;
    private float lastTime;
    private float startTime;

    [Inspected]
    public AbilityItem AbilityItem { get; set; }

    public IEntity Target { get; set; }

    public string Name => this.GetType().Name;

    public ParameterEffectQueueEnum Queue => this.queue;

    public bool Prepare(float currentRealTime, float currentGameTime) => true;

    public bool Compute(float currentRealTime, float currentGameTime)
    {
      float num = this.realTime ? currentRealTime : currentGameTime;
      if (this.durationType == DurationTypeEnum.ByDuration && (double) num - (double) this.startTime > (double) this.duration || this.durationType == DurationTypeEnum.ByAbility && (this.AbilityItem == null || !this.AbilityItem.Active))
        return false;
      if ((double) this.interval == 0.0)
      {
        this.lastTime = num;
        this.ComputeEffect();
      }
      else
      {
        while ((double) num - (double) this.lastTime >= (double) this.interval)
        {
          float lastTime = this.lastTime;
          this.lastTime += this.interval;
          if ((double) lastTime == (double) this.lastTime)
          {
            Debug.LogError((object) ("Error compute effects, effect name : " + this.Name + " , target : " + this.Target.GetInfo()));
            break;
          }
          this.ComputeEffect();
        }
      }
      return this.durationType != DurationTypeEnum.None && this.durationType != DurationTypeEnum.Once;
    }

    private float ComputeSkyLight()
    {
      float skyLight = 1f;
      TOD_Sky tod = ServiceLocator.GetService<EnvironmentService>().Tod;
      if ((Object) tod != (Object) null)
        skyLight = tod.LerpValue;
      return skyLight;
    }

    private void ComputeEffect()
    {
      ParametersComponent component = this.Target?.GetComponent<ParametersComponent>();
      IParameter<float> byName1 = component?.GetByName<float>(this.VisibileDistanceParameterName);
      if (byName1 == null)
        return;
      if (this.Target.GetComponent<LocationItemComponent>().IsIndoor)
      {
        byName1.Value = byName1.MaxValue;
      }
      else
      {
        float skyLight = this.ComputeSkyLight();
        float num1 = 0.0f;
        IParameter<bool> byName2 = component?.GetByName<bool>(this.FlashlightOnParameterName);
        if (byName2 != null)
          num1 = byName2.Value ? 1f : 0.0f;
        int num2 = 0;
        LightService service = ServiceLocator.GetService<LightService>();
        if (service != null)
          num2 = service.PlayerIsLighted ? 1 : 0;
        float t = skyLight;
        if ((double) t < (double) num1)
          t = num1;
        if ((double) t < (double) num2)
          t = (float) num2;
        float num3 = Mathf.Lerp(byName1.MinValue, byName1.MaxValue, t);
        byName1.Value = num3;
      }
    }

    public void Cleanup()
    {
    }
  }
}

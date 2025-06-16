using Engine.Common;
using Engine.Common.Generator;
using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;
using Engine.Source.Commons.Effects;
using Engine.Source.Commons.Parameters;
using Inspectors;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Engine.Source.Components
{
  [Required(typeof (ParametersComponent))]
  [Factory(typeof (EffectsComponent))]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class EffectsComponent : EngineComponent, IUpdatable
  {
    [Inspected]
    private List<IEffect> effects = new List<IEffect>();
    private static List<IEffect> tmp = new List<IEffect>();
    [FromThis]
    private ParametersComponent parameters;
    [FromThis]
    private LocationItemComponent locationItemComponent;
    [FromLocator]
    private TimeService timeService;
    private IUpdater updater;
    private TimeSpan prevTime;
    private TimeSpan currentTime;
    private TimeSpan prevRealTime;
    private TimeSpan currentRealTime;
    private bool added;
    private bool forcedUpdate;

    public bool Disabled { get; set; }

    public IEnumerable<IEffect> Effects => (IEnumerable<IEffect>) this.effects;

    public void AddEffect(IEffect effect)
    {
      int index1 = -1;
      for (int index2 = 0; index2 < this.effects.Count; ++index2)
      {
        if (this.effects[index2].Queue >= effect.Queue)
        {
          index1 = index2;
          break;
        }
      }
      float totalSeconds1 = (float) this.currentRealTime.TotalSeconds;
      float totalSeconds2 = (float) this.currentTime.TotalSeconds;
      if (effect.Prepare(totalSeconds1, totalSeconds2))
      {
        if (index1 == -1)
          this.effects.Add(effect);
        else
          this.effects.Insert(index1, effect);
        this.ComputeUpdate();
      }
      this.ComputeListener();
    }

    public void RemoveEffect(IEffect effect)
    {
      this.effects.Remove(effect);
      effect.Cleanup();
      this.ComputeListener();
    }

    public override void OnAdded()
    {
      base.OnAdded();
      this.prevTime = this.timeService.AbsoluteGameTime;
      this.prevRealTime = this.timeService.RealTime;
      this.ComputeListener();
    }

    public override void OnRemoved()
    {
      this.RemoveUpdate();
      this.parameters = (ParametersComponent) null;
      base.OnRemoved();
    }

    private void ComputeListener()
    {
      if (this.effects.Count != 0)
      {
        if (this.added)
          return;
        this.AddUpdate();
      }
      else if (this.added)
        this.RemoveUpdate();
    }

    private void AddUpdate()
    {
      if (this.added)
        return;
      this.prevTime = this.timeService.AbsoluteGameTime;
      this.prevRealTime = this.timeService.RealTime;
      if (this.updater == null)
        this.updater = InstanceByRequest<UpdateService>.Instance.EffectUpdater;
      this.updater.AddUpdatable((IUpdatable) this);
      this.added = true;
    }

    private void RemoveUpdate()
    {
      if (!this.added)
        return;
      this.updater.RemoveUpdatable((IUpdatable) this);
      this.added = false;
    }

    public void ComputeUpdate()
    {
      if (this.Disabled && !this.forcedUpdate)
      {
        this.prevTime = this.timeService.AbsoluteGameTime;
        this.prevRealTime = this.timeService.RealTime;
      }
      else
      {
        if (InstanceByRequest<EngineApplication>.Instance.IsPaused && !this.forcedUpdate)
          return;
        this.forcedUpdate = false;
        TimeSpan timeSpan1 = this.timeService.AbsoluteGameTime - this.prevTime;
        this.prevTime = this.timeService.AbsoluteGameTime;
        TimeSpan timeSpan2 = this.timeService.RealTime - this.prevRealTime;
        this.prevRealTime = this.timeService.RealTime;
        if (!this.Owner.IsEnabledInHierarchy || this.locationItemComponent != null && this.locationItemComponent.IsHibernation)
          return;
        this.currentTime += timeSpan1;
        this.currentRealTime += timeSpan2;
        float totalSeconds1 = (float) this.currentRealTime.TotalSeconds;
        float totalSeconds2 = (float) this.currentTime.TotalSeconds;
        if (EngineApplication.Sleep && !((Entity) this.Owner).IsPlayer)
          return;
        this.ComputeEffects(totalSeconds1, totalSeconds2);
      }
    }

    public void ForceComputeUpdate() => this.forcedUpdate = true;

    private void ComputeEffects(float currentRealTime, float currentGameTime)
    {
      foreach (IComputeParameter parameter in this.parameters.Parameters)
        parameter.ResetResetable();
      EffectsComponent.tmp.Clear();
      foreach (IEffect effect in this.effects)
        EffectsComponent.tmp.Add(effect);
      int index = 0;
      while (index < EffectsComponent.tmp.Count)
      {
        IEffect effect = EffectsComponent.tmp[index];
        bool flag;
        try
        {
          flag = effect.Compute(currentRealTime, currentGameTime);
        }
        catch (Exception ex)
        {
          Debug.LogException(ex);
          flag = true;
        }
        if (flag)
          EffectsComponent.tmp.RemoveAt(index);
        else
          ++index;
      }
      foreach (IEffect effect in EffectsComponent.tmp)
      {
        effect.Cleanup();
        this.effects.Remove(effect);
      }
      foreach (IComputeParameter parameter in this.parameters.Parameters)
        parameter.CorrectValue();
    }
  }
}

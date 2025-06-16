using System;
using System.Collections.Generic;
using Engine.Common;
using Engine.Common.Generator;
using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;
using Engine.Source.Commons.Effects;
using Engine.Source.Commons.Parameters;
using Inspectors;
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

    public IEnumerable<IEffect> Effects => effects;

    public void AddEffect(IEffect effect)
    {
      int index1 = -1;
      for (int index2 = 0; index2 < effects.Count; ++index2)
      {
        if (effects[index2].Queue >= effect.Queue)
        {
          index1 = index2;
          break;
        }
      }
      float totalSeconds1 = (float) currentRealTime.TotalSeconds;
      float totalSeconds2 = (float) currentTime.TotalSeconds;
      if (effect.Prepare(totalSeconds1, totalSeconds2))
      {
        if (index1 == -1)
          effects.Add(effect);
        else
          effects.Insert(index1, effect);
        ComputeUpdate();
      }
      ComputeListener();
    }

    public void RemoveEffect(IEffect effect)
    {
      effects.Remove(effect);
      effect.Cleanup();
      ComputeListener();
    }

    public override void OnAdded()
    {
      base.OnAdded();
      prevTime = timeService.AbsoluteGameTime;
      prevRealTime = timeService.RealTime;
      ComputeListener();
    }

    public override void OnRemoved()
    {
      RemoveUpdate();
      parameters = null;
      base.OnRemoved();
    }

    private void ComputeListener()
    {
      if (effects.Count != 0)
      {
        if (added)
          return;
        AddUpdate();
      }
      else if (added)
        RemoveUpdate();
    }

    private void AddUpdate()
    {
      if (added)
        return;
      prevTime = timeService.AbsoluteGameTime;
      prevRealTime = timeService.RealTime;
      if (updater == null)
        updater = InstanceByRequest<UpdateService>.Instance.EffectUpdater;
      updater.AddUpdatable(this);
      added = true;
    }

    private void RemoveUpdate()
    {
      if (!added)
        return;
      updater.RemoveUpdatable(this);
      added = false;
    }

    public void ComputeUpdate()
    {
      if (Disabled && !forcedUpdate)
      {
        prevTime = timeService.AbsoluteGameTime;
        prevRealTime = timeService.RealTime;
      }
      else
      {
        if (InstanceByRequest<EngineApplication>.Instance.IsPaused && !forcedUpdate)
          return;
        forcedUpdate = false;
        TimeSpan timeSpan1 = timeService.AbsoluteGameTime - prevTime;
        prevTime = timeService.AbsoluteGameTime;
        TimeSpan timeSpan2 = timeService.RealTime - prevRealTime;
        prevRealTime = timeService.RealTime;
        if (!Owner.IsEnabledInHierarchy || locationItemComponent != null && locationItemComponent.IsHibernation)
          return;
        currentTime += timeSpan1;
        currentRealTime += timeSpan2;
        float totalSeconds1 = (float) currentRealTime.TotalSeconds;
        float totalSeconds2 = (float) currentTime.TotalSeconds;
        if (EngineApplication.Sleep && !((Entity) Owner).IsPlayer)
          return;
        ComputeEffects(totalSeconds1, totalSeconds2);
      }
    }

    public void ForceComputeUpdate() => forcedUpdate = true;

    private void ComputeEffects(float currentRealTime, float currentGameTime)
    {
      foreach (IComputeParameter parameter in parameters.Parameters)
        parameter.ResetResetable();
      tmp.Clear();
      foreach (IEffect effect in effects)
        tmp.Add(effect);
      int index = 0;
      while (index < tmp.Count)
      {
        IEffect effect = tmp[index];
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
          tmp.RemoveAt(index);
        else
          ++index;
      }
      foreach (IEffect effect in tmp)
      {
        effect.Cleanup();
        effects.Remove(effect);
      }
      foreach (IComputeParameter parameter in parameters.Parameters)
        parameter.CorrectValue();
    }
  }
}

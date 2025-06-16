using System.Collections.Generic;
using Engine.Common;
using Engine.Common.Components.Parameters;
using Engine.Common.Generator;
using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Impl.Services.Factories;
using Engine.Source.Components;
using Engine.Source.Effects.Values;
using Inspectors;
using UnityEngine;

namespace Engine.Source.Commons.Abilities.Controllers
{
  [Factory]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class PreInfectionTrialAbilityController : 
    IAbilityController,
    IAbilityValueContainer,
    IUpdatable,
    IChangeParameterListener
  {
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected float thresholdValue = 0.3f;
    [DataReadProxy(Name = "BeakCoeficient1")]
    [DataWriteProxy(Name = "BeakCoeficient1")]
    [CopyableProxy]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected float breakCoeficient1 = 2.05f;
    [DataReadProxy(Name = "BeakCoeficient2")]
    [DataWriteProxy(Name = "BeakCoeficient2")]
    [CopyableProxy]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected float breakCoeficient2 = 0.3f;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy()]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected float preinfectionCheckInterval = 20f;
    private AbilityItem abilityItem;
    private ParametersComponent parameters;
    private IParameter<float> parameterPreInfection;
    private IParameter<float> parameterInfection;
    private TimeService timeService;
    private float nextCheckTime;
    private float lastPreinfectionValue;
    private bool gotPreinfectionHit;
    private Dictionary<AbilityValueNameEnum, IAbilityValue> values = new Dictionary<AbilityValueNameEnum, IAbilityValue>();

    public void Initialise(AbilityItem abilityItem)
    {
      this.abilityItem = abilityItem;
      timeService = ServiceLocator.GetService<TimeService>();
      parameters = this.abilityItem.Ability.Owner.GetComponent<ParametersComponent>();
      parameterPreInfection = parameters.GetByName<float>(ParameterNameEnum.PreInfection);
      if (parameterPreInfection != null)
      {
        lastPreinfectionValue = parameterPreInfection.Value;
        parameterPreInfection.AddListener(this);
        CheckParameter();
      }
      parameterInfection = parameters.GetByName<float>(ParameterNameEnum.Infection);
      gotPreinfectionHit = false;
      InstanceByRequest<UpdateService>.Instance.Updater.AddUpdatable(this);
    }

    public void Shutdown()
    {
      if (parameterPreInfection != null)
      {
        parameterPreInfection.RemoveListener(this);
        abilityItem.Active = false;
      }
      abilityItem = null;
      InstanceByRequest<UpdateService>.Instance.Updater.RemoveUpdatable(this);
    }

    private void CheckParameter()
    {
      if (parameterPreInfection == null)
        return;
      if (parameterPreInfection.Value > (double) thresholdValue && parameterPreInfection.Value > (double) lastPreinfectionValue)
        gotPreinfectionHit = true;
      lastPreinfectionValue = parameterPreInfection.Value;
    }

    public void ComputeUpdate()
    {
      if (gotPreinfectionHit && timeService.AbsoluteGameTime.TotalSeconds > nextCheckTime)
      {
        if (breakCoeficient1 * Mathf.Pow(parameterPreInfection.Value - breakCoeficient2, 2f) >= (double) Random.value)
        {
          values.Clear();
          values.Add(AbilityValueNameEnum.PreInfectionSuccessTrial, new AbilityValue<float> {
            Value = parameterPreInfection.Value
          });
          abilityItem.Active = true;
          abilityItem.Active = false;
        }
        nextCheckTime = (float) timeService.AbsoluteGameTime.TotalSeconds + preinfectionCheckInterval;
      }
      gotPreinfectionHit = false;
    }

    public IAbilityValue<T> GetAbilityValue<T>(AbilityValueNameEnum parameter) where T : struct
    {
      IAbilityValue abilityValue;
      values.TryGetValue(parameter, out abilityValue);
      return abilityValue as IAbilityValue<T>;
    }

    public void OnParameterChanged(IParameter parameter) => CheckParameter();
  }
}

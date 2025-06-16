// Decompiled with JetBrains decompiler
// Type: Engine.Source.Commons.Abilities.Controllers.PreInfectionTrialAbilityController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using Engine.Common.Components.Parameters;
using Engine.Common.Generator;
using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Impl.Services.Factories;
using Engine.Source.Components;
using Engine.Source.Effects.Values;
using Inspectors;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
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
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected float thresholdValue = 0.3f;
    [DataReadProxy(MemberEnum.None, Name = "BeakCoeficient1")]
    [DataWriteProxy(MemberEnum.None, Name = "BeakCoeficient1")]
    [CopyableProxy(MemberEnum.None)]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected float breakCoeficient1 = 2.05f;
    [DataReadProxy(MemberEnum.None, Name = "BeakCoeficient2")]
    [DataWriteProxy(MemberEnum.None, Name = "BeakCoeficient2")]
    [CopyableProxy(MemberEnum.None)]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected float breakCoeficient2 = 0.3f;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
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
      this.timeService = ServiceLocator.GetService<TimeService>();
      this.parameters = this.abilityItem.Ability.Owner.GetComponent<ParametersComponent>();
      this.parameterPreInfection = this.parameters.GetByName<float>(ParameterNameEnum.PreInfection);
      if (this.parameterPreInfection != null)
      {
        this.lastPreinfectionValue = this.parameterPreInfection.Value;
        this.parameterPreInfection.AddListener((IChangeParameterListener) this);
        this.CheckParameter();
      }
      this.parameterInfection = this.parameters.GetByName<float>(ParameterNameEnum.Infection);
      this.gotPreinfectionHit = false;
      InstanceByRequest<UpdateService>.Instance.Updater.AddUpdatable((IUpdatable) this);
    }

    public void Shutdown()
    {
      if (this.parameterPreInfection != null)
      {
        this.parameterPreInfection.RemoveListener((IChangeParameterListener) this);
        this.abilityItem.Active = false;
      }
      this.abilityItem = (AbilityItem) null;
      InstanceByRequest<UpdateService>.Instance.Updater.RemoveUpdatable((IUpdatable) this);
    }

    private void CheckParameter()
    {
      if (this.parameterPreInfection == null)
        return;
      if ((double) this.parameterPreInfection.Value > (double) this.thresholdValue && (double) this.parameterPreInfection.Value > (double) this.lastPreinfectionValue)
        this.gotPreinfectionHit = true;
      this.lastPreinfectionValue = this.parameterPreInfection.Value;
    }

    public void ComputeUpdate()
    {
      if (this.gotPreinfectionHit && this.timeService.AbsoluteGameTime.TotalSeconds > (double) this.nextCheckTime)
      {
        if ((double) (this.breakCoeficient1 * Mathf.Pow(this.parameterPreInfection.Value - this.breakCoeficient2, 2f)) >= (double) Random.value)
        {
          this.values.Clear();
          this.values.Add(AbilityValueNameEnum.PreInfectionSuccessTrial, (IAbilityValue) new AbilityValue<float>()
          {
            Value = this.parameterPreInfection.Value
          });
          this.abilityItem.Active = true;
          this.abilityItem.Active = false;
        }
        this.nextCheckTime = (float) this.timeService.AbsoluteGameTime.TotalSeconds + this.preinfectionCheckInterval;
      }
      this.gotPreinfectionHit = false;
    }

    public IAbilityValue<T> GetAbilityValue<T>(AbilityValueNameEnum parameter) where T : struct
    {
      IAbilityValue abilityValue;
      this.values.TryGetValue(parameter, out abilityValue);
      return abilityValue as IAbilityValue<T>;
    }

    public void OnParameterChanged(IParameter parameter) => this.CheckParameter();
  }
}

// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Common.EngineAPI.VMECS.VMPlayerController
// Assembly: VirtualMachine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FE3F54BA-1089-4F0E-B049-A4D27F3D2E73
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.Common.dll

using Cofe.Loggers;
using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Components;
using PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes;
using System;

#nullable disable
namespace PLVirtualMachine.Common.EngineAPI.VMECS
{
  [Info("PlayerControllerComponent", typeof (IPlayerControllerComponent))]
  public class VMPlayerController : VMEngineComponent<IPlayerControllerComponent>
  {
    public const string ComponentName = "PlayerControllerComponent";

    [Property("Is dead", "")]
    public bool IsDead
    {
      get
      {
        if (this.Component != null)
          return this.Component.IsDead.Value;
        Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", (object) this.Name, (object) this.Parent.Name));
        return false;
      }
      set
      {
        if (this.Component == null)
          Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", (object) this.Name, (object) this.Parent.Name));
        else
          this.Component.IsDead.Value = value;
      }
    }

    [Property("Is immortal", "", false, true, false)]
    public bool IsImmortal
    {
      get
      {
        if (this.Component != null)
          return this.Component.IsImmortal.Value;
        Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", (object) this.Name, (object) this.Parent.Name));
        return false;
      }
      set
      {
        if (this.Component == null)
          Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", (object) this.Name, (object) this.Parent.Name));
        else
          this.Component.IsImmortal.Value = value;
      }
    }

    [Property("Health", "", false, 1f, false)]
    public float Health
    {
      get
      {
        if (this.Component != null)
          return this.Component.Health.Value;
        Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", (object) this.Name, (object) this.Parent.Name));
        return 0.0f;
      }
      set
      {
        if (this.Component == null)
          Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", (object) this.Name, (object) this.Parent.Name));
        else
          this.Component.Health.Value = value;
      }
    }

    [Property("Max Health", "", false, 1f, false)]
    public float MaxHealth
    {
      get
      {
        if (this.Component != null)
          return this.Component.Health.MaxValue;
        Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", (object) this.Name, (object) this.Parent.Name));
        return 0.0f;
      }
      set
      {
        if (this.Component == null)
          Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", (object) this.Name, (object) this.Parent.Name));
        else
          this.Component.Health.MaxValue = value;
      }
    }

    [Property("Min Health", "", false, 1f, false)]
    public float MinHealth
    {
      get
      {
        if (this.Component != null)
          return this.Component.Health.MinValue;
        Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", (object) this.Name, (object) this.Parent.Name));
        return 0.0f;
      }
      set
      {
        if (this.Component == null)
          Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", (object) this.Name, (object) this.Parent.Name));
        else
          this.Component.Health.MinValue = value;
      }
    }

    [Property("Reputation", "")]
    public float Reputation
    {
      get
      {
        if (this.Component != null)
          return this.Component.Reputation.Value;
        Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", (object) this.Name, (object) this.Parent.Name));
        return 0.0f;
      }
    }

    [Property("Hunger", "")]
    public float Hunger
    {
      get
      {
        if (this.Component != null)
          return this.Component.Hunger.Value;
        Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", (object) this.Name, (object) this.Parent.Name));
        return 0.0f;
      }
      set
      {
        if (this.Component == null)
          Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", (object) this.Name, (object) this.Parent.Name));
        else
          this.Component.Hunger.Value = value;
      }
    }

    [Property("Max Hunger", "", false, 1f, false)]
    public float MaxHunger
    {
      get
      {
        if (this.Component != null)
          return this.Component.Hunger.MaxValue;
        Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", (object) this.Name, (object) this.Parent.Name));
        return 0.0f;
      }
      set
      {
        if (this.Component == null)
          Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", (object) this.Name, (object) this.Parent.Name));
        else
          this.Component.Hunger.MaxValue = value;
      }
    }

    [Property("Thirst", "")]
    public float Thirst
    {
      get
      {
        if (this.Component != null)
          return this.Component.Thirst.Value;
        Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", (object) this.Name, (object) this.Parent.Name));
        return 0.0f;
      }
      set
      {
        if (this.Component == null)
          Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", (object) this.Name, (object) this.Parent.Name));
        else
          this.Component.Thirst.Value = value;
      }
    }

    [Property("Fatigue", "")]
    public float Fatigue
    {
      get
      {
        if (this.Component != null)
          return this.Component.Fatigue.Value;
        Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", (object) this.Name, (object) this.Parent.Name));
        return 0.0f;
      }
      set
      {
        if (this.Component == null)
          Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", (object) this.Name, (object) this.Parent.Name));
        else
          this.Component.Fatigue.Value = value;
      }
    }

    [Property("Max Fatigue", "", false, 1f, false)]
    public float MaxFatigue
    {
      get
      {
        if (this.Component != null)
          return this.Component.Fatigue.MaxValue;
        Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", (object) this.Name, (object) this.Parent.Name));
        return 0.0f;
      }
      set
      {
        if (this.Component == null)
          Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", (object) this.Name, (object) this.Parent.Name));
        else
          this.Component.Fatigue.MaxValue = value;
      }
    }

    [Property("PreInfection", "")]
    public float PreInfection
    {
      get
      {
        if (this.Component != null)
          return this.Component.PreInfection.Value;
        Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", (object) this.Name, (object) this.Parent.Name));
        return 0.0f;
      }
      set
      {
        if (this.Component == null)
          Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", (object) this.Name, (object) this.Parent.Name));
        else
          this.Component.PreInfection.Value = value;
      }
    }

    [Property("Infection", "")]
    public float Infection
    {
      get
      {
        if (this.Component != null)
          return this.Component.Infection.Value;
        Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", (object) this.Name, (object) this.Parent.Name));
        return 0.0f;
      }
      set
      {
        if (this.Component == null)
          Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", (object) this.Name, (object) this.Parent.Name));
        else
          this.Component.Infection.Value = value;
      }
    }

    [Property("Immunity", "", false, 1f, false)]
    public float Immunity
    {
      get
      {
        if (this.Component != null)
          return this.Component.Immunity.Value;
        Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", (object) this.Name, (object) this.Parent.Name));
        return 0.0f;
      }
      set
      {
        if (this.Component == null)
          Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", (object) this.Name, (object) this.Parent.Name));
        else
          this.Component.Immunity.Value = value;
      }
    }

    [Property("Sleep", "")]
    public bool Sleep
    {
      get
      {
        if (this.Component != null)
          return this.Component.Sleep.Value;
        Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", (object) this.Name, (object) this.Parent.Name));
        return false;
      }
      set
      {
        if (this.Component == null)
          Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", (object) this.Name, (object) this.Parent.Name));
        else
          this.Component.Sleep.Value = value;
      }
    }

    [Property("CanTrade", "")]
    public bool CanTrade
    {
      get
      {
        if (this.Component != null)
          return this.Component.CanTrade.Value;
        Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", (object) this.Name, (object) this.Parent.Name));
        return false;
      }
      set
      {
        if (this.Component == null)
          Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", (object) this.Name, (object) this.Parent.Name));
        else
          this.Component.CanTrade.Value = value;
      }
    }

    [Property("Fraction", "", false, FractionEnum.Player, false)]
    public FractionEnum Fraction
    {
      get
      {
        if (this.Component != null)
          return this.Component.Fraction.Value;
        Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", (object) this.Name, (object) this.Parent.Name));
        return FractionEnum.None;
      }
      set
      {
        if (this.Component == null)
          Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", (object) this.Name, (object) this.Parent.Name));
        else
          this.Component.Fraction.Value = value;
      }
    }

    [Property("Fund Enabled", "", true, false)]
    public bool FundEnabled
    {
      get
      {
        if (this.Component != null)
          return this.Component.FundEnabled.Value;
        Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", (object) this.Name, (object) this.Parent.Name));
        return false;
      }
      set
      {
        if (this.Component == null)
          Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", (object) this.Name, (object) this.Parent.Name));
        else
          this.Component.FundEnabled.Value = value;
      }
    }

    [Property("Fund Finished", "", true, false)]
    public bool FundFinished
    {
      get
      {
        if (this.Component != null)
          return this.Component.FundFinished.Value;
        Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", (object) this.Name, (object) this.Parent.Name));
        return false;
      }
      set
      {
        if (this.Component == null)
          Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", (object) this.Name, (object) this.Parent.Name));
        else
          this.Component.FundFinished.Value = value;
      }
    }

    [Property("Fund Points", "", true, 0.0f, false)]
    public float FundPoints
    {
      get
      {
        if (this.Component != null)
          return this.Component.FundPoints.Value;
        Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", (object) this.Name, (object) this.Parent.Name));
        return 0.0f;
      }
      set
      {
        if (this.Component == null)
          Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", (object) this.Name, (object) this.Parent.Name));
        else
          this.Component.FundPoints.Value = value;
      }
    }

    [Property("Can Receive Mail", "", true, false)]
    public bool CanReceiveMail
    {
      get
      {
        if (this.Component != null)
          return this.Component.CanReceiveMail.Value;
        Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", (object) this.Name, (object) this.Parent.Name));
        return false;
      }
      set
      {
        if (this.Component == null)
          Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", (object) this.Name, (object) this.Parent.Name));
        else
          this.Component.CanReceiveMail.Value = value;
      }
    }

    public override void Clear()
    {
      if (!this.InstanceValid)
        return;
      this.Component.Health.ChangeValueEvent -= new Action<float>(this.ChangeHealthValueEvent);
      this.Component.Infection.ChangeValueEvent -= new Action<float>(this.ChangeInfectionValueEvent);
      this.Component.PreInfection.ChangeValueEvent -= new Action<float>(this.ChangePreInfectionValueEvent);
      this.Component.Sleep.ChangeValueEvent -= new Action<bool>(this.ChangeSleepValueEvent);
      this.Component.CombatActionEvent -= new Action<CombatActionEnum, IEntity>(this.OnCombatActionEvent);
      base.Clear();
    }

    protected override void Init()
    {
      if (this.IsTemplate)
        return;
      this.Component.Health.ChangeValueEvent += new Action<float>(this.ChangeHealthValueEvent);
      this.Component.Infection.ChangeValueEvent += new Action<float>(this.ChangeInfectionValueEvent);
      this.Component.PreInfection.ChangeValueEvent += new Action<float>(this.ChangePreInfectionValueEvent);
      this.Component.Sleep.ChangeValueEvent += new Action<bool>(this.ChangeSleepValueEvent);
      this.Component.CombatActionEvent += new Action<CombatActionEnum, IEntity>(this.OnCombatActionEvent);
    }

    private void ChangeSleepValueEvent(bool value)
    {
      Action<bool> onChangeSleep = this.OnChangeSleep;
      if (onChangeSleep == null)
        return;
      onChangeSleep(value);
    }

    private void ChangeHealthValueEvent(float value)
    {
      Action<float> onChangeHealth = this.OnChangeHealth;
      if (onChangeHealth == null)
        return;
      onChangeHealth(value);
    }

    private void ChangeInfectionValueEvent(float value)
    {
      Action<float> onChangeInfection = this.OnChangeInfection;
      if (onChangeInfection == null)
        return;
      onChangeInfection(value);
    }

    private void ChangePreInfectionValueEvent(float value)
    {
      Action<float> changePreInfection = this.OnChangePreInfection;
      if (changePreInfection == null)
        return;
      changePreInfection(value);
    }

    private void OnCombatActionEvent(CombatActionEnum action, IEntity target)
    {
      Action<CombatActionEnum, IEntity> combatActionEvent = this.CombatActionEvent;
      if (combatActionEvent == null)
        return;
      combatActionEvent(action, target);
    }

    [Event("Change health", "Value")]
    public event Action<float> OnChangeHealth;

    [Event("Change infection", "Value")]
    public event Action<float> OnChangeInfection;

    [Event("Change preInfection", "Value")]
    public event Action<float> OnChangePreInfection;

    [Event("Change sleep", "Value")]
    public event Action<bool> OnChangeSleep;

    [Event("CombatAction", "Action type, Entity")]
    public event Action<CombatActionEnum, IEntity> CombatActionEvent;
  }
}

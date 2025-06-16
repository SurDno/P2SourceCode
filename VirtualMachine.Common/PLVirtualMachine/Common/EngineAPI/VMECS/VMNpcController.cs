// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Common.EngineAPI.VMECS.VMNpcController
// Assembly: VirtualMachine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FE3F54BA-1089-4F0E-B049-A4D27F3D2E73
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.Common.dll

using Cofe.Loggers;
using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Components;
using Engine.Common.Services;
using PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes;
using System;

#nullable disable
namespace PLVirtualMachine.Common.EngineAPI.VMECS
{
  [Info("NpcControllerComponent", typeof (INpcControllerComponent))]
  public class VMNpcController : VMEngineComponent<INpcControllerComponent>
  {
    public const string ComponentName = "NpcControllerComponent";

    [Property("Is dead", "", false, true, false)]
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

    [Property("Can autopsy", "", false, true, false)]
    public bool CanAutopsy
    {
      get
      {
        if (this.Component != null)
          return this.Component.CanAutopsy.Value;
        Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", (object) this.Name, (object) this.Parent.Name));
        return false;
      }
      set
      {
        if (this.Component == null)
          Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", (object) this.Name, (object) this.Parent.Name));
        else
          this.Component.CanAutopsy.Value = value;
      }
    }

    [Property("Can trade", "", false, true, false)]
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

    [Property("Force trade", "", false, true, false)]
    public bool ForceTrade
    {
      get
      {
        if (this.Component != null)
          return this.Component.ForceTrade.Value;
        Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", (object) this.Name, (object) this.Parent.Name));
        return false;
      }
      set
      {
        if (this.Component == null)
          Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", (object) this.Name, (object) this.Parent.Name));
        else
          this.Component.ForceTrade.Value = value;
      }
    }

    [Property("Can heal", "", false, true, false)]
    public bool CanHeal
    {
      get
      {
        if (this.Component != null)
          return this.Component.CanHeal.Value;
        Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", (object) this.Name, (object) this.Parent.Name));
        return false;
      }
      set
      {
        if (this.Component == null)
          Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", (object) this.Name, (object) this.Parent.Name));
        else
          this.Component.CanHeal.Value = value;
      }
    }

    [Property("Infection", "", false, 0.0f, false)]
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

    [Property("PreInfection", "", false, 0.0f, false)]
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

    [Property("Pain", "", false, 0.0f, false)]
    public float Pain
    {
      get
      {
        if (this.Component != null)
          return this.Component.Pain.Value;
        Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", (object) this.Name, (object) this.Parent.Name));
        return 0.0f;
      }
      set
      {
        if (this.Component == null)
          Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", (object) this.Name, (object) this.Parent.Name));
        else
          this.Component.Pain.Value = value;
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

    [Property("Is away", "")]
    public bool IsAway => this.Component.IsAway.Value;

    [Property("Stamm Kind", "")]
    public StammKind StammKind
    {
      get
      {
        if (this.Component != null)
          return this.Component.StammKind.Value;
        Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", (object) this.Name, (object) this.Parent.Name));
        return StammKind.Unknown;
      }
      set
      {
        if (this.Component == null)
          Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", (object) this.Name, (object) this.Parent.Name));
        else
          this.Component.StammKind.Value = value;
      }
    }

    [Property("Fraction", "", false, FractionEnum.Civilian, false)]
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

    [Property("Combat style", "", false, CombatStyleEnum.Default, false)]
    public CombatStyleEnum CombatStyle
    {
      get
      {
        if (this.Component != null)
          return this.Component.CombatStyle.Value;
        Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", (object) this.Name, (object) this.Parent.Name));
        return CombatStyleEnum.None;
      }
      set
      {
        if (this.Component == null)
          Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", (object) this.Name, (object) this.Parent.Name));
        else
          this.Component.CombatStyle.Value = value;
      }
    }

    [Property("Bound health state", "", false, BoundHealthStateEnum.Normal, false)]
    public BoundHealthStateEnum BoundHealthState
    {
      get
      {
        if (this.Component != null)
          return this.Component.BoundHealthState.Value;
        Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", (object) this.Name, (object) this.Parent.Name));
        return BoundHealthStateEnum.None;
      }
      set
      {
        if (this.Component == null)
          Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", (object) this.Name, (object) this.Parent.Name));
        else
          this.Component.BoundHealthState.Value = value;
      }
    }

    [Property("Healing attempted", "", false, false, false)]
    public bool HealingAttempted
    {
      get
      {
        if (this.Component != null)
          return this.Component.HealingAttempted.Value;
        Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", (object) this.Name, (object) this.Parent.Name));
        return false;
      }
      set
      {
        if (this.Component == null)
          Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", (object) this.Name, (object) this.Parent.Name));
        else
          this.Component.HealingAttempted.Value = value;
      }
    }

    [Property("Immune boost attempted", "", false, false, false)]
    public bool ImmuneBoostAttempted
    {
      get
      {
        if (this.Component != null)
          return this.Component.ImmuneBoostAttempted.Value;
        Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", (object) this.Name, (object) this.Parent.Name));
        return false;
      }
      set
      {
        if (this.Component == null)
          Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", (object) this.Name, (object) this.Parent.Name));
        else
          this.Component.ImmuneBoostAttempted.Value = value;
      }
    }

    [Property("Is combat ignored", "", false, false, false)]
    public bool IsCombatIgnored
    {
      get
      {
        if (this.Component != null)
          return this.Component.IsCombatIgnored.Value;
        Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", (object) this.Name, (object) this.Parent.Name));
        return false;
      }
      set
      {
        if (this.Component == null)
          Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", (object) this.Name, (object) this.Parent.Name));
        else
          this.Component.IsCombatIgnored.Value = value;
      }
    }

    [Property("Say replics in idle", "", false, true, false)]
    public bool SayReplicsInIdle
    {
      get
      {
        if (this.Component != null)
          return this.Component.SayReplicsInIdle.Value;
        Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", (object) this.Name, (object) this.Parent.Name));
        return false;
      }
      set
      {
        if (this.Component == null)
          Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", (object) this.Name, (object) this.Parent.Name));
        else
          this.Component.SayReplicsInIdle.Value = value;
      }
    }

    [Method("Add Personal Enemy", "target", "")]
    public void AddPersonalEnemy(IEntity target)
    {
      ServiceLocator.GetService<ICombatService>().AddPersonalEnemy(this.Component.Owner, target);
    }

    [Method("Remove Personal Enemy", "target", "")]
    public void RemovePersonalEnemy(IEntity target)
    {
      ServiceLocator.GetService<ICombatService>().RemovePersonalEnemy(this.Component.Owner, target);
    }

    public override void Clear()
    {
      if (!this.InstanceValid)
        return;
      this.Component.ActionEvent -= new Action<ActionEnum>(this.OnActionEvent);
      this.Component.CombatActionEvent -= new Action<CombatActionEnum, IEntity>(this.OnCombatActionEvent);
      this.Component.Health.ChangeValueEvent -= new Action<float>(this.ChangeHealthValueEvent);
      this.Component.Pain.ChangeValueEvent -= new Action<float>(this.ChangePainValueEvent);
      this.Component.IsAway.ChangeValueEvent -= new Action<bool>(this.ChangeIsAwayValueEvent);
      base.Clear();
    }

    protected override void Init()
    {
      if (this.IsTemplate)
        return;
      this.Component.ActionEvent += new Action<ActionEnum>(this.OnActionEvent);
      this.Component.CombatActionEvent += new Action<CombatActionEnum, IEntity>(this.OnCombatActionEvent);
      this.Component.Health.ChangeValueEvent += new Action<float>(this.ChangeHealthValueEvent);
      this.Component.Pain.ChangeValueEvent += new Action<float>(this.ChangePainValueEvent);
      this.Component.IsAway.ChangeValueEvent += new Action<bool>(this.ChangeIsAwayValueEvent);
    }

    private void ChangeIsAwayValueEvent(bool value)
    {
      Action<bool> changeAwayEvent = this.ChangeAwayEvent;
      if (changeAwayEvent == null)
        return;
      changeAwayEvent(value);
    }

    private void ChangePainValueEvent(float value)
    {
      Action<float> onChangePain = this.OnChangePain;
      if (onChangePain == null)
        return;
      onChangePain(value);
    }

    private void ChangeHealthValueEvent(float value)
    {
      Action<float> onChangeHealth = this.OnChangeHealth;
      if (onChangeHealth == null)
        return;
      onChangeHealth(value);
    }

    private void OnActionEvent(ActionEnum action)
    {
      Action<ActionEnum> actionEvent = this.ActionEvent;
      if (actionEvent == null)
        return;
      actionEvent(action);
    }

    private void OnCombatActionEvent(CombatActionEnum action, IEntity target)
    {
      Action<CombatActionEnum, IEntity> combatActionEvent = this.CombatActionEvent;
      if (combatActionEvent == null)
        return;
      combatActionEvent(action, target);
    }

    [Event("Action", "Action type")]
    public event Action<ActionEnum> ActionEvent;

    [Event("CombatAction", "Action type, Entity")]
    public event Action<CombatActionEnum, IEntity> CombatActionEvent;

    [Event("Change health", "Value")]
    public event Action<float> OnChangeHealth;

    [Event("Change pain", "Value")]
    public event Action<float> OnChangePain;

    [Event("Change away", "Value")]
    public event Action<bool> ChangeAwayEvent;
  }
}

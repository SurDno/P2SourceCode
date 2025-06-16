using System;
using Cofe.Loggers;
using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Components;
using PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes;

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
        if (Component != null)
          return Component.IsDead.Value;
        Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
        return false;
      }
      set
      {
        if (Component == null)
          Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
        else
          Component.IsDead.Value = value;
      }
    }

    [Property("Is immortal", "", false, true, false)]
    public bool IsImmortal
    {
      get
      {
        if (Component != null)
          return Component.IsImmortal.Value;
        Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
        return false;
      }
      set
      {
        if (Component == null)
          Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
        else
          Component.IsImmortal.Value = value;
      }
    }

    [Property("Health", "", false, 1f, false)]
    public float Health
    {
      get
      {
        if (Component != null)
          return Component.Health.Value;
        Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
        return 0.0f;
      }
      set
      {
        if (Component == null)
          Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
        else
          Component.Health.Value = value;
      }
    }

    [Property("Max Health", "", false, 1f, false)]
    public float MaxHealth
    {
      get
      {
        if (Component != null)
          return Component.Health.MaxValue;
        Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
        return 0.0f;
      }
      set
      {
        if (Component == null)
          Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
        else
          Component.Health.MaxValue = value;
      }
    }

    [Property("Min Health", "", false, 1f, false)]
    public float MinHealth
    {
      get
      {
        if (Component != null)
          return Component.Health.MinValue;
        Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
        return 0.0f;
      }
      set
      {
        if (Component == null)
          Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
        else
          Component.Health.MinValue = value;
      }
    }

    [Property("Reputation", "")]
    public float Reputation
    {
      get
      {
        if (Component != null)
          return Component.Reputation.Value;
        Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
        return 0.0f;
      }
    }

    [Property("Hunger", "")]
    public float Hunger
    {
      get
      {
        if (Component != null)
          return Component.Hunger.Value;
        Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
        return 0.0f;
      }
      set
      {
        if (Component == null)
          Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
        else
          Component.Hunger.Value = value;
      }
    }

    [Property("Max Hunger", "", false, 1f, false)]
    public float MaxHunger
    {
      get
      {
        if (Component != null)
          return Component.Hunger.MaxValue;
        Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
        return 0.0f;
      }
      set
      {
        if (Component == null)
          Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
        else
          Component.Hunger.MaxValue = value;
      }
    }

    [Property("Thirst", "")]
    public float Thirst
    {
      get
      {
        if (Component != null)
          return Component.Thirst.Value;
        Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
        return 0.0f;
      }
      set
      {
        if (Component == null)
          Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
        else
          Component.Thirst.Value = value;
      }
    }

    [Property("Fatigue", "")]
    public float Fatigue
    {
      get
      {
        if (Component != null)
          return Component.Fatigue.Value;
        Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
        return 0.0f;
      }
      set
      {
        if (Component == null)
          Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
        else
          Component.Fatigue.Value = value;
      }
    }

    [Property("Max Fatigue", "", false, 1f, false)]
    public float MaxFatigue
    {
      get
      {
        if (Component != null)
          return Component.Fatigue.MaxValue;
        Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
        return 0.0f;
      }
      set
      {
        if (Component == null)
          Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
        else
          Component.Fatigue.MaxValue = value;
      }
    }

    [Property("PreInfection", "")]
    public float PreInfection
    {
      get
      {
        if (Component != null)
          return Component.PreInfection.Value;
        Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
        return 0.0f;
      }
      set
      {
        if (Component == null)
          Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
        else
          Component.PreInfection.Value = value;
      }
    }

    [Property("Infection", "")]
    public float Infection
    {
      get
      {
        if (Component != null)
          return Component.Infection.Value;
        Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
        return 0.0f;
      }
      set
      {
        if (Component == null)
          Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
        else
          Component.Infection.Value = value;
      }
    }

    [Property("Immunity", "", false, 1f, false)]
    public float Immunity
    {
      get
      {
        if (Component != null)
          return Component.Immunity.Value;
        Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
        return 0.0f;
      }
      set
      {
        if (Component == null)
          Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
        else
          Component.Immunity.Value = value;
      }
    }

    [Property("Sleep", "")]
    public bool Sleep
    {
      get
      {
        if (Component != null)
          return Component.Sleep.Value;
        Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
        return false;
      }
      set
      {
        if (Component == null)
          Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
        else
          Component.Sleep.Value = value;
      }
    }

    [Property("CanTrade", "")]
    public bool CanTrade
    {
      get
      {
        if (Component != null)
          return Component.CanTrade.Value;
        Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
        return false;
      }
      set
      {
        if (Component == null)
          Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
        else
          Component.CanTrade.Value = value;
      }
    }

    [Property("Fraction", "", false, FractionEnum.Player, false)]
    public FractionEnum Fraction
    {
      get
      {
        if (Component != null)
          return Component.Fraction.Value;
        Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
        return FractionEnum.None;
      }
      set
      {
        if (Component == null)
          Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
        else
          Component.Fraction.Value = value;
      }
    }

    [Property("Fund Enabled", "", true, false)]
    public bool FundEnabled
    {
      get
      {
        if (Component != null)
          return Component.FundEnabled.Value;
        Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
        return false;
      }
      set
      {
        if (Component == null)
          Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
        else
          Component.FundEnabled.Value = value;
      }
    }

    [Property("Fund Finished", "", true, false)]
    public bool FundFinished
    {
      get
      {
        if (Component != null)
          return Component.FundFinished.Value;
        Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
        return false;
      }
      set
      {
        if (Component == null)
          Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
        else
          Component.FundFinished.Value = value;
      }
    }

    [Property("Fund Points", "", true, 0.0f, false)]
    public float FundPoints
    {
      get
      {
        if (Component != null)
          return Component.FundPoints.Value;
        Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
        return 0.0f;
      }
      set
      {
        if (Component == null)
          Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
        else
          Component.FundPoints.Value = value;
      }
    }

    [Property("Can Receive Mail", "", true, false)]
    public bool CanReceiveMail
    {
      get
      {
        if (Component != null)
          return Component.CanReceiveMail.Value;
        Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
        return false;
      }
      set
      {
        if (Component == null)
          Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
        else
          Component.CanReceiveMail.Value = value;
      }
    }

    public override void Clear()
    {
      if (!InstanceValid)
        return;
      Component.Health.ChangeValueEvent -= ChangeHealthValueEvent;
      Component.Infection.ChangeValueEvent -= ChangeInfectionValueEvent;
      Component.PreInfection.ChangeValueEvent -= ChangePreInfectionValueEvent;
      Component.Sleep.ChangeValueEvent -= ChangeSleepValueEvent;
      Component.CombatActionEvent -= OnCombatActionEvent;
      base.Clear();
    }

    protected override void Init()
    {
      if (IsTemplate)
        return;
      Component.Health.ChangeValueEvent += ChangeHealthValueEvent;
      Component.Infection.ChangeValueEvent += ChangeInfectionValueEvent;
      Component.PreInfection.ChangeValueEvent += ChangePreInfectionValueEvent;
      Component.Sleep.ChangeValueEvent += ChangeSleepValueEvent;
      Component.CombatActionEvent += OnCombatActionEvent;
    }

    private void ChangeSleepValueEvent(bool value)
    {
      Action<bool> onChangeSleep = OnChangeSleep;
      if (onChangeSleep == null)
        return;
      onChangeSleep(value);
    }

    private void ChangeHealthValueEvent(float value)
    {
      Action<float> onChangeHealth = OnChangeHealth;
      if (onChangeHealth == null)
        return;
      onChangeHealth(value);
    }

    private void ChangeInfectionValueEvent(float value)
    {
      Action<float> onChangeInfection = OnChangeInfection;
      if (onChangeInfection == null)
        return;
      onChangeInfection(value);
    }

    private void ChangePreInfectionValueEvent(float value)
    {
      Action<float> changePreInfection = OnChangePreInfection;
      if (changePreInfection == null)
        return;
      changePreInfection(value);
    }

    private void OnCombatActionEvent(CombatActionEnum action, IEntity target)
    {
      Action<CombatActionEnum, IEntity> combatActionEvent = CombatActionEvent;
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

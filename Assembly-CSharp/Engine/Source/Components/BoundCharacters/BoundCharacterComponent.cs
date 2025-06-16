using Engine.Common;
using Engine.Common.BoundCharacters;
using Engine.Common.Commons;
using Engine.Common.Components;
using Engine.Common.Components.Parameters;
using Engine.Common.Generator;
using Engine.Common.Services;
using Engine.Common.Types;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;
using Engine.Source.Commons.Parameters;
using Engine.Source.Components.Maps;
using Engine.Source.Connections;
using Engine.Source.Services;
using Inspectors;
using System;

namespace Engine.Source.Components.BoundCharacters
{
  [Factory(typeof (IBoundCharacterComponent))]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite | TypeEnum.StateSave | TypeEnum.StateLoad)]
  public class BoundCharacterComponent : 
    EngineComponent,
    IBoundCharacterComponent,
    IComponent,
    INeedSave
  {
    [CopyableProxy(MemberEnum.None)]
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [StateSaveProxy(MemberEnum.None)]
    [StateLoadProxy(MemberEnum.None)]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected bool isEnabled = true;
    [StateSaveProxy(MemberEnum.None)]
    [StateLoadProxy(MemberEnum.None)]
    protected BoundCharacterGroup group;
    [StateSaveProxy(MemberEnum.None)]
    [StateLoadProxy(MemberEnum.None)]
    [Inspected(Mutable = true)]
    protected Typed<IBoundCharacterPlaceholder> placeholder;
    protected IEntity homeRegion;
    private bool added;
    [FromThis]
    private ParametersComponent parameters;
    [FromLocator]
    private BoundCharactersService boundCharactersService;

    [StateSaveProxy(MemberEnum.None)]
    [StateLoadProxy(MemberEnum.None)]
    [Inspected(Mutable = true)]
    public int SortOrder { get; set; }

    [StateSaveProxy(MemberEnum.None)]
    [StateLoadProxy(MemberEnum.None)]
    [Inspected(Mutable = true)]
    public BoundCharacterGroup SeenGroup { get; set; }

    [StateSaveProxy(MemberEnum.None)]
    [StateLoadProxy(MemberEnum.None)]
    [Inspected]
    public LocalizedText Name { get; set; } = LocalizedText.Empty;

    [Inspected]
    public IParameterValue<BoundHealthStateEnum> BoundHealthState { get; } = (IParameterValue<BoundHealthStateEnum>) new ParameterValue<BoundHealthStateEnum>();

    [Inspected]
    public IParameterValue<bool> HealingAttempted { get; } = (IParameterValue<bool>) new ParameterValue<bool>();

    [Inspected]
    public IParameterValue<bool> ImmuneBoosterAttempted { get; } = (IParameterValue<bool>) new ParameterValue<bool>();

    [Inspected]
    public IParameterValue<float> Immunity { get; } = (IParameterValue<float>) new ParameterValue<float>();

    [Inspected]
    public IParameterValue<float> Infection { get; } = (IParameterValue<float>) new ParameterValue<float>();

    [Inspected]
    public IParameterValue<float> RandomRoll { get; } = (IParameterValue<float>) new ParameterValue<float>();

    [StateSaveProxy(MemberEnum.None)]
    [StateLoadProxy(MemberEnum.None)]
    [Inspected(Mutable = true)]
    public BoundHealthStateEnum SeenBoundHealthState { get; set; } = BoundHealthStateEnum.None;

    [StateSaveProxy(MemberEnum.CustomReference)]
    [StateLoadProxy(MemberEnum.CustomReference)]
    [Inspected]
    public IEntity HomeRegion
    {
      get => this.homeRegion;
      set
      {
        if (this.homeRegion == value)
          return;
        if (this.homeRegion != null)
        {
          MapItemComponent component = this.homeRegion.GetComponent<MapItemComponent>();
          if (component != null)
            component.DiscoveredChangeEvent -= new Action(this.NotifyIfNotSeen);
        }
        this.homeRegion = value;
        if (this.homeRegion != null)
        {
          MapItemComponent component = this.homeRegion.GetComponent<MapItemComponent>();
          if (component != null)
            component.DiscoveredChangeEvent += new Action(this.NotifyIfNotSeen);
        }
        this.NotifyIfNotSeen();
      }
    }

    [Inspected]
    public bool PreRollStateStored { get; set; }

    [Inspected]
    public BoundHealthStateEnum PreRollHealthState { get; private set; }

    [Inspected]
    public float PreRollStatValue { get; private set; }

    [Inspected]
    public bool PreRollMedicated { get; set; }

    [Inspected(Mutable = true)]
    public bool IsEnabled
    {
      get => this.isEnabled;
      set
      {
        if (this.isEnabled == value)
          return;
        this.isEnabled = value;
        this.OnChangeEnabled();
      }
    }

    [Inspected(Mutable = true)]
    public BoundCharacterGroup Group
    {
      get => this.group;
      set
      {
        if (this.group == value)
          return;
        this.group = value;
        this.NotifyIfNotSeen();
      }
    }

    public bool Discovered
    {
      get => true;
      set
      {
      }
    }

    public IBoundCharacterPlaceholder Resource
    {
      get => this.placeholder.Value;
      set => this.placeholder.Value = value;
    }

    public bool NeedSave => true;

    private void NotifyIfNotSeen()
    {
      if (!this.added || !InstanceByRequest<EngineApplication>.Instance.ViewEnabled || this.group == BoundCharacterGroup.None)
        return;
      if (this.group == this.SeenGroup)
      {
        BoundHealthStateEnum boundHealthStateEnum = BoundCharacterUtility.PerceivedHealth(this);
        if (this.SeenBoundHealthState == boundHealthStateEnum || boundHealthStateEnum == BoundHealthStateEnum.Normal)
          return;
      }
      ServiceLocator.GetService<NotificationService>().AddNotify(NotificationEnum.BoundCharacters, Array.Empty<object>());
    }

    public override void OnAdded()
    {
      base.OnAdded();
      this.BoundHealthState.Set<BoundHealthStateEnum>(this.parameters?.GetByName<BoundHealthStateEnum>(ParameterNameEnum.BoundHealthState));
      this.BoundHealthState.ChangeValueEvent += new Action<BoundHealthStateEnum>(this.OnBoundHealthStateValueChange);
      this.HealingAttempted.Set<bool>(this.parameters?.GetByName<bool>(ParameterNameEnum.HealingAttempted));
      this.ImmuneBoosterAttempted.Set<bool>(this.parameters?.GetByName<bool>(ParameterNameEnum.ImmuneBoostAttempted));
      this.Immunity.Set<float>(this.parameters?.GetByName<float>(ParameterNameEnum.Immunity));
      this.Infection.Set<float>(this.parameters?.GetByName<float>(ParameterNameEnum.Infection));
      this.RandomRoll.Set<float>(this.parameters?.GetByName<float>(ParameterNameEnum.RandomRoll));
      this.OnEnableChangedEvent();
    }

    private void OnBoundHealthStateValueChange(BoundHealthStateEnum value)
    {
      this.NotifyIfNotSeen();
    }

    private void OnEnableChangedEvent()
    {
      if (this.IsEnabled && !this.IsDisposed)
        this.AddToService();
      else
        this.RemoveFromService();
    }

    public override void OnRemoved()
    {
      this.RemoveFromService();
      this.parameters = (ParametersComponent) null;
      this.BoundHealthState.ChangeValueEvent -= new Action<BoundHealthStateEnum>(this.OnBoundHealthStateValueChange);
      this.BoundHealthState.Set<BoundHealthStateEnum>((IParameter<BoundHealthStateEnum>) null);
      this.HealingAttempted.Set<bool>((IParameter<bool>) null);
      this.ImmuneBoosterAttempted.Set<bool>((IParameter<bool>) null);
      this.Immunity.Set<float>((IParameter<float>) null);
      this.Infection.Set<float>((IParameter<float>) null);
      this.RandomRoll.Set<float>((IParameter<float>) null);
      base.OnRemoved();
    }

    private void AddToService()
    {
      if (this.added)
        return;
      this.added = true;
      this.boundCharactersService.AddKeyCharacter(this);
      this.NotifyIfNotSeen();
    }

    private void RemoveFromService()
    {
      if (!this.added)
        return;
      this.added = false;
      this.boundCharactersService.RemoveKeyCharacter(this);
    }

    public override void OnChangeEnabled()
    {
      base.OnChangeEnabled();
      this.OnEnableChangedEvent();
    }

    [Cofe.Serializations.Data.OnLoaded]
    private void OnLoaded() => this.OnEnableChangedEvent();

    [Inspected]
    public void StorePreRollState()
    {
      this.PreRollStateStored = true;
      this.PreRollHealthState = this.BoundHealthState.Value;
      if (this.PreRollHealthState == BoundHealthStateEnum.Danger)
      {
        this.PreRollStatValue = this.Immunity.Value;
        this.PreRollMedicated = this.ImmuneBoosterAttempted.Value;
      }
      else if (this.PreRollHealthState == BoundHealthStateEnum.Diseased)
      {
        this.PreRollStatValue = this.Infection.Value;
        this.PreRollMedicated = this.HealingAttempted.Value;
      }
      else
        this.PreRollMedicated = false;
    }
  }
}

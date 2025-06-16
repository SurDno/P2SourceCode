// Decompiled with JetBrains decompiler
// Type: Engine.Source.Components.DoorComponent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Proxies;
using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Components;
using Engine.Common.Components.Parameters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;
using Engine.Source.Commons.Parameters;
using Engine.Source.Connections;
using Inspectors;
using System;
using System.Collections.Generic;

#nullable disable
namespace Engine.Source.Components
{
  [Required(typeof (ParametersComponent))]
  [Factory(typeof (IDoorComponent))]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite | TypeEnum.StateSave | TypeEnum.StateLoad)]
  public class DoorComponent : EngineComponent, IDoorComponent, IComponent, INeedSave
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [StateSaveProxy(MemberEnum.None)]
    [StateLoadProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    [CopyableProxy(MemberEnum.None)]
    protected PriorityContainer<List<Typed<IEntity>>> picklocks = ProxyFactory.Create<PriorityContainer<List<Typed<IEntity>>>>();
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [StateSaveProxy(MemberEnum.None)]
    [StateLoadProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    [CopyableProxy(MemberEnum.None)]
    protected PriorityContainer<List<Typed<IEntity>>> keys = ProxyFactory.Create<PriorityContainer<List<Typed<IEntity>>>>();
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    [CopyableProxy(MemberEnum.None)]
    protected bool isOutdoor;
    [FromThis]
    private ParametersComponent parameters;

    [Inspected]
    public IPriorityParameterValue<bool> IsFree { get; } = (IPriorityParameterValue<bool>) new PriorityParameterValue<bool>();

    [Inspected]
    public IPriorityParameterValue<bool> Opened { get; } = (IPriorityParameterValue<bool>) new PriorityParameterValue<bool>();

    [Inspected]
    public IPriorityParameterValue<bool> Bolted { get; } = (IPriorityParameterValue<bool>) new PriorityParameterValue<bool>();

    [Inspected]
    public IPriorityParameterValue<bool> Marked { get; } = (IPriorityParameterValue<bool>) new PriorityParameterValue<bool>();

    [Inspected]
    public IPriorityParameterValue<bool> SendEnterWithoutKnock { get; } = (IPriorityParameterValue<bool>) new PriorityParameterValue<bool>();

    [Inspected]
    public IPriorityParameterValue<Engine.Common.Components.Gate.LockState> LockState { get; } = (IPriorityParameterValue<Engine.Common.Components.Gate.LockState>) new PriorityParameterValue<Engine.Common.Components.Gate.LockState>();

    [Inspected]
    public IParameterValue<bool> CanBeMarked { get; } = (IParameterValue<bool>) new ParameterValue<bool>();

    [Inspected]
    public IParameterValue<bool> Knockable { get; } = (IParameterValue<bool>) new ParameterValue<bool>();

    [Inspected]
    public IParameterValue<bool> Pickable { get; } = (IParameterValue<bool>) new ParameterValue<bool>();

    [Inspected]
    public IParameterValue<int> Difficulty { get; } = (IParameterValue<int>) new ParameterValue<int>();

    public event Action<IDoorComponent> OnInvalidate;

    [Slider]
    [StateSaveProxy(MemberEnum.None)]
    [StateLoadProxy(MemberEnum.None)]
    [Inspected(Mutable = true)]
    public float MinReputation { get; set; }

    [Slider]
    [StateSaveProxy(MemberEnum.None)]
    [StateLoadProxy(MemberEnum.None)]
    [Inspected(Mutable = true)]
    public float MaxReputation { get; set; } = 1f;

    public bool IsOutdoor => this.isOutdoor;

    [Inspected]
    public IEnumerable<IEntity> Picklocks
    {
      get
      {
        List<Typed<IEntity>> items = this.picklocks.Value;
        if (items != null)
        {
          foreach (Typed<IEntity> typed in items)
          {
            Typed<IEntity> item = typed;
            IEntity value = item.Value;
            if (value != null)
              yield return value;
            value = (IEntity) null;
            item = new Typed<IEntity>();
          }
        }
      }
    }

    public void AddPicklock(IEntity item) => this.AddPicklock(PriorityParameterEnum.Default, item);

    public void RemovePicklock(IEntity item)
    {
      this.RemovePicklock(PriorityParameterEnum.Default, item);
    }

    public void AddPicklock(PriorityParameterEnum priority, IEntity item)
    {
      List<Typed<IEntity>> result;
      if (!this.picklocks.TryGetValue(priority, out result))
      {
        result = new List<Typed<IEntity>>();
        this.picklocks.SetValue(priority, result);
      }
      result.Add(new Typed<IEntity>(item.Id));
      this.FireInvalidate();
    }

    public void RemovePicklock(PriorityParameterEnum priority, IEntity item)
    {
      List<Typed<IEntity>> result;
      if (!this.picklocks.TryGetValue(priority, out result))
      {
        result = new List<Typed<IEntity>>();
        this.picklocks.SetValue(priority, result);
      }
      result.Remove(new Typed<IEntity>(item.Id));
      this.FireInvalidate();
    }

    public void ResetPicklocks(PriorityParameterEnum priority)
    {
      this.picklocks.ResetValue(priority);
      this.FireInvalidate();
    }

    [Inspected]
    public IEnumerable<IEntity> Keys
    {
      get
      {
        List<Typed<IEntity>> items = this.keys.Value;
        if (items != null)
        {
          foreach (Typed<IEntity> typed in items)
          {
            Typed<IEntity> item = typed;
            IEntity value = item.Value;
            if (value != null)
              yield return value;
            value = (IEntity) null;
            item = new Typed<IEntity>();
          }
        }
      }
    }

    public bool NeedSave => true;

    public void AddKey(IEntity item) => this.AddKey(PriorityParameterEnum.Default, item);

    public void RemoveKey(IEntity item) => this.RemoveKey(PriorityParameterEnum.Default, item);

    public void AddKey(PriorityParameterEnum priority, IEntity item)
    {
      List<Typed<IEntity>> result;
      if (!this.keys.TryGetValue(priority, out result))
      {
        result = new List<Typed<IEntity>>();
        this.keys.SetValue(priority, result);
      }
      result.Add(new Typed<IEntity>(item.Id));
      this.FireInvalidate();
    }

    public void RemoveKey(PriorityParameterEnum priority, IEntity item)
    {
      List<Typed<IEntity>> result;
      if (!this.keys.TryGetValue(priority, out result))
      {
        result = new List<Typed<IEntity>>();
        this.keys.SetValue(priority, result);
      }
      result.Remove(new Typed<IEntity>(item.Id));
      this.FireInvalidate();
    }

    public void ResetKeys(PriorityParameterEnum priority)
    {
      this.keys.ResetValue(priority);
      this.FireInvalidate();
    }

    [Cofe.Serializations.Data.OnLoaded]
    private void OnLoaded() => this.FireInvalidate();

    private void FireInvalidate()
    {
      Action<IDoorComponent> onInvalidate = this.OnInvalidate;
      if (onInvalidate == null)
        return;
      onInvalidate((IDoorComponent) this);
    }

    public override void OnAdded()
    {
      base.OnAdded();
      this.IsFree.Set<bool>(this.parameters.GetByName<bool>(ParameterNameEnum.IsFree));
      this.IsFree.ChangeValueEvent += new Action<bool>(this.ChangeBoolValueEvent);
      this.Opened.Set<bool>(this.parameters.GetByName<bool>(ParameterNameEnum.Opened));
      this.Opened.ChangeValueEvent += new Action<bool>(this.ChangeBoolValueEvent);
      this.Bolted.Set<bool>(this.parameters.GetByName<bool>(ParameterNameEnum.Bolted));
      this.Bolted.ChangeValueEvent += new Action<bool>(this.ChangeBoolValueEvent);
      this.Marked.Set<bool>(this.parameters.GetByName<bool>(ParameterNameEnum.Marked));
      this.Marked.ChangeValueEvent += new Action<bool>(this.ChangeBoolValueEvent);
      this.CanBeMarked.Set<bool>(this.parameters.GetByName<bool>(ParameterNameEnum.CanBeMarked));
      this.CanBeMarked.ChangeValueEvent += new Action<bool>(this.ChangeBoolValueEvent);
      this.SendEnterWithoutKnock.Set<bool>(this.parameters.GetByName<bool>(ParameterNameEnum.SendEnterWithoutKnock));
      this.SendEnterWithoutKnock.ChangeValueEvent += new Action<bool>(this.ChangeBoolValueEvent);
      this.LockState.Set<Engine.Common.Components.Gate.LockState>(this.parameters.GetByName<Engine.Common.Components.Gate.LockState>(ParameterNameEnum.LockState));
      this.LockState.ChangeValueEvent += new Action<Engine.Common.Components.Gate.LockState>(this.ChangeLockStateValueEvent);
      this.Knockable.Set<bool>(this.parameters.GetByName<bool>(ParameterNameEnum.DoorKnockable));
      this.Knockable.ChangeValueEvent += new Action<bool>(this.ChangeBoolValueEvent);
      this.Pickable.Set<bool>(this.parameters.GetByName<bool>(ParameterNameEnum.DoorPickable));
      this.Pickable.ChangeValueEvent += new Action<bool>(this.ChangeBoolValueEvent);
      this.Difficulty.Set<int>(this.parameters.GetByName<int>(ParameterNameEnum.DoorDifficulty));
      this.Difficulty.ChangeValueEvent += new Action<int>(this.ChangeIntValueEvent);
    }

    public override void OnRemoved()
    {
      this.Opened.Set<bool>((IParameter<bool>) null);
      this.Opened.ChangeValueEvent -= new Action<bool>(this.ChangeBoolValueEvent);
      this.Bolted.Set<bool>((IParameter<bool>) null);
      this.Bolted.ChangeValueEvent -= new Action<bool>(this.ChangeBoolValueEvent);
      this.Marked.Set<bool>((IParameter<bool>) null);
      this.Marked.ChangeValueEvent -= new Action<bool>(this.ChangeBoolValueEvent);
      this.CanBeMarked.Set<bool>((IParameter<bool>) null);
      this.CanBeMarked.ChangeValueEvent -= new Action<bool>(this.ChangeBoolValueEvent);
      this.SendEnterWithoutKnock.Set<bool>((IParameter<bool>) null);
      this.SendEnterWithoutKnock.ChangeValueEvent -= new Action<bool>(this.ChangeBoolValueEvent);
      this.LockState.Set<Engine.Common.Components.Gate.LockState>((IParameter<Engine.Common.Components.Gate.LockState>) null);
      this.LockState.ChangeValueEvent -= new Action<Engine.Common.Components.Gate.LockState>(this.ChangeLockStateValueEvent);
      this.IsFree.Set<bool>((IParameter<bool>) null);
      this.IsFree.ChangeValueEvent -= new Action<bool>(this.ChangeBoolValueEvent);
      this.Knockable.Set<bool>((IParameter<bool>) null);
      this.Knockable.ChangeValueEvent -= new Action<bool>(this.ChangeBoolValueEvent);
      this.Pickable.Set<bool>((IParameter<bool>) null);
      this.Pickable.ChangeValueEvent -= new Action<bool>(this.ChangeBoolValueEvent);
      this.Difficulty.Set<int>((IParameter<int>) null);
      this.Difficulty.ChangeValueEvent -= new Action<int>(this.ChangeIntValueEvent);
      base.OnRemoved();
    }

    private void ChangeValueEvent(bool value) => this.FireInvalidate();

    private void ChangeLockStateValueEvent(Engine.Common.Components.Gate.LockState value)
    {
      this.FireInvalidate();
    }

    private void ChangeBoolValueEvent(bool value) => this.FireInvalidate();

    private void ChangeIntValueEvent(int value) => this.FireInvalidate();
  }
}

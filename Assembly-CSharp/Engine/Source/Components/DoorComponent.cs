using System;
using System.Collections.Generic;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Components;
using Engine.Common.Components.Gate;
using Engine.Common.Components.Parameters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;
using Engine.Source.Commons.Parameters;
using Engine.Source.Connections;
using Inspectors;

namespace Engine.Source.Components
{
  [Required(typeof (ParametersComponent))]
  [Factory(typeof (IDoorComponent))]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite | TypeEnum.StateSave | TypeEnum.StateLoad)]
  public class DoorComponent : EngineComponent, IDoorComponent, IComponent, INeedSave
  {
    [DataReadProxy]
    [DataWriteProxy]
    [StateSaveProxy]
    [StateLoadProxy]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    [CopyableProxy]
    protected PriorityContainer<List<Typed<IEntity>>> picklocks = ProxyFactory.Create<PriorityContainer<List<Typed<IEntity>>>>();
    [DataReadProxy]
    [DataWriteProxy]
    [StateSaveProxy]
    [StateLoadProxy]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    [CopyableProxy]
    protected PriorityContainer<List<Typed<IEntity>>> keys = ProxyFactory.Create<PriorityContainer<List<Typed<IEntity>>>>();
    [DataReadProxy]
    [DataWriteProxy]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    [CopyableProxy()]
    protected bool isOutdoor;
    [FromThis]
    private ParametersComponent parameters;

    [Inspected]
    public IPriorityParameterValue<bool> IsFree { get; } = new PriorityParameterValue<bool>();

    [Inspected]
    public IPriorityParameterValue<bool> Opened { get; } = new PriorityParameterValue<bool>();

    [Inspected]
    public IPriorityParameterValue<bool> Bolted { get; } = new PriorityParameterValue<bool>();

    [Inspected]
    public IPriorityParameterValue<bool> Marked { get; } = new PriorityParameterValue<bool>();

    [Inspected]
    public IPriorityParameterValue<bool> SendEnterWithoutKnock { get; } = new PriorityParameterValue<bool>();

    [Inspected]
    public IPriorityParameterValue<LockState> LockState { get; } = new PriorityParameterValue<LockState>();

    [Inspected]
    public IParameterValue<bool> CanBeMarked { get; } = new ParameterValue<bool>();

    [Inspected]
    public IParameterValue<bool> Knockable { get; } = new ParameterValue<bool>();

    [Inspected]
    public IParameterValue<bool> Pickable { get; } = new ParameterValue<bool>();

    [Inspected]
    public IParameterValue<int> Difficulty { get; } = new ParameterValue<int>();

    public event Action<IDoorComponent> OnInvalidate;

    [Slider]
    [StateSaveProxy]
    [StateLoadProxy]
    [Inspected(Mutable = true)]
    public float MinReputation { get; set; }

    [Slider]
    [StateSaveProxy]
    [StateLoadProxy]
    [Inspected(Mutable = true)]
    public float MaxReputation { get; set; } = 1f;

    public bool IsOutdoor => isOutdoor;

    [Inspected]
    public IEnumerable<IEntity> Picklocks
    {
      get
      {
        List<Typed<IEntity>> items = picklocks.Value;
        if (items != null)
        {
          foreach (Typed<IEntity> typed in items)
          {
            Typed<IEntity> item = typed;
            IEntity value = item.Value;
            if (value != null)
              yield return value;
            value = null;
            item = new Typed<IEntity>();
          }
        }
      }
    }

    public void AddPicklock(IEntity item) => AddPicklock(PriorityParameterEnum.Default, item);

    public void RemovePicklock(IEntity item)
    {
      RemovePicklock(PriorityParameterEnum.Default, item);
    }

    public void AddPicklock(PriorityParameterEnum priority, IEntity item)
    {
      if (!picklocks.TryGetValue(priority, out List<Typed<IEntity>> result))
      {
        result = [];
        picklocks.SetValue(priority, result);
      }
      result.Add(new Typed<IEntity>(item.Id));
      FireInvalidate();
    }

    public void RemovePicklock(PriorityParameterEnum priority, IEntity item)
    {
      if (!picklocks.TryGetValue(priority, out List<Typed<IEntity>> result))
      {
        result = [];
        picklocks.SetValue(priority, result);
      }
      result.Remove(new Typed<IEntity>(item.Id));
      FireInvalidate();
    }

    public void ResetPicklocks(PriorityParameterEnum priority)
    {
      picklocks.ResetValue(priority);
      FireInvalidate();
    }

    [Inspected]
    public IEnumerable<IEntity> Keys
    {
      get
      {
        List<Typed<IEntity>> items = keys.Value;
        if (items != null)
        {
          foreach (Typed<IEntity> typed in items)
          {
            Typed<IEntity> item = typed;
            IEntity value = item.Value;
            if (value != null)
              yield return value;
            value = null;
            item = new Typed<IEntity>();
          }
        }
      }
    }

    public bool NeedSave => true;

    public void AddKey(IEntity item) => AddKey(PriorityParameterEnum.Default, item);

    public void RemoveKey(IEntity item) => RemoveKey(PriorityParameterEnum.Default, item);

    public void AddKey(PriorityParameterEnum priority, IEntity item)
    {
      if (!keys.TryGetValue(priority, out List<Typed<IEntity>> result))
      {
        result = [];
        keys.SetValue(priority, result);
      }
      result.Add(new Typed<IEntity>(item.Id));
      FireInvalidate();
    }

    public void RemoveKey(PriorityParameterEnum priority, IEntity item)
    {
      if (!keys.TryGetValue(priority, out List<Typed<IEntity>> result))
      {
        result = [];
        keys.SetValue(priority, result);
      }
      result.Remove(new Typed<IEntity>(item.Id));
      FireInvalidate();
    }

    public void ResetKeys(PriorityParameterEnum priority)
    {
      keys.ResetValue(priority);
      FireInvalidate();
    }

    [OnLoaded]
    private void OnLoaded() => FireInvalidate();

    private void FireInvalidate()
    {
      Action<IDoorComponent> onInvalidate = OnInvalidate;
      if (onInvalidate == null)
        return;
      onInvalidate(this);
    }

    public override void OnAdded()
    {
      base.OnAdded();
      IsFree.Set(parameters.GetByName<bool>(ParameterNameEnum.IsFree));
      IsFree.ChangeValueEvent += ChangeBoolValueEvent;
      Opened.Set(parameters.GetByName<bool>(ParameterNameEnum.Opened));
      Opened.ChangeValueEvent += ChangeBoolValueEvent;
      Bolted.Set(parameters.GetByName<bool>(ParameterNameEnum.Bolted));
      Bolted.ChangeValueEvent += ChangeBoolValueEvent;
      Marked.Set(parameters.GetByName<bool>(ParameterNameEnum.Marked));
      Marked.ChangeValueEvent += ChangeBoolValueEvent;
      CanBeMarked.Set(parameters.GetByName<bool>(ParameterNameEnum.CanBeMarked));
      CanBeMarked.ChangeValueEvent += ChangeBoolValueEvent;
      SendEnterWithoutKnock.Set(parameters.GetByName<bool>(ParameterNameEnum.SendEnterWithoutKnock));
      SendEnterWithoutKnock.ChangeValueEvent += ChangeBoolValueEvent;
      LockState.Set(parameters.GetByName<LockState>(ParameterNameEnum.LockState));
      LockState.ChangeValueEvent += ChangeLockStateValueEvent;
      Knockable.Set(parameters.GetByName<bool>(ParameterNameEnum.DoorKnockable));
      Knockable.ChangeValueEvent += ChangeBoolValueEvent;
      Pickable.Set(parameters.GetByName<bool>(ParameterNameEnum.DoorPickable));
      Pickable.ChangeValueEvent += ChangeBoolValueEvent;
      Difficulty.Set(parameters.GetByName<int>(ParameterNameEnum.DoorDifficulty));
      Difficulty.ChangeValueEvent += ChangeIntValueEvent;
    }

    public override void OnRemoved()
    {
      Opened.Set(null);
      Opened.ChangeValueEvent -= ChangeBoolValueEvent;
      Bolted.Set(null);
      Bolted.ChangeValueEvent -= ChangeBoolValueEvent;
      Marked.Set(null);
      Marked.ChangeValueEvent -= ChangeBoolValueEvent;
      CanBeMarked.Set(null);
      CanBeMarked.ChangeValueEvent -= ChangeBoolValueEvent;
      SendEnterWithoutKnock.Set(null);
      SendEnterWithoutKnock.ChangeValueEvent -= ChangeBoolValueEvent;
      LockState.Set(null);
      LockState.ChangeValueEvent -= ChangeLockStateValueEvent;
      IsFree.Set(null);
      IsFree.ChangeValueEvent -= ChangeBoolValueEvent;
      Knockable.Set(null);
      Knockable.ChangeValueEvent -= ChangeBoolValueEvent;
      Pickable.Set(null);
      Pickable.ChangeValueEvent -= ChangeBoolValueEvent;
      Difficulty.Set(null);
      Difficulty.ChangeValueEvent -= ChangeIntValueEvent;
      base.OnRemoved();
    }

    private void ChangeValueEvent(bool value) => FireInvalidate();

    private void ChangeLockStateValueEvent(LockState value)
    {
      FireInvalidate();
    }

    private void ChangeBoolValueEvent(bool value) => FireInvalidate();

    private void ChangeIntValueEvent(int value) => FireInvalidate();
  }
}

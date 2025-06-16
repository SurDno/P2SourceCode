using BehaviorDesigner.Runtime;
using Engine.Behaviours.Components;
using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Components;
using Engine.Common.Generator;
using Engine.Common.Services;
using Engine.Impl.Services.Factories;
using Engine.Source.BehaviorNodes;
using Engine.Source.Commons;
using Engine.Source.Connections;
using Inspectors;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Engine.Source.Components
{
  [Required(typeof (ParametersComponent))]
  [Required(typeof (EffectsComponent))]
  [Required(typeof (AbilitiesComponent))]
  [Factory(typeof (IBehaviorComponent))]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite | TypeEnum.StateSave | TypeEnum.StateLoad)]
  public class BehaviorComponent : 
    EngineComponent,
    IBehaviorComponent,
    IComponent,
    INeedSave,
    IUpdatable
  {
    [DataReadProxy(MemberEnum.None, Name = "BehaviorTree")]
    [DataWriteProxy(MemberEnum.None, Name = "BehaviorTree")]
    [StateSaveProxy(MemberEnum.None)]
    [StateLoadProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    [CopyableProxy(MemberEnum.None)]
    protected UnityAsset<ExternalBehaviorTree> behaviorTreeResource;
    private EngineBehavior behavior;
    private Dictionary<string, IEntity> values = new Dictionary<string, IEntity>();
    private Dictionary<string, bool> valuesBool = new Dictionary<string, bool>();
    private Dictionary<string, int> valuesInt = new Dictionary<string, int>();
    private Dictionary<string, float> valuesFloat = new Dictionary<string, float>();
    private bool geometryVisible;
    private BehaviorTree behaviorTree;
    private bool needUpdate;

    public event Action<string, bool> OnAbility;

    public event Action<IBehaviorComponent> SuccessEvent;

    public event Action<IBehaviorComponent> FailEvent;

    public event Action<IBehaviorComponent, string> CustomEvent;

    public void FireSuccessEvent()
    {
      Action<IBehaviorComponent> successEvent = this.SuccessEvent;
      if (successEvent == null)
        return;
      successEvent((IBehaviorComponent) this);
    }

    public void FireFailEvent()
    {
      Action<IBehaviorComponent> failEvent = this.FailEvent;
      if (failEvent == null)
        return;
      failEvent((IBehaviorComponent) this);
    }

    public void FireCustomEvent(string message)
    {
      Action<IBehaviorComponent, string> customEvent = this.CustomEvent;
      if (customEvent == null)
        return;
      customEvent((IBehaviorComponent) this, message);
    }

    [Inspected]
    public IBehaviorObject BehaviorObject
    {
      get
      {
        return ServiceLocator.GetService<ITemplateService>().GetTemplate<IBehaviorObject>(this.behaviorTreeResource.Id);
      }
      set
      {
        bool flag = false;
        NpcControllerComponent component = this.Owner.GetComponent<NpcControllerComponent>();
        if (component != null && component.IsDead != null)
          flag = component.IsDead.Value;
        if (flag)
          return;
        this.behaviorTreeResource = new UnityAsset<ExternalBehaviorTree>(value != null ? value.Id : Guid.Empty);
        this.values.Clear();
        this.valuesBool.Clear();
        this.valuesInt.Clear();
        this.valuesFloat.Clear();
        this.UpdateData(false);
      }
    }

    [Inspected]
    public IBehaviorObject BehaviorObjectForced
    {
      get
      {
        return ServiceLocator.GetService<ITemplateService>().GetTemplate<IBehaviorObject>(this.behaviorTreeResource.Id);
      }
      set
      {
        this.behaviorTreeResource = new UnityAsset<ExternalBehaviorTree>(value != null ? value.Id : Guid.Empty);
        this.UpdateData(false);
      }
    }

    [Inspected(Mutable = true)]
    public bool GeometryVisible
    {
      get => this.geometryVisible;
      set
      {
        this.geometryVisible = value;
        if (!((UnityEngine.Object) this.behavior != (UnityEngine.Object) null))
          return;
        this.behavior.GeometryVisible = this.geometryVisible;
      }
    }

    public bool NeedSave => true;

    public void SetValue(string name, IEntity value)
    {
      this.values[name] = value;
      this.UpdateData(false);
    }

    public void SetBoolValue(string name, bool value)
    {
      this.valuesBool[name] = value;
      this.UpdateData(false);
    }

    public void SetIntValue(string name, int value)
    {
      this.valuesInt[name] = value;
      this.UpdateData(false);
    }

    public void SetFloatValue(string name, float value)
    {
      this.valuesFloat[name] = value;
      this.UpdateData(false);
    }

    public void SendEvent(string name)
    {
      GameObject gameObject = ((IEntityView) this.Owner).GameObject;
      if ((UnityEngine.Object) gameObject == (UnityEngine.Object) null)
        return;
      if (this.needUpdate)
        this.UpdateData(true);
      BehaviorTree component = gameObject.GetComponent<BehaviorTree>();
      if ((UnityEngine.Object) component == (UnityEngine.Object) null)
        return;
      component.SendEvent(name);
    }

    public void SendEvent<T>(string name, T arg1)
    {
      GameObject gameObject = ((IEntityView) this.Owner).GameObject;
      if ((UnityEngine.Object) gameObject == (UnityEngine.Object) null)
        return;
      if (this.needUpdate)
        this.UpdateData(true);
      BehaviorTree component = gameObject.GetComponent<BehaviorTree>();
      if ((UnityEngine.Object) component == (UnityEngine.Object) null)
        return;
      component.SendEvent<object>(name, (object) arg1);
    }

    public override void OnAdded()
    {
      base.OnAdded();
      ((IEntityView) this.Owner).OnGameObjectChangedEvent += new Action(this.OnGameObjectChangedEvent);
      InstanceByRequest<EngineApplication>.Instance.OnPauseEvent += new Action(this.WorldPauseHandler);
      InstanceByRequest<UpdateService>.Instance.Updater.AddUpdatable((IUpdatable) this);
      this.OnGameObjectChangedEvent();
    }

    public override void OnRemoved()
    {
      this.behavior = (EngineBehavior) null;
      ((IEntityView) this.Owner).OnGameObjectChangedEvent -= new Action(this.OnGameObjectChangedEvent);
      InstanceByRequest<EngineApplication>.Instance.OnPauseEvent -= new Action(this.WorldPauseHandler);
      InstanceByRequest<UpdateService>.Instance.Updater.RemoveUpdatable((IUpdatable) this);
      base.OnRemoved();
    }

    private void WorldPauseHandler()
    {
      if ((UnityEngine.Object) this.behaviorTree != (UnityEngine.Object) null)
      {
        if (InstanceByRequest<EngineApplication>.Instance.IsPaused)
          this.behaviorTree.DisableBehavior(true);
        else
          this.behaviorTree.EnableBehavior();
      }
      if (!((UnityEngine.Object) this.behavior != (UnityEngine.Object) null))
        return;
      this.behavior.IsPaused = InstanceByRequest<EngineApplication>.Instance.IsPaused;
    }

    private void OnGameObjectChangedEvent()
    {
      IEntityView owner = (IEntityView) this.Owner;
      if ((UnityEngine.Object) owner.GameObject == (UnityEngine.Object) null)
      {
        this.behavior = (EngineBehavior) null;
      }
      else
      {
        this.UpdateData(false);
        this.behavior = owner.GameObject.GetComponent<EngineBehavior>();
        if ((UnityEngine.Object) this.behavior == (UnityEngine.Object) null)
          return;
        this.behavior.Gait = EngineBehavior.GaitType.Walk;
        this.WorldPauseHandler();
      }
    }

    public void SetBehaviorForced(IBehaviorObject behaviorObject)
    {
      this.behaviorTreeResource = new UnityAsset<ExternalBehaviorTree>(behaviorObject != null ? behaviorObject.Id : Guid.Empty);
      this.values.Clear();
      this.valuesBool.Clear();
      this.valuesInt.Clear();
      this.valuesFloat.Clear();
      this.UpdateData(false);
    }

    public void ComputeUpdate()
    {
      if (!this.needUpdate)
        return;
      this.UpdateData(false);
    }

    private void UpdateData(bool forced)
    {
      if (ServiceCache.OptimizationService.FrameHasSpike && !forced)
      {
        this.needUpdate = true;
      }
      else
      {
        ServiceCache.OptimizationService.FrameHasSpike = true;
        this.needUpdate = false;
        if ((UnityEngine.Object) this.behaviorTree != (UnityEngine.Object) null)
        {
          this.behaviorTree.UnregisterEvent<object, object>("Ability", new Action<object, object>(this.AbilityEvent));
          this.behaviorTree = (BehaviorTree) null;
        }
        GameObject gameObject = ((IEntityView) this.Owner).GameObject;
        if ((UnityEngine.Object) gameObject == (UnityEngine.Object) null)
          return;
        this.behaviorTree = gameObject.GetComponent<BehaviorTree>();
        if ((UnityEngine.Object) this.behaviorTree == (UnityEngine.Object) null)
          return;
        this.behaviorTree.RegisterEvent<object, object>("Ability", new Action<object, object>(this.AbilityEvent));
        ExternalBehaviorTree externalBehaviorTree = ((Engine.Source.Commons.BehaviorObject) this.BehaviorObject)?.ExternalBehaviorTree;
        if ((UnityEngine.Object) this.behaviorTree.ExternalBehaviorTree != (UnityEngine.Object) externalBehaviorTree)
          this.behaviorTree.ExternalBehaviorTree = externalBehaviorTree;
        if (this.values.Count != 0)
        {
          foreach (KeyValuePair<string, IEntity> keyValuePair in this.values)
            this.behaviorTree.SetVariableValue(keyValuePair.Key, (object) new SharedEntity()
            {
              Entity = keyValuePair.Value
            });
          this.values.Clear();
        }
        if (this.valuesBool.Count != 0)
        {
          foreach (KeyValuePair<string, bool> keyValuePair in this.valuesBool)
          {
            BehaviorTree behaviorTree = this.behaviorTree;
            string key = keyValuePair.Key;
            SharedBool sharedBool = new SharedBool();
            sharedBool.Value = keyValuePair.Value;
            behaviorTree.SetVariableValue(key, (object) sharedBool);
          }
          this.valuesBool.Clear();
        }
        if (this.valuesInt.Count != 0)
        {
          foreach (KeyValuePair<string, int> keyValuePair in this.valuesInt)
          {
            BehaviorTree behaviorTree = this.behaviorTree;
            string key = keyValuePair.Key;
            SharedInt sharedInt = new SharedInt();
            sharedInt.Value = keyValuePair.Value;
            behaviorTree.SetVariableValue(key, (object) sharedInt);
          }
          this.valuesInt.Clear();
        }
        if (this.valuesFloat.Count == 0)
          return;
        foreach (KeyValuePair<string, float> keyValuePair in this.valuesFloat)
        {
          BehaviorTree behaviorTree = this.behaviorTree;
          string key = keyValuePair.Key;
          SharedFloat sharedFloat = new SharedFloat();
          sharedFloat.Value = keyValuePair.Value;
          behaviorTree.SetVariableValue(key, (object) sharedFloat);
        }
        this.valuesFloat.Clear();
      }
    }

    private void AbilityEvent(object name, object enable)
    {
      Action<string, bool> onAbility = this.OnAbility;
      if (onAbility == null)
        return;
      onAbility((string) name, (bool) enable);
    }
  }
}

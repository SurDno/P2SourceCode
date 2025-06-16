using System;
using System.Collections.Generic;
using AssetDatabases;
using Cofe.Utility;
using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Components;
using Engine.Common.Generator;
using Engine.Common.Services;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;
using Engine.Source.Connections;
using Engine.Source.Services;
using FlowCanvas;
using Inspectors;

namespace Engine.Source.Components
{
  [Factory(typeof (IBlueprintComponent))]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite | TypeEnum.StateSave | TypeEnum.StateLoad)]
  public class BlueprintComponent : 
    EngineComponent,
    IBlueprintComponent,
    IComponent,
    INeedSave,
    IUpdatable
  {
    private bool async = true;
    private bool asyncStarted;
    private IEntity asyncTarget;
    private List<string> asyncEvents = new List<string>();
    [StateSaveProxy]
    [StateLoadProxy]
    [DataReadProxy]
    [DataWriteProxy]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    [CopyableProxy]
    protected bool isEnabled = true;
    [DataReadProxy(Name = "Blueprint")]
    [DataWriteProxy(Name = "Blueprint")]
    [StateSaveProxy]
    [StateLoadProxy]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    [CopyableProxy()]
    protected UnityAsset<GameObject> blueprintResource;
    [Inspected]
    private FlowScriptController controller;
    [FromThis]
    private ILocationItemComponent locationItem;

    [Inspected(Mutable = true)]
    public bool IsEnabled
    {
      get => isEnabled;
      set
      {
        isEnabled = value;
        OnChangeEnabled();
      }
    }

    [Inspected]
    public bool IsAttached { get; private set; }

    public event Action<IBlueprintComponent> CompleteEvent;

    public event Action<IBlueprintComponent> AttachEvent;

    public IBlueprintObject Blueprint
    {
      get
      {
        return ServiceLocator.GetService<ITemplateService>().GetTemplate<IBlueprintObject>(blueprintResource.Id);
      }
      set
      {
        if ((UnityEngine.Object) controller != (UnityEngine.Object) null)
          Debug.LogError((object) ("Instance already started : " + Owner.GetInfo()));
        else
          blueprintResource = new UnityAsset<GameObject>(value != null ? value.Id : Guid.Empty);
      }
    }

    public bool IsStarted
    {
      get => async ? asyncStarted : (UnityEngine.Object) controller != (UnityEngine.Object) null;
    }

    public bool NeedSave => true;

    public void Start(IEntity target)
    {
      if ((UnityEngine.Object) controller != (UnityEngine.Object) null)
        Debug.LogError((object) ("Instance already exist : " + Owner.GetInfo()));
      else if (async)
      {
        if (asyncStarted)
        {
          Debug.LogError((object) ("Instance already async started : " + Owner.GetInfo()));
        }
        else
        {
          UnityAsset<GameObject> blueprintResource = this.blueprintResource;
          if (false)
          {
            Debug.LogError((object) ("blueprintResource is null : " + Owner.GetInfo()));
          }

          string resourcePath = AssetDatabaseUtility.ConvertToResourcePath(AssetDatabaseService.Instance.GetPath(this.blueprintResource.Id));
          if (resourcePath.IsNullOrEmpty())
          {
            Debug.LogError((object) ("Path is null : " + Owner.GetInfo()));
          }
          else
          {
            ResourceRequest resourceRequest = Resources.LoadAsync(resourcePath);
            if (resourceRequest == null)
            {
              Debug.LogError((object) ("LoadAsync failed : " + Owner.GetInfo()));
            }
            else
            {
              asyncTarget = target;
              asyncStarted = true;
              asyncEvents.Clear();
              resourceRequest.completed += new Action<AsyncOperation>(Request_completed);
            }
          }
        }
      }
      else
        controller = BlueprintServiceUtility.Start(Blueprint, target, (Action) (() =>
        {
          if (!((UnityEngine.Object) controller != (UnityEngine.Object) null))
            return;
          Action<IBlueprintComponent> completeEvent = CompleteEvent;
          if (completeEvent != null)
            completeEvent(this);
          controller = null;
        }), Owner.GetInfo());
    }

    private void Request_completed(AsyncOperation async)
    {
      async.completed -= new Action<AsyncOperation>(Request_completed);
      if (!asyncStarted)
        return;
      controller = BlueprintServiceUtility.Start(Blueprint, asyncTarget, (Action) (() =>
      {
        if (!((UnityEngine.Object) controller != (UnityEngine.Object) null))
          return;
        Action<IBlueprintComponent> completeEvent = CompleteEvent;
        if (completeEvent != null)
          completeEvent(this);
        controller = null;
        asyncStarted = false;
      }), Owner.GetInfo());
      foreach (string asyncEvent in asyncEvents)
        controller.SendEvent(asyncEvent);
      asyncEvents.Clear();
    }

    [Inspected]
    public void Start() => Start(Owner);

    public void Stop()
    {
      if (async)
      {
        if (!asyncStarted)
          Debug.LogError((object) ("Stop to not asyncStarted: " + Owner.GetInfo()));
        else if ((UnityEngine.Object) controller == (UnityEngine.Object) null)
        {
          asyncStarted = false;
          Action<IBlueprintComponent> completeEvent = CompleteEvent;
          if (completeEvent == null)
            return;
          completeEvent(this);
        }
        else
        {
          controller.SendEvent(nameof (Stop));
          controller = null;
          Action<IBlueprintComponent> completeEvent = CompleteEvent;
          if (completeEvent != null)
            completeEvent(this);
        }
      }
      else if ((UnityEngine.Object) controller == (UnityEngine.Object) null)
      {
        Debug.LogError((object) ("Instance already destroed: " + Owner.GetInfo()));
      }
      else
      {
        controller.SendEvent(nameof (Stop));
        controller = null;
        Action<IBlueprintComponent> completeEvent = CompleteEvent;
        if (completeEvent != null)
          completeEvent(this);
      }
    }

    public void SendEvent(string name)
    {
      if (async)
      {
        if (!asyncStarted)
          Debug.LogError((object) ("SendEvent to not asyncStarted: " + Owner.GetInfo()));
        else if ((UnityEngine.Object) controller != (UnityEngine.Object) null)
          controller.SendEvent(name);
        else
          asyncEvents.Add(name);
      }
      else if ((UnityEngine.Object) controller == (UnityEngine.Object) null)
        Debug.LogError((object) ("Instance already destroed: " + Owner.GetInfo()));
      else
        controller.SendEvent(name);
    }

    public override void OnAdded()
    {
      base.OnAdded();
      InstanceByRequest<UpdateService>.Instance.Updater.AddUpdatable(this);
    }

    public override void OnRemoved()
    {
      base.OnRemoved();
      InstanceByRequest<UpdateService>.Instance.Updater.RemoveUpdatable(this);
      if (!((UnityEngine.Object) controller != (UnityEngine.Object) null))
        return;
      UnityEngine.Object.Destroy((UnityEngine.Object) controller.gameObject);
      controller = null;
    }

    public void ComputeUpdate()
    {
      bool flag = locationItem != null && !locationItem.IsHibernation && ((IEntityView) Owner).IsAttached;
      if (IsAttached == flag)
        return;
      IsAttached = flag;
      Action<IBlueprintComponent> attachEvent = AttachEvent;
      if (attachEvent != null)
        attachEvent(this);
    }
  }
}

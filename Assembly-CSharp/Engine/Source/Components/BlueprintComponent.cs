// Decompiled with JetBrains decompiler
// Type: Engine.Source.Components.BlueprintComponent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

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
using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
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
    [StateSaveProxy(MemberEnum.None)]
    [StateLoadProxy(MemberEnum.None)]
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    [CopyableProxy(MemberEnum.None)]
    protected bool isEnabled = true;
    [DataReadProxy(MemberEnum.None, Name = "Blueprint")]
    [DataWriteProxy(MemberEnum.None, Name = "Blueprint")]
    [StateSaveProxy(MemberEnum.None)]
    [StateLoadProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    [CopyableProxy(MemberEnum.None)]
    protected UnityAsset<GameObject> blueprintResource;
    [Inspected]
    private FlowScriptController controller;
    [FromThis]
    private ILocationItemComponent locationItem;

    [Inspected(Mutable = true)]
    public bool IsEnabled
    {
      get => this.isEnabled;
      set
      {
        this.isEnabled = value;
        this.OnChangeEnabled();
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
        return ServiceLocator.GetService<ITemplateService>().GetTemplate<IBlueprintObject>(this.blueprintResource.Id);
      }
      set
      {
        if ((UnityEngine.Object) this.controller != (UnityEngine.Object) null)
          Debug.LogError((object) ("Instance already started : " + this.Owner.GetInfo()));
        else
          this.blueprintResource = new UnityAsset<GameObject>(value != null ? value.Id : Guid.Empty);
      }
    }

    public bool IsStarted
    {
      get => this.async ? this.asyncStarted : (UnityEngine.Object) this.controller != (UnityEngine.Object) null;
    }

    public bool NeedSave => true;

    public void Start(IEntity target)
    {
      if ((UnityEngine.Object) this.controller != (UnityEngine.Object) null)
        Debug.LogError((object) ("Instance already exist : " + this.Owner.GetInfo()));
      else if (this.async)
      {
        if (this.asyncStarted)
        {
          Debug.LogError((object) ("Instance already async started : " + this.Owner.GetInfo()));
        }
        else
        {
          UnityAsset<GameObject> blueprintResource = this.blueprintResource;
          if (false)
          {
            Debug.LogError((object) ("blueprintResource is null : " + this.Owner.GetInfo()));
          }
          else
          {
            string resourcePath = AssetDatabaseUtility.ConvertToResourcePath(AssetDatabaseService.Instance.GetPath(this.blueprintResource.Id));
            if (resourcePath.IsNullOrEmpty())
            {
              Debug.LogError((object) ("Path is null : " + this.Owner.GetInfo()));
            }
            else
            {
              ResourceRequest resourceRequest = Resources.LoadAsync(resourcePath);
              if (resourceRequest == null)
              {
                Debug.LogError((object) ("LoadAsync failed : " + this.Owner.GetInfo()));
              }
              else
              {
                this.asyncTarget = target;
                this.asyncStarted = true;
                this.asyncEvents.Clear();
                resourceRequest.completed += new Action<AsyncOperation>(this.Request_completed);
              }
            }
          }
        }
      }
      else
        this.controller = BlueprintServiceUtility.Start(this.Blueprint, target, (Action) (() =>
        {
          if (!((UnityEngine.Object) this.controller != (UnityEngine.Object) null))
            return;
          Action<IBlueprintComponent> completeEvent = this.CompleteEvent;
          if (completeEvent != null)
            completeEvent((IBlueprintComponent) this);
          this.controller = (FlowScriptController) null;
        }), this.Owner.GetInfo());
    }

    private void Request_completed(AsyncOperation async)
    {
      async.completed -= new Action<AsyncOperation>(this.Request_completed);
      if (!this.asyncStarted)
        return;
      this.controller = BlueprintServiceUtility.Start(this.Blueprint, this.asyncTarget, (Action) (() =>
      {
        if (!((UnityEngine.Object) this.controller != (UnityEngine.Object) null))
          return;
        Action<IBlueprintComponent> completeEvent = this.CompleteEvent;
        if (completeEvent != null)
          completeEvent((IBlueprintComponent) this);
        this.controller = (FlowScriptController) null;
        this.asyncStarted = false;
      }), this.Owner.GetInfo());
      foreach (string asyncEvent in this.asyncEvents)
        this.controller.SendEvent(asyncEvent);
      this.asyncEvents.Clear();
    }

    [Inspected]
    public void Start() => this.Start(this.Owner);

    public void Stop()
    {
      if (this.async)
      {
        if (!this.asyncStarted)
          Debug.LogError((object) ("Stop to not asyncStarted: " + this.Owner.GetInfo()));
        else if ((UnityEngine.Object) this.controller == (UnityEngine.Object) null)
        {
          this.asyncStarted = false;
          Action<IBlueprintComponent> completeEvent = this.CompleteEvent;
          if (completeEvent == null)
            return;
          completeEvent((IBlueprintComponent) this);
        }
        else
        {
          this.controller.SendEvent(nameof (Stop));
          this.controller = (FlowScriptController) null;
          Action<IBlueprintComponent> completeEvent = this.CompleteEvent;
          if (completeEvent != null)
            completeEvent((IBlueprintComponent) this);
        }
      }
      else if ((UnityEngine.Object) this.controller == (UnityEngine.Object) null)
      {
        Debug.LogError((object) ("Instance already destroed: " + this.Owner.GetInfo()));
      }
      else
      {
        this.controller.SendEvent(nameof (Stop));
        this.controller = (FlowScriptController) null;
        Action<IBlueprintComponent> completeEvent = this.CompleteEvent;
        if (completeEvent != null)
          completeEvent((IBlueprintComponent) this);
      }
    }

    public void SendEvent(string name)
    {
      if (this.async)
      {
        if (!this.asyncStarted)
          Debug.LogError((object) ("SendEvent to not asyncStarted: " + this.Owner.GetInfo()));
        else if ((UnityEngine.Object) this.controller != (UnityEngine.Object) null)
          this.controller.SendEvent(name);
        else
          this.asyncEvents.Add(name);
      }
      else if ((UnityEngine.Object) this.controller == (UnityEngine.Object) null)
        Debug.LogError((object) ("Instance already destroed: " + this.Owner.GetInfo()));
      else
        this.controller.SendEvent(name);
    }

    public override void OnAdded()
    {
      base.OnAdded();
      InstanceByRequest<UpdateService>.Instance.Updater.AddUpdatable((IUpdatable) this);
    }

    public override void OnRemoved()
    {
      base.OnRemoved();
      InstanceByRequest<UpdateService>.Instance.Updater.RemoveUpdatable((IUpdatable) this);
      if (!((UnityEngine.Object) this.controller != (UnityEngine.Object) null))
        return;
      UnityEngine.Object.Destroy((UnityEngine.Object) this.controller.gameObject);
      this.controller = (FlowScriptController) null;
    }

    public void ComputeUpdate()
    {
      bool flag = this.locationItem != null && !this.locationItem.IsHibernation && ((IEntityView) this.Owner).IsAttached;
      if (this.IsAttached == flag)
        return;
      this.IsAttached = flag;
      Action<IBlueprintComponent> attachEvent = this.AttachEvent;
      if (attachEvent != null)
        attachEvent((IBlueprintComponent) this);
    }
  }
}

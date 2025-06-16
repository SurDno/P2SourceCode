// Decompiled with JetBrains decompiler
// Type: Engine.Source.Components.DynamicModelComponent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Utility;
using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Components;
using Engine.Common.Generator;
using Engine.Common.Services;
using Engine.Impl.Services.Factories;
using Engine.Services.Engine.Assets;
using Engine.Source.Commons;
using Engine.Source.Connections;
using Engine.Source.Settings.External;
using Inspectors;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Profiling;

#nullable disable
namespace Engine.Source.Components
{
  [Required(typeof (LocationItemComponent))]
  [Factory(typeof (IDynamicModelComponent))]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite | TypeEnum.StateSave | TypeEnum.StateLoad)]
  public class DynamicModelComponent : 
    EngineComponent,
    IDynamicModelComponent,
    IComponent,
    INeedSave,
    IUpdatable
  {
    [StateSaveProxy(MemberEnum.None)]
    [StateLoadProxy(MemberEnum.None)]
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    [CopyableProxy(MemberEnum.None)]
    protected bool isEnabled = true;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [StateSaveProxy(MemberEnum.None)]
    [StateLoadProxy(MemberEnum.None)]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    [CopyableProxy(MemberEnum.None)]
    protected Typed<IModel> model;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    [CopyableProxy(MemberEnum.None)]
    protected List<Typed<IModel>> models = new List<Typed<IModel>>();
    private PrefabAsset prefabAsset;
    private string group;
    [FromThis]
    private ILocationItemComponent locationItem;
    [FromLocator]
    private AssetLoader assetLoader;
    private bool modelInvalidate;
    private bool delaySetInstance;
    private GameObject delayInstance;
    private string delayInstanceName;

    public static string GroupContext { get; set; }

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
    public IModel Model
    {
      get => this.model.Value;
      set
      {
        this.model.Value = value;
        this.modelInvalidate = true;
      }
    }

    [Inspected]
    public IEnumerable<IModel> Models
    {
      get
      {
        return this.models.Select<Typed<IModel>, IModel>((Func<Typed<IModel>, IModel>) (o => o.Value)).Where<IModel>((Func<IModel, bool>) (o => o != null));
      }
    }

    public bool NeedSave => true;

    public override void PrepareAdded()
    {
      base.PrepareAdded();
      this.group = DynamicModelComponent.GroupContext;
      if (this.group.IsNullOrEmpty())
        this.group = "[Dynamics]";
      DynamicModelComponent.GroupContext = (string) null;
    }

    public override void OnAdded()
    {
      base.OnAdded();
      this.locationItem.OnHibernationChanged += new Action<ILocationItemComponent>(this.OnChangeHibernation);
      InstanceByRequest<EngineApplication>.Instance.OnViewEnabledEvent += new Action<bool>(this.OnViewEnabledEvent);
      this.modelInvalidate = true;
      InstanceByRequest<UpdateService>.Instance.Updater.AddUpdatable((IUpdatable) this);
    }

    public override void OnRemoved()
    {
      InstanceByRequest<UpdateService>.Instance.Updater.RemoveUpdatable((IUpdatable) this);
      InstanceByRequest<EngineApplication>.Instance.OnViewEnabledEvent -= new Action<bool>(this.OnViewEnabledEvent);
      this.locationItem.OnHibernationChanged -= new Action<ILocationItemComponent>(this.OnChangeHibernation);
      this.Clear("On remove");
      base.OnRemoved();
    }

    private void OnDispose()
    {
    }

    private void Clear(string reason)
    {
      this.DestroyCurrentGameObject();
      if (this.prefabAsset == null)
        return;
      this.assetLoader.DisposeAsset((IAsset) this.prefabAsset, new Action(this.OnDispose), reason);
      this.prefabAsset = (PrefabAsset) null;
    }

    private void OnChangeHibernation(ILocationItemComponent sender) => this.UpdateEnabled();

    private void UpdateModel()
    {
      this.Clear("Change model");
      if (!(this.Model is Engine.Source.Commons.Model model))
        return;
      this.prefabAsset = this.assetLoader.CreatePrefabAsset(model.Connection, new Func<bool, IEnumerator>(this.OnLoad));
    }

    private void UpdateEnabled()
    {
      if (this.locationItem == null || !((IEntityView) this.Owner).IsAttached)
        return;
      bool flag = !this.locationItem.IsHibernation && this.Owner.IsEnabledInHierarchy && this.IsEnabled && InstanceByRequest<EngineApplication>.Instance.ViewEnabled;
      GameObject gameObject = ((IEntityView) this.Owner).GameObject;
      if ((UnityEngine.Object) gameObject == (UnityEngine.Object) null)
      {
        Debug.LogError((object) "go == null");
      }
      else
      {
        if (Profiler.enabled)
          Profiler.BeginSample("Set Enable : " + gameObject.name);
        gameObject.SetActive(flag);
        if (!Profiler.enabled)
          return;
        Profiler.EndSample();
      }
    }

    public override void OnChangeEnabled()
    {
      base.OnChangeEnabled();
      this.UpdateEnabled();
    }

    private IEnumerator OnLoad(bool success)
    {
      if (!success)
      {
        this.Clear("Error load");
      }
      else
      {
        this.DestroyCurrentGameObject();
        if (this.prefabAsset == null)
          throw new Exception(this.Owner.GetInfo());
        if (!this.prefabAsset.IsValid)
          throw new Exception(this.Owner.GetInfo());
        this.CreateGameObject();
        yield break;
      }
    }

    private void CreateGameObject()
    {
      GameObject prefab = this.prefabAsset.Prefab;
      if (Profiler.enabled)
        Profiler.BeginSample("Instantiate : " + prefab.name);
      GameObject gameObject = UnityFactory.Instantiate(prefab, this.group);
      if (Profiler.enabled)
        Profiler.EndSample();
      this.delaySetInstance = true;
      this.delayInstance = gameObject;
      this.delayInstanceName = this.model.Value.Name;
    }

    private void DestroyCurrentGameObject()
    {
      if (this.delaySetInstance)
      {
        this.delaySetInstance = false;
        GameObject delayInstance = this.delayInstance;
        string delayInstanceName = this.delayInstanceName;
        this.delayInstance = (GameObject) null;
        this.delayInstanceName = (string) null;
        UnityEngine.Object.Destroy((UnityEngine.Object) delayInstance);
      }
      if (!((IEntityView) this.Owner).IsAttached)
        return;
      GameObject gameObject = ((IEntityView) this.Owner).GameObject;
      ((IEntityViewSetter) this.Owner).GameObject = (GameObject) null;
      UnityEngine.Object.Destroy((UnityEngine.Object) gameObject);
    }

    private void SetGameObject(GameObject go, string name)
    {
      go.name = this.Owner.Name + " : (" + name + ")";
      if ((UnityEngine.Object) go.GetComponent<EngineGameObject>() == (UnityEngine.Object) null)
        go.AddComponent<EngineGameObject>();
      if (Profiler.enabled)
        Profiler.BeginSample("Set GameObject : " + go.name);
      ((IEntityViewSetter) this.Owner).GameObject = go;
      if (Profiler.enabled)
        Profiler.EndSample();
      EntityViewUtility.SetTransformAndData(this.Owner, ((IEntityView) this.Owner).Position, ((IEntityView) this.Owner).Rotation, ((Entity) this.Owner).IsPlayer);
      this.UpdateEnabled();
    }

    private void OnViewEnabledEvent(bool enabled)
    {
      if (this.IsDisposed)
        return;
      this.UpdateEnabled();
    }

    [Cofe.Serializations.Data.OnLoaded]
    private void OnLoaded() => this.UpdateEnabled();

    public void ComputeUpdate()
    {
      bool flag = !this.locationItem.IsHibernation && this.Owner.IsEnabledInHierarchy && this.IsEnabled && InstanceByRequest<EngineApplication>.Instance.ViewEnabled;
      if (this.modelInvalidate && (flag || ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.CreateDisabledPrefabs))
      {
        this.modelInvalidate = false;
        this.UpdateModel();
      }
      if (!flag && ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.DestroyDisabledPrefabs && ((IEntityView) this.Owner).IsAttached && !ServiceCache.OptimizationService.FrameHasSpike)
      {
        ServiceCache.OptimizationService.FrameHasSpike = true;
        GameObject gameObject = ((IEntityView) this.Owner).GameObject;
        ((IEntityViewSetter) this.Owner).GameObject = (GameObject) null;
        UnityEngine.Object.Destroy((UnityEngine.Object) gameObject);
        this.modelInvalidate = true;
      }
      if (this.delaySetInstance && !ServiceCache.OptimizationService.FrameHasSpike)
      {
        ServiceCache.OptimizationService.FrameHasSpike = true;
        this.delaySetInstance = false;
        GameObject delayInstance = this.delayInstance;
        string delayInstanceName = this.delayInstanceName;
        this.delayInstance = (GameObject) null;
        this.delayInstanceName = (string) null;
        this.SetGameObject(delayInstance, delayInstanceName);
      }
      if (!flag || !((IEntityView) this.Owner).IsAttached)
        return;
      EntityViewUtility.FromTransformToData(this.Owner);
    }
  }
}

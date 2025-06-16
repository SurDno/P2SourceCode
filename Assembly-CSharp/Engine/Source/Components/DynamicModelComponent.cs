using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cofe.Serializations.Data;
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
using UnityEngine;
using UnityEngine.Profiling;
using Object = UnityEngine.Object;

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
    [StateSaveProxy]
    [StateLoadProxy]
    [DataReadProxy]
    [DataWriteProxy]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    [CopyableProxy]
    protected bool isEnabled = true;
    [DataReadProxy]
    [DataWriteProxy]
    [StateSaveProxy]
    [StateLoadProxy]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    [CopyableProxy]
    protected Typed<IModel> model;
    [DataReadProxy]
    [DataWriteProxy]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    [CopyableProxy()]
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
      get => isEnabled;
      set
      {
        isEnabled = value;
        OnChangeEnabled();
      }
    }

    [Inspected]
    public IModel Model
    {
      get => model.Value;
      set
      {
        model.Value = value;
        modelInvalidate = true;
      }
    }

    [Inspected]
    public IEnumerable<IModel> Models
    {
      get
      {
        return models.Select(o => o.Value).Where(o => o != null);
      }
    }

    public bool NeedSave => true;

    public override void PrepareAdded()
    {
      base.PrepareAdded();
      group = GroupContext;
      if (group.IsNullOrEmpty())
        group = "[Dynamics]";
      GroupContext = null;
    }

    public override void OnAdded()
    {
      base.OnAdded();
      locationItem.OnHibernationChanged += OnChangeHibernation;
      InstanceByRequest<EngineApplication>.Instance.OnViewEnabledEvent += OnViewEnabledEvent;
      modelInvalidate = true;
      InstanceByRequest<UpdateService>.Instance.Updater.AddUpdatable(this);
    }

    public override void OnRemoved()
    {
      InstanceByRequest<UpdateService>.Instance.Updater.RemoveUpdatable(this);
      InstanceByRequest<EngineApplication>.Instance.OnViewEnabledEvent -= OnViewEnabledEvent;
      locationItem.OnHibernationChanged -= OnChangeHibernation;
      Clear("On remove");
      base.OnRemoved();
    }

    private void OnDispose()
    {
    }

    private void Clear(string reason)
    {
      DestroyCurrentGameObject();
      if (prefabAsset == null)
        return;
      assetLoader.DisposeAsset(prefabAsset, OnDispose, reason);
      prefabAsset = null;
    }

    private void OnChangeHibernation(ILocationItemComponent sender) => UpdateEnabled();

    private void UpdateModel()
    {
      Clear("Change model");
      if (!(Model is Model model))
        return;
      prefabAsset = assetLoader.CreatePrefabAsset(model.Connection, OnLoad);
    }

    private void UpdateEnabled()
    {
      if (locationItem == null || !((IEntityView) Owner).IsAttached)
        return;
      bool flag = !locationItem.IsHibernation && Owner.IsEnabledInHierarchy && IsEnabled && InstanceByRequest<EngineApplication>.Instance.ViewEnabled;
      GameObject gameObject = ((IEntityView) Owner).GameObject;
      if (gameObject == null)
      {
        Debug.LogError("go == null");
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
      UpdateEnabled();
    }

    private IEnumerator OnLoad(bool success)
    {
      if (!success)
      {
        Clear("Error load");
      }
      else
      {
        DestroyCurrentGameObject();
        if (prefabAsset == null)
          throw new Exception(Owner.GetInfo());
        if (!prefabAsset.IsValid)
          throw new Exception(Owner.GetInfo());
        CreateGameObject();
        yield break;
      }
    }

    private void CreateGameObject()
    {
      GameObject prefab = prefabAsset.Prefab;
      if (Profiler.enabled)
        Profiler.BeginSample("Instantiate : " + prefab.name);
      GameObject gameObject = UnityFactory.Instantiate(prefab, group);
      if (Profiler.enabled)
        Profiler.EndSample();
      delaySetInstance = true;
      delayInstance = gameObject;
      delayInstanceName = model.Value.Name;
    }

    private void DestroyCurrentGameObject()
    {
      if (delaySetInstance)
      {
        delaySetInstance = false;
        GameObject delayInstance = this.delayInstance;
        string delayInstanceName = this.delayInstanceName;
        this.delayInstance = null;
        this.delayInstanceName = null;
        Object.Destroy(delayInstance);
      }
      if (!((IEntityView) Owner).IsAttached)
        return;
      GameObject gameObject = ((IEntityView) Owner).GameObject;
      ((IEntityViewSetter) Owner).GameObject = null;
      Object.Destroy(gameObject);
    }

    private void SetGameObject(GameObject go, string name)
    {
      go.name = Owner.Name + " : (" + name + ")";
      if (go.GetComponent<EngineGameObject>() == null)
        go.AddComponent<EngineGameObject>();
      if (Profiler.enabled)
        Profiler.BeginSample("Set GameObject : " + go.name);
      ((IEntityViewSetter) Owner).GameObject = go;
      if (Profiler.enabled)
        Profiler.EndSample();
      EntityViewUtility.SetTransformAndData(Owner, ((IEntityView) Owner).Position, ((IEntityView) Owner).Rotation, ((Entity) Owner).IsPlayer);
      UpdateEnabled();
    }

    private void OnViewEnabledEvent(bool enabled)
    {
      if (IsDisposed)
        return;
      UpdateEnabled();
    }

    [OnLoaded]
    private void OnLoaded() => UpdateEnabled();

    public void ComputeUpdate()
    {
      bool flag = !locationItem.IsHibernation && Owner.IsEnabledInHierarchy && IsEnabled && InstanceByRequest<EngineApplication>.Instance.ViewEnabled;
      if (modelInvalidate && (flag || ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.CreateDisabledPrefabs))
      {
        modelInvalidate = false;
        UpdateModel();
      }
      if (!flag && ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.DestroyDisabledPrefabs && ((IEntityView) Owner).IsAttached && !ServiceCache.OptimizationService.FrameHasSpike)
      {
        ServiceCache.OptimizationService.FrameHasSpike = true;
        GameObject gameObject = ((IEntityView) Owner).GameObject;
        ((IEntityViewSetter) Owner).GameObject = null;
        Object.Destroy(gameObject);
        modelInvalidate = true;
      }
      if (delaySetInstance && !ServiceCache.OptimizationService.FrameHasSpike)
      {
        ServiceCache.OptimizationService.FrameHasSpike = true;
        delaySetInstance = false;
        GameObject delayInstance = this.delayInstance;
        string delayInstanceName = this.delayInstanceName;
        this.delayInstance = null;
        this.delayInstanceName = null;
        SetGameObject(delayInstance, delayInstanceName);
      }
      if (!flag || !((IEntityView) Owner).IsAttached)
        return;
      EntityViewUtility.FromTransformToData(Owner);
    }
  }
}

using System;
using System.Collections.Generic;
using Engine.Common;
using Engine.Source.Commons;
using Engine.Source.Settings.External;
using FlowCanvas;
using Inspectors;
using UnityEngine;
using UnityEngine.Profiling;

namespace ParadoxNotion.Services
{
  public class BlueprintManager : MonoBehaviour, IUpdateItem<Graph>, IUpdatable
  {
    private static bool isQuiting;
    [NonSerialized]
    public List<Graph> graphs = [];
    [Inspected]
    private ReduceUpdateProxy<Graph> updater;
    private static BlueprintManager _current;

    public static UpdateMode updateMode
    {
      get => current.enabled ? UpdateMode.Auto : UpdateMode.Manual;
      set => current.enabled = value == UpdateMode.Auto;
    }

    public event Action onLateUpdate;

    public event Action onFixedUpdate;

    public event Action onApplicationQuit;

    public event Action<bool> onApplicationPause;

    public static BlueprintManager current
    {
      get
      {
        if (_current == null && !isQuiting)
        {
          _current = FindObjectOfType<BlueprintManager>();
          if (_current == null)
            _current = UnityFactory.GetOrCreateGroup("[Blueprints]").AddComponent<BlueprintManager>();
        }
        return _current;
      }
    }

    private void OnApplicationQuit()
    {
      isQuiting = true;
      Action onApplicationQuit = this.onApplicationQuit;
      if (onApplicationQuit == null)
        return;
      onApplicationQuit();
    }

    private void OnApplicationPause(bool isPause)
    {
      Action<bool> applicationPause = onApplicationPause;
      if (applicationPause == null)
        return;
      applicationPause(isPause);
    }

    private void Awake()
    {
      updater = new ReduceUpdateProxy<Graph>(graphs, this, ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.BlueprintUpdateDelay);
      if (_current != null && _current != this)
      {
        DestroyImmediate(gameObject);
      }
      else
      {
        InstanceByRequest<UpdateService>.Instance.BlueprintUpdater.AddUpdatable(this);
        _current = this;
      }
    }

    private void OnDestroy()
    {
      _current = null;
      InstanceByRequest<UpdateService>.Instance.BlueprintUpdater.RemoveUpdatable(this);
    }

    private void LateUpdate()
    {
      Action onLateUpdate = this.onLateUpdate;
      if (onLateUpdate == null)
        return;
      onLateUpdate();
    }

    private void FixedUpdate()
    {
      Action onFixedUpdate = this.onFixedUpdate;
      if (onFixedUpdate == null)
        return;
      onFixedUpdate();
    }

    public void ComputeUpdateItem(Graph item)
    {
      if (Profiler.enabled)
        Profiler.BeginSample(item.agentName);
      item.UpdateGraph();
      if (!Profiler.enabled)
        return;
      Profiler.EndSample();
    }

    public void ComputeUpdate()
    {
      if (ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.DisableBlueprints)
        return;
      updater.Update();
    }

    public enum UpdateMode
    {
      Auto,
      Manual,
    }
  }
}

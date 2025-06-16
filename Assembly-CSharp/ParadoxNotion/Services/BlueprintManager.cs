// Decompiled with JetBrains decompiler
// Type: ParadoxNotion.Services.BlueprintManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using Engine.Source.Commons;
using Engine.Source.Settings.External;
using FlowCanvas;
using Inspectors;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

#nullable disable
namespace ParadoxNotion.Services
{
  public class BlueprintManager : MonoBehaviour, IUpdateItem<Graph>, IUpdatable
  {
    private static bool isQuiting;
    [NonSerialized]
    public List<Graph> graphs = new List<Graph>();
    [Inspected]
    private ReduceUpdateProxy<Graph> updater;
    private static BlueprintManager _current;

    public static BlueprintManager.UpdateMode updateMode
    {
      get
      {
        return BlueprintManager.current.enabled ? BlueprintManager.UpdateMode.Auto : BlueprintManager.UpdateMode.Manual;
      }
      set => BlueprintManager.current.enabled = value == BlueprintManager.UpdateMode.Auto;
    }

    public event Action onLateUpdate;

    public event Action onFixedUpdate;

    public event Action onApplicationQuit;

    public event Action<bool> onApplicationPause;

    public static BlueprintManager current
    {
      get
      {
        if ((UnityEngine.Object) BlueprintManager._current == (UnityEngine.Object) null && !BlueprintManager.isQuiting)
        {
          BlueprintManager._current = UnityEngine.Object.FindObjectOfType<BlueprintManager>();
          if ((UnityEngine.Object) BlueprintManager._current == (UnityEngine.Object) null)
            BlueprintManager._current = UnityFactory.GetOrCreateGroup("[Blueprints]").AddComponent<BlueprintManager>();
        }
        return BlueprintManager._current;
      }
    }

    private void OnApplicationQuit()
    {
      BlueprintManager.isQuiting = true;
      Action onApplicationQuit = this.onApplicationQuit;
      if (onApplicationQuit == null)
        return;
      onApplicationQuit();
    }

    private void OnApplicationPause(bool isPause)
    {
      Action<bool> applicationPause = this.onApplicationPause;
      if (applicationPause == null)
        return;
      applicationPause(isPause);
    }

    private void Awake()
    {
      this.updater = new ReduceUpdateProxy<Graph>(this.graphs, (IUpdateItem<Graph>) this, ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.BlueprintUpdateDelay);
      if ((UnityEngine.Object) BlueprintManager._current != (UnityEngine.Object) null && (UnityEngine.Object) BlueprintManager._current != (UnityEngine.Object) this)
      {
        UnityEngine.Object.DestroyImmediate((UnityEngine.Object) this.gameObject);
      }
      else
      {
        InstanceByRequest<UpdateService>.Instance.BlueprintUpdater.AddUpdatable((IUpdatable) this);
        BlueprintManager._current = this;
      }
    }

    private void OnDestroy()
    {
      BlueprintManager._current = (BlueprintManager) null;
      InstanceByRequest<UpdateService>.Instance.BlueprintUpdater.RemoveUpdatable((IUpdatable) this);
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
      this.updater.Update();
    }

    public enum UpdateMode
    {
      Auto,
      Manual,
    }
  }
}

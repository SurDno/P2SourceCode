// Decompiled with JetBrains decompiler
// Type: Engine.Services.Engine.Assets.AssetLoader
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using AssetDatabases;
using Cofe.Utility;
using Engine.Common;
using Engine.Common.Services;
using Engine.Source.Commons;
using Engine.Source.Connections;
using Engine.Source.Services;
using Engine.Source.Services.Assets;
using Engine.Source.Settings.External;
using Inspectors;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Profiling;

#nullable disable
namespace Engine.Services.Engine.Assets
{
  [RuntimeService(new System.Type[] {typeof (AssetLoader)})]
  public class AssetLoader : IUpdatable, IInitialisable
  {
    [Inspected]
    private List<AssetState> assets = new List<AssetState>();
    private bool initialise;

    public bool IsEmpty => this.assets.Count == 0;

    public void Initialise()
    {
      this.initialise = true;
      InstanceByRequest<UpdateService>.Instance.Updater.AddUpdatable((IUpdatable) this);
    }

    public void Terminate()
    {
      InstanceByRequest<UpdateService>.Instance.Updater.RemoveUpdatable((IUpdatable) this);
      this.initialise = false;
    }

    public void ComputeUpdate()
    {
      if (!this.initialise)
        throw new Exception(this.GetType().Name + "." + MethodBase.GetCurrentMethod().Name);
      if (ServiceCache.OptimizationService.FrameHasSpike)
        return;
      for (int index1 = 0; index1 < this.assets.Count; ++index1)
      {
        AssetState asset = this.assets[index1];
        if (asset.NeedDispose)
        {
          asset.Asset.Dispose(asset.DisposeReason);
          asset.NeedDispose = false;
        }
        asset.Asset.Update();
        if (asset.Asset.IsError)
        {
          this.assets.RemoveAt(index1);
          Func<bool, IEnumerator> onLoad = asset.OnLoad;
          IEnumerator enumerator = onLoad != null ? onLoad(false) : (IEnumerator) null;
          do
            ;
          while (enumerator.MoveNext());
          Action onDispose = asset.OnDispose;
          if (onDispose != null)
            onDispose();
          ServiceCache.OptimizationService.FrameHasSpike = true;
          break;
        }
        if (asset.Asset.IsReadyToDispose)
        {
          this.assets.RemoveAt(index1);
          Action onDispose = asset.OnDispose;
          if (onDispose != null)
            onDispose();
          ServiceCache.OptimizationService.FrameHasSpike = true;
          break;
        }
        if (!asset.Asset.IsDisposed && asset.Asset.IsLoaded)
        {
          Profiler.BeginSample("AssetLoader Processing " + asset.Asset.Path);
          if (asset.Processor == null && asset.OnLoad != null)
            asset.Processor = asset.OnLoad(true);
          if (asset.Processor == null)
          {
            this.assets.RemoveAt(index1);
          }
          else
          {
            int num = ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.SmoothHierarchyMapping ? 12 : int.MaxValue;
            for (int index2 = 0; index2 < num; ++index2)
            {
              if (!asset.Processor.MoveNext())
              {
                asset.Processor = (IEnumerator) null;
                this.assets.RemoveAt(index1);
                ServiceCache.OptimizationService.FrameHasSpike = true;
                break;
              }
            }
          }
          Profiler.EndSample();
          break;
        }
      }
    }

    public PrefabAsset CreatePrefabAsset(
      UnityAsset<GameObject> resource,
      Func<bool, IEnumerator> load)
    {
      if (!this.initialise)
        throw new Exception(this.GetType().Name + "." + MethodBase.GetCurrentMethod().Name);
      string path = AssetDatabaseService.Instance.GetPath(resource.Id);
      if (path.IsNullOrEmpty())
        return (PrefabAsset) null;
      PrefabAsset prefabAsset = new PrefabAsset(path);
      this.assets.Add(new AssetState()
      {
        Asset = (IAsset) prefabAsset,
        OnLoad = load
      });
      return prefabAsset;
    }

    public SceneAsset CreateSceneAsset(
      IScene reference,
      Func<bool, IEnumerator> load,
      string context)
    {
      if (!this.initialise)
        throw new Exception(this.GetType().Name + "." + MethodBase.GetCurrentMethod().Name);
      if (reference == null)
        return (SceneAsset) null;
      SceneAsset sceneAsset = new SceneAsset(reference, context);
      this.assets.Add(new AssetState()
      {
        Asset = (IAsset) sceneAsset,
        OnLoad = load
      });
      return sceneAsset;
    }

    public void DisposeAsset(IAsset asset, Action dispose, string reason)
    {
      if (!this.initialise)
        throw new Exception(this.GetType().Name + "." + MethodBase.GetCurrentMethod().Name);
      int num = -1;
      for (int index = 0; index < this.assets.Count; ++index)
      {
        AssetState asset1 = this.assets[index];
        if (asset1.Asset == asset)
        {
          asset1.OnDispose = dispose;
          if (asset1.Processor != null)
          {
            AssetState assetState = new AssetState()
            {
              Asset = asset
            };
            assetState.OnDispose = dispose;
            assetState.NeedDispose = true;
            assetState.DisposeReason = reason;
            this.assets.Add(assetState);
            return;
          }
          num = index;
          break;
        }
      }
      if (num == -1)
      {
        AssetState assetState = new AssetState()
        {
          Asset = asset
        };
        int count = this.assets.Count;
        assetState.OnDispose = dispose;
        this.assets.Add(assetState);
      }
      asset.Dispose(reason);
    }
  }
}

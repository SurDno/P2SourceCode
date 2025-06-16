using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
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

namespace Engine.Services.Engine.Assets
{
  [RuntimeService(typeof (AssetLoader))]
  public class AssetLoader : IUpdatable, IInitialisable
  {
    [Inspected]
    private List<AssetState> assets = new List<AssetState>();
    private bool initialise;

    public bool IsEmpty => assets.Count == 0;

    public void Initialise()
    {
      initialise = true;
      InstanceByRequest<UpdateService>.Instance.Updater.AddUpdatable(this);
    }

    public void Terminate()
    {
      InstanceByRequest<UpdateService>.Instance.Updater.RemoveUpdatable(this);
      initialise = false;
    }

    public void ComputeUpdate()
    {
      if (!initialise)
        throw new Exception(GetType().Name + "." + MethodBase.GetCurrentMethod().Name);
      if (ServiceCache.OptimizationService.FrameHasSpike)
        return;
      for (int index1 = 0; index1 < assets.Count; ++index1)
      {
        AssetState asset = assets[index1];
        if (asset.NeedDispose)
        {
          asset.Asset.Dispose(asset.DisposeReason);
          asset.NeedDispose = false;
        }
        asset.Asset.Update();
        if (asset.Asset.IsError)
        {
          assets.RemoveAt(index1);
          Func<bool, IEnumerator> onLoad = asset.OnLoad;
          IEnumerator enumerator = onLoad != null ? onLoad(false) : null;
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
          assets.RemoveAt(index1);
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
            assets.RemoveAt(index1);
          }
          else
          {
            int num = ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.SmoothHierarchyMapping ? 12 : int.MaxValue;
            for (int index2 = 0; index2 < num; ++index2)
            {
              if (!asset.Processor.MoveNext())
              {
                asset.Processor = null;
                assets.RemoveAt(index1);
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
      if (!initialise)
        throw new Exception(GetType().Name + "." + MethodBase.GetCurrentMethod().Name);
      string path = AssetDatabaseService.Instance.GetPath(resource.Id);
      if (path.IsNullOrEmpty())
        return null;
      PrefabAsset prefabAsset = new PrefabAsset(path);
      assets.Add(new AssetState {
        Asset = prefabAsset,
        OnLoad = load
      });
      return prefabAsset;
    }

    public SceneAsset CreateSceneAsset(
      IScene reference,
      Func<bool, IEnumerator> load,
      string context)
    {
      if (!initialise)
        throw new Exception(GetType().Name + "." + MethodBase.GetCurrentMethod().Name);
      if (reference == null)
        return null;
      SceneAsset sceneAsset = new SceneAsset(reference, context);
      assets.Add(new AssetState {
        Asset = sceneAsset,
        OnLoad = load
      });
      return sceneAsset;
    }

    public void DisposeAsset(IAsset asset, Action dispose, string reason)
    {
      if (!initialise)
        throw new Exception(GetType().Name + "." + MethodBase.GetCurrentMethod().Name);
      int num = -1;
      for (int index = 0; index < assets.Count; ++index)
      {
        AssetState asset1 = assets[index];
        if (asset1.Asset == asset)
        {
          asset1.OnDispose = dispose;
          if (asset1.Processor != null)
          {
            AssetState assetState = new AssetState {
              Asset = asset
            };
            assetState.OnDispose = dispose;
            assetState.NeedDispose = true;
            assetState.DisposeReason = reason;
            assets.Add(assetState);
            return;
          }
          num = index;
          break;
        }
      }
      if (num == -1)
      {
        AssetState assetState = new AssetState {
          Asset = asset
        };
        int count = assets.Count;
        assetState.OnDispose = dispose;
        assets.Add(assetState);
      }
      asset.Dispose(reason);
    }
  }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using AssetDatabases;
using Engine.Common;
using Engine.Common.Services;
using Engine.Source.Commons;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Engine.Source.Services.Templates
{
  public class RuntimeTemplateLoader : ITemplateLoader
  {
    public int AsyncCount => AssetDatabaseService.Instance.GetAllAssetPathsCount();

    public IEnumerator Load(Dictionary<Guid, IObject> items, Dictionary<Guid, string> names)
    {
      Stopwatch sw = new Stopwatch();
      sw.Restart();
      InitialiseEngineProgressService progressService = ServiceLocator.GetService<InitialiseEngineProgressService>();
      int created = 0;
      float delay = 1f;
      float time = Time.realtimeSinceStartup;
      IFactory factory = ServiceLocator.GetService<IFactory>();
      IEnumerable<string> assets = AssetDatabaseService.Instance.GetAllAssetPaths();
      foreach (string asset in assets)
      {
        ++progressService.Progress;
        if (time + (double) delay < Time.realtimeSinceStartup)
        {
          time = Time.realtimeSinceStartup;
          progressService.Update(nameof (RuntimeTemplateLoader), asset);
          yield return null;
        }
        IObject obj = null;
        if (asset.EndsWith(".bytes"))
          obj = TemplateLoaderUtility.LoadObject(asset);
        else if (asset.EndsWith("_AI.asset"))
        {
          Guid id = AssetDatabaseService.Instance.GetId(asset);
          if (!(id == Guid.Empty))
          {
            obj = factory.Create<BehaviorObject>(id);
            ++created;
            id = new Guid();
          }
          else
            continue;
        }
        else if (asset.EndsWith("_Blueprint.prefab"))
        {
          Guid id = AssetDatabaseService.Instance.GetId(asset);
          if (!(id == Guid.Empty))
          {
            obj = factory.Create<BlueprintObject>(id);
            ++created;
            id = new Guid();
          }
          else
            continue;
        }
        else
          continue;
        obj.Name = Path.GetFileNameWithoutExtension(asset);
        TemplateLoaderUtility.AddTemplateImpl(obj, asset, items, names);
        obj = null;
      }
      sw.Stop();
      Debug.Log(ObjectInfoUtility.GetStream().Append(nameof (RuntimeTemplateLoader)).Append(" : ").Append(nameof (Load)).Append(" , elapsed : ").Append(sw.Elapsed).Append(" , created : ").Append(created));
    }
  }
}

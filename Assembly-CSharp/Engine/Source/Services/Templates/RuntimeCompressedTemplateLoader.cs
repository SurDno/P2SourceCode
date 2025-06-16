using AssetDatabases;
using Engine.Common;
using Engine.Common.Services;
using Engine.Common.Threads;
using Engine.Source.Commons;
using Engine.Source.Settings.External;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading;
using System.Xml;

namespace Engine.Source.Services.Templates
{
  public class RuntimeCompressedTemplateLoader : ITemplateLoader
  {
    public int AsyncCount => 0;

    public IEnumerator Load(Dictionary<Guid, IObject> items, Dictionary<Guid, string> names)
    {
      Stopwatch sw = new Stopwatch();
      sw.Restart();
      InitialiseEngineProgressService progressService = ServiceLocator.GetService<InitialiseEngineProgressService>();
      progressService.Update(nameof (RuntimeCompressedTemplateLoader), "Prepare");
      ThreadState<string, Dictionary<Guid, IObject>> state = RuntimeCompressedTemplateLoader.BeginThreads(items);
      int created = 0;
      IFactory factory = ServiceLocator.GetService<IFactory>();
      IEnumerable<string> assets = AssetDatabaseService.Instance.GetAllAssetPaths();
      foreach (string asset in assets)
      {
        IObject obj = (IObject) null;
        if (asset.EndsWith("_AI.asset"))
        {
          Guid id = AssetDatabaseService.Instance.GetId(asset);
          if (!(id == Guid.Empty))
          {
            obj = (IObject) factory.Create<BehaviorObject>(id);
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
            obj = (IObject) factory.Create<BlueprintObject>(id);
            ++created;
            id = new Guid();
          }
          else
            continue;
        }
        else
          continue;
        obj.Name = Path.GetFileNameWithoutExtension(asset);
        RuntimeCompressedTemplateLoader.AddTemplateImpl(obj, items);
        obj = (IObject) null;
      }
      RuntimeCompressedTemplateLoader.WaitThreads(state);
      sw.Stop();
      UnityEngine.Debug.Log((object) ObjectInfoUtility.GetStream().Append(nameof (RuntimeCompressedTemplateLoader)).Append(" : ").Append(nameof (Load)).Append(" , elapsed : ").Append((object) sw.Elapsed).Append(" , created : ").Append(created));
      yield break;
    }

    private static ThreadState<string, Dictionary<Guid, IObject>> BeginThreads(
      Dictionary<Guid, IObject> items)
    {
      int threadCount = ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.ThreadCount;
      return ThreadPoolUtility.BeginCompute<string, Dictionary<Guid, IObject>>(new Action<string, ThreadState<string, Dictionary<Guid, IObject>>>(RuntimeCompressedTemplateLoader.LoadTemplateFile), (IEnumerable<string>) Directory.GetFiles(PlatformUtility.GetPath("Data/Templates/"), "*.gz"), threadCount, items);
    }

    private static void WaitThreads(
      ThreadState<string, Dictionary<Guid, IObject>> state)
    {
      ThreadPoolUtility.Worker<string, Dictionary<Guid, IObject>>(state);
      ThreadPoolUtility.Wait<string, Dictionary<Guid, IObject>>(state);
    }

    private static void LoadTemplateFile(
      string fileName,
      ThreadState<string, Dictionary<Guid, IObject>> state)
    {
      Stopwatch stopwatch = new Stopwatch();
      stopwatch.Restart();
      using (FileStream fileStream = File.OpenRead(fileName))
      {
        using (GZipStream inStream = new GZipStream((Stream) fileStream, CompressionMode.Decompress))
        {
          XmlDocument xmlDocument = new XmlDocument();
          xmlDocument.Load((Stream) inStream);
          foreach (XmlNode childNode in xmlDocument.DocumentElement.ChildNodes)
          {
            IObject template = SerializeUtility.Deserialize<IObject>(childNode, fileName);
            if (template != null)
            {
              template.Name = childNode["Name"].InnerText;
              RuntimeCompressedTemplateLoader.AddTemplateImpl(template, state.Context);
            }
            else
              UnityEngine.Debug.LogError((object) ("Error load template from : " + fileName));
          }
        }
      }
      stopwatch.Stop();
      UnityEngine.Debug.Log((object) new StringBuilder().Append(nameof (RuntimeCompressedTemplateLoader)).Append(" : ").Append(nameof (LoadTemplateFile)).Append(" , file name : ").Append(fileName).Append(" , elapsed : ").Append((object) stopwatch.Elapsed).Append(" , thread : ").Append(Thread.CurrentThread.ManagedThreadId));
    }

    private static void AddTemplateImpl(IObject template, Dictionary<Guid, IObject> items)
    {
      if (template is ITemplateSetter templateSetter)
        templateSetter.IsTemplate = true;
      lock (items)
        items.Add(template.Id, template);
    }
  }
}

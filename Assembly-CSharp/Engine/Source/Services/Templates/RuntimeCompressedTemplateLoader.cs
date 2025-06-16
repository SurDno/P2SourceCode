using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading;
using System.Xml;
using AssetDatabases;
using Engine.Common;
using Engine.Common.Services;
using Engine.Common.Threads;
using Engine.Source.Commons;
using Engine.Source.Settings.External;
using Debug = UnityEngine.Debug;

namespace Engine.Source.Services.Templates;

public class RuntimeCompressedTemplateLoader : ITemplateLoader {
	public int AsyncCount => 0;

	public IEnumerator Load(Dictionary<Guid, IObject> items, Dictionary<Guid, string> names) {
		var sw = new Stopwatch();
		sw.Restart();
		var progressService = ServiceLocator.GetService<InitialiseEngineProgressService>();
		progressService.Update(nameof(RuntimeCompressedTemplateLoader), "Prepare");
		var state = BeginThreads(items);
		var created = 0;
		var factory = ServiceLocator.GetService<IFactory>();
		var assets = AssetDatabaseService.Instance.GetAllAssetPaths();
		foreach (var asset in assets) {
			IObject obj = null;
			if (asset.EndsWith("_AI.asset")) {
				var id = AssetDatabaseService.Instance.GetId(asset);
				if (!(id == Guid.Empty)) {
					obj = factory.Create<BehaviorObject>(id);
					++created;
					id = new Guid();
				} else
					continue;
			} else if (asset.EndsWith("_Blueprint.prefab")) {
				var id = AssetDatabaseService.Instance.GetId(asset);
				if (!(id == Guid.Empty)) {
					obj = factory.Create<BlueprintObject>(id);
					++created;
					id = new Guid();
				} else
					continue;
			} else
				continue;

			obj.Name = Path.GetFileNameWithoutExtension(asset);
			AddTemplateImpl(obj, items);
			obj = null;
		}

		WaitThreads(state);
		sw.Stop();
		Debug.Log(ObjectInfoUtility.GetStream().Append(nameof(RuntimeCompressedTemplateLoader)).Append(" : ")
			.Append(nameof(Load)).Append(" , elapsed : ").Append(sw.Elapsed).Append(" , created : ").Append(created));
		yield break;
	}

	private static ThreadState<string, Dictionary<Guid, IObject>> BeginThreads(
		Dictionary<Guid, IObject> items) {
		var threadCount = ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.ThreadCount;
		return ThreadPoolUtility.BeginCompute(LoadTemplateFile,
			Directory.GetFiles(PlatformUtility.GetPath("Data/Templates/"), "*.gz"), threadCount, items);
	}

	private static void WaitThreads(
		ThreadState<string, Dictionary<Guid, IObject>> state) {
		ThreadPoolUtility.Worker(state);
		ThreadPoolUtility.Wait(state);
	}

	private static void LoadTemplateFile(
		string fileName,
		ThreadState<string, Dictionary<Guid, IObject>> state) {
		var stopwatch = new Stopwatch();
		stopwatch.Restart();
		using (var fileStream = File.OpenRead(fileName)) {
			using (var inStream = new GZipStream(fileStream, CompressionMode.Decompress)) {
				var xmlDocument = new XmlDocument();
				xmlDocument.Load(inStream);
				foreach (XmlNode childNode in xmlDocument.DocumentElement.ChildNodes) {
					var template = SerializeUtility.Deserialize<IObject>(childNode, fileName);
					if (template != null) {
						template.Name = childNode["Name"].InnerText;
						AddTemplateImpl(template, state.Context);
					} else
						Debug.LogError("Error load template from : " + fileName);
				}
			}
		}

		stopwatch.Stop();
		Debug.Log(new StringBuilder().Append(nameof(RuntimeCompressedTemplateLoader)).Append(" : ")
			.Append(nameof(LoadTemplateFile)).Append(" , file name : ").Append(fileName).Append(" , elapsed : ")
			.Append(stopwatch.Elapsed).Append(" , thread : ").Append(Thread.CurrentThread.ManagedThreadId));
	}

	private static void AddTemplateImpl(IObject template, Dictionary<Guid, IObject> items) {
		if (template is ITemplateSetter templateSetter)
			templateSetter.IsTemplate = true;
		lock (items) {
			items.Add(template.Id, template);
		}
	}
}
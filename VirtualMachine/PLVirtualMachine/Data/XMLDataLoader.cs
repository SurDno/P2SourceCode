using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Xml;
using Cofe.Loggers;
using Cofe.Serializations.Converters;
using Engine.Common.Comparers;
using Engine.Common.Threads;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Objects;
using VirtualMachine.Common;
using VirtualMachine.Data;

namespace PLVirtualMachine.Data;

public class XMLDataLoader : IStaticDataContainer, IDataCreator {
	private GameDataInfo currentData;
	private static object sync = new();
	public static HashSet<string> Logs = new();

	private XMLDataLoader() {
		StaticDataContainer = this;
	}

	public static XMLDataLoader Instance { get; private set; } = new();

	public override IGameRoot GameRoot {
		get {
			var currentData = this.currentData;
			return currentData == null ? null : (IGameRoot)currentData.Root;
		}
	}

	public IEnumerator LoadDataFromXML(string dataFolderName, int threadCount, int dataCapacity) {
		Clear();
		currentData = new GameDataInfo {
			Name = dataFolderName,
			Objects = new Dictionary<ulong, IObject>(dataCapacity, UlongComparer.Instance)
		};
		yield return null;
		if (threadCount == 0) {
			var state = new ThreadState<KeyValuePair<string, Type>, string> {
				Context = dataFolderName
			};
			foreach (var keyValuePair in DataFactoryAttribute.Items)
				ComputeDataType(keyValuePair, state);
		} else
			ThreadPoolUtility.Compute(ComputeDataType, DataFactoryAttribute.Items, threadCount, dataFolderName);

		if (currentData.Root == null)
			Logger.AddError("Root not found");
		yield return null;
		yield return AfterLoadObjects();
		if (dataCapacity < currentData.Objects.Count)
			Logger.AddError("Wrong capacity");
		Logger.AddInfo("Load data complete, name : " + dataFolderName + " , count : " + currentData.Objects.Count +
		               " , capacity : " + dataCapacity);
	}

	private IEnumerator AfterLoadObjects() {
		currentData.Root.OnAfterLoad();
		yield return null;
		foreach (var keyValuePair in currentData.Objects) {
			var @object = keyValuePair.Value;
			if (@object != currentData.Root && @object is IOnAfterLoaded onAfterLoaded)
				onAfterLoaded.OnAfterLoad();
			if (DelayTimer.Check)
				yield return null;
		}

		foreach (var keyValuePair in currentData.Objects) {
			var @object = keyValuePair.Value;
			if (@object != currentData.Root && @object is IOnAfterLoaded onAfterLoaded)
				onAfterLoaded.OnPostLoad();
			if (DelayTimer.Check)
				yield return null;
		}
	}

	private void ComputeDataType(
		KeyValuePair<string, Type> item,
		ThreadState<KeyValuePair<string, Type>, string> state) {
		var str1 = state.Context + "/" + item.Key + ".xml.gz";
		if (File.Exists(str1)) {
			IObject @object = null;
			LoadDataCompress(str1, item.Key, item.Value, ref @object);
			if (!(item.Value == typeof(VMGameRoot)))
				return;
			currentData.Root = (VMGameRoot)@object;
		} else {
			var str2 = state.Context + "/" + item.Key + ".xml";
			if (!File.Exists(str2))
				return;
			IObject @object = null;
			LoadData(str2, item.Key, item.Value, ref @object);
			if (!(item.Value == typeof(VMGameRoot)))
				return;
			currentData.Root = (VMGameRoot)@object;
		}
	}

	private void LoadDataCompress(string fileName, string typeName, Type type, ref IObject item) {
		var stopwatch = new Stopwatch();
		stopwatch.Start();
		using (var fileStream = File.OpenRead(fileName)) {
			using (var input = new GZipStream(fileStream, CompressionMode.Decompress)) {
				using (var xml = XmlReader.Create(input)) {
					while (xml.Read())
						if (xml.NodeType == XmlNodeType.Element)
							LoadXmlDataObjectsFromReader(xml, type, ref item, typeName);
				}
			}
		}

		stopwatch.Stop();
		Logger.AddInfo(" -  - [LoadDataFromXML] LoadObjects, name : " + Path.GetFileNameWithoutExtension(fileName) +
		               " , elapsed : " + stopwatch.Elapsed + " , thread : " + Thread.CurrentThread.ManagedThreadId);
	}

	private void LoadData(string fileName, string typeName, Type type, ref IObject item) {
		var stopwatch = new Stopwatch();
		stopwatch.Start();
		using (var input = File.OpenRead(fileName)) {
			using (var xml = XmlReader.Create(input)) {
				while (xml.Read())
					if (xml.NodeType == XmlNodeType.Element)
						LoadXmlDataObjectsFromReader(xml, type, ref item, typeName);
			}
		}

		stopwatch.Stop();
		Logger.AddInfo(" -  - [LoadDataFromXML] LoadObjects, name : " + Path.GetFileNameWithoutExtension(fileName) +
		               " , elapsed : " + stopwatch.Elapsed + " , thread : " + Thread.CurrentThread.ManagedThreadId);
	}

	private void LoadXmlDataObjectsFromReader(
		XmlReader xml,
		Type type,
		ref IObject item,
		string typeContext) {
		while (xml.Read())
			if (xml.NodeType == XmlNodeType.Element && xml.Name == "Item") {
				if (!xml.MoveToAttribute("id")) {
					Logger.AddError("Id not found");
					break;
				}

				var id = DefaultConverter.ParseUlong(xml.Value);
				if (xml.MoveToAttribute(nameof(type)))
					type = DataFactoryAttribute.GetTypeByName(xml.Value);
				item = GetOrCreateObjectThreadSave(type, id);
				if (item is IEditorDataReader editorDataReader)
					editorDataReader.EditorDataRead(xml, this, typeContext);
				else
					Logger.AddError("Type : " + item.GetType() + " is not IEditorDataReader");
			}
	}

	private IObject GetOrCreateObjectThreadSave(Type type, ulong id) {
		lock (sync) {
			IObject instance;
			if (currentData.Objects.TryGetValue(id, out instance))
				return instance;
			instance = (IObject)Activator.CreateInstance(type, id);
			currentData.Objects.Add(id, instance);
			return instance;
		}
	}

	public IObject GetOrCreateObjectThreadSave(ulong id) {
		lock (sync) {
			IObject instance;
			if (currentData.Objects.TryGetValue(id, out instance))
				return instance;
			instance = (IObject)Activator.CreateInstance(XmlDataLoaderUtility.GetObjTypeById(id), id);
			currentData.Objects.Add(id, instance);
			return instance;
		}
	}

	public override IObject GetOrCreateObject(ulong id) {
		IObject @object;
		if (currentData.Objects.TryGetValue(id, out @object))
			return @object;
		var instance = (IObject)Activator.CreateInstance(XmlDataLoaderUtility.GetObjTypeById(id), id);
		currentData.Objects.Add(id, instance);
		return instance;
	}

	public override IObject GetObjectByGuid(ulong id) {
		IObject objectByGuid;
		currentData.Objects.TryGetValue(id, out objectByGuid);
		return objectByGuid;
	}

	public void Clear() {
		if (currentData == null)
			return;
		if (currentData.Root != null)
			currentData.Root.Clear();
		currentData.Objects.Clear();
		currentData.Objects = null;
		currentData.Root = null;
		currentData = null;
	}
}
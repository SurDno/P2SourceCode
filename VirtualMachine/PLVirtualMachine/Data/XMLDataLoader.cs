// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Data.XMLDataLoader
// Assembly: VirtualMachine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4141C12C-9CB3-4BEE-B86E-276A0762C9CD
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.dll

using Cofe.Loggers;
using Cofe.Serializations.Converters;
using Engine.Common.Comparers;
using Engine.Common.Threads;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Objects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Xml;
using VirtualMachine.Common;
using VirtualMachine.Data;

#nullable disable
namespace PLVirtualMachine.Data
{
  public class XMLDataLoader : IStaticDataContainer, IDataCreator
  {
    private GameDataInfo currentData;
    private static object sync = new object();
    public static HashSet<string> Logs = new HashSet<string>();

    private XMLDataLoader()
    {
      IStaticDataContainer.StaticDataContainer = (IStaticDataContainer) this;
    }

    public static XMLDataLoader Instance { get; private set; } = new XMLDataLoader();

    public override IGameRoot GameRoot
    {
      get
      {
        GameDataInfo currentData = this.currentData;
        return currentData == null ? (IGameRoot) null : (IGameRoot) currentData.Root;
      }
    }

    public IEnumerator LoadDataFromXML(string dataFolderName, int threadCount, int dataCapacity)
    {
      this.Clear();
      this.currentData = new GameDataInfo()
      {
        Name = dataFolderName,
        Objects = new Dictionary<ulong, IObject>(dataCapacity, (IEqualityComparer<ulong>) UlongComparer.Instance)
      };
      yield return (object) null;
      if (threadCount == 0)
      {
        ThreadState<KeyValuePair<string, Type>, string> state = new ThreadState<KeyValuePair<string, Type>, string>()
        {
          Context = dataFolderName
        };
        foreach (KeyValuePair<string, Type> keyValuePair in DataFactoryAttribute.Items)
          this.ComputeDataType(keyValuePair, state);
      }
      else
        ThreadPoolUtility.Compute<KeyValuePair<string, Type>, string>(new Action<KeyValuePair<string, Type>, ThreadState<KeyValuePair<string, Type>, string>>(this.ComputeDataType), DataFactoryAttribute.Items, threadCount, dataFolderName);
      if (this.currentData.Root == null)
        Logger.AddError("Root not found");
      yield return (object) null;
      yield return (object) this.AfterLoadObjects();
      if (dataCapacity < this.currentData.Objects.Count)
        Logger.AddError("Wrong capacity");
      Logger.AddInfo("Load data complete, name : " + dataFolderName + " , count : " + (object) this.currentData.Objects.Count + " , capacity : " + (object) dataCapacity);
    }

    private IEnumerator AfterLoadObjects()
    {
      this.currentData.Root.OnAfterLoad();
      yield return (object) null;
      foreach (KeyValuePair<ulong, IObject> keyValuePair in this.currentData.Objects)
      {
        IObject @object = keyValuePair.Value;
        if (@object != this.currentData.Root && @object is IOnAfterLoaded onAfterLoaded)
          onAfterLoaded.OnAfterLoad();
        if (DelayTimer.Check)
          yield return (object) null;
      }
      foreach (KeyValuePair<ulong, IObject> keyValuePair in this.currentData.Objects)
      {
        IObject @object = keyValuePair.Value;
        if (@object != this.currentData.Root && @object is IOnAfterLoaded onAfterLoaded)
          onAfterLoaded.OnPostLoad();
        if (DelayTimer.Check)
          yield return (object) null;
      }
    }

    private void ComputeDataType(
      KeyValuePair<string, Type> item,
      ThreadState<KeyValuePair<string, Type>, string> state)
    {
      string str1 = state.Context + "/" + item.Key + ".xml.gz";
      if (File.Exists(str1))
      {
        IObject @object = (IObject) null;
        this.LoadDataCompress(str1, item.Key, item.Value, ref @object);
        if (!(item.Value == typeof (VMGameRoot)))
          return;
        this.currentData.Root = (VMGameRoot) @object;
      }
      else
      {
        string str2 = state.Context + "/" + item.Key + ".xml";
        if (!File.Exists(str2))
          return;
        IObject @object = (IObject) null;
        this.LoadData(str2, item.Key, item.Value, ref @object);
        if (!(item.Value == typeof (VMGameRoot)))
          return;
        this.currentData.Root = (VMGameRoot) @object;
      }
    }

    private void LoadDataCompress(string fileName, string typeName, Type type, ref IObject item)
    {
      Stopwatch stopwatch = new Stopwatch();
      stopwatch.Start();
      using (FileStream fileStream = File.OpenRead(fileName))
      {
        using (GZipStream input = new GZipStream((Stream) fileStream, CompressionMode.Decompress))
        {
          using (XmlReader xml = XmlReader.Create((Stream) input))
          {
            while (xml.Read())
            {
              if (xml.NodeType == XmlNodeType.Element)
                this.LoadXmlDataObjectsFromReader(xml, type, ref item, typeName);
            }
          }
        }
      }
      stopwatch.Stop();
      Logger.AddInfo(" -  - [LoadDataFromXML] LoadObjects, name : " + Path.GetFileNameWithoutExtension(fileName) + " , elapsed : " + (object) stopwatch.Elapsed + " , thread : " + (object) Thread.CurrentThread.ManagedThreadId);
    }

    private void LoadData(string fileName, string typeName, Type type, ref IObject item)
    {
      Stopwatch stopwatch = new Stopwatch();
      stopwatch.Start();
      using (FileStream input = File.OpenRead(fileName))
      {
        using (XmlReader xml = XmlReader.Create((Stream) input))
        {
          while (xml.Read())
          {
            if (xml.NodeType == XmlNodeType.Element)
              this.LoadXmlDataObjectsFromReader(xml, type, ref item, typeName);
          }
        }
      }
      stopwatch.Stop();
      Logger.AddInfo(" -  - [LoadDataFromXML] LoadObjects, name : " + Path.GetFileNameWithoutExtension(fileName) + " , elapsed : " + (object) stopwatch.Elapsed + " , thread : " + (object) Thread.CurrentThread.ManagedThreadId);
    }

    private void LoadXmlDataObjectsFromReader(
      XmlReader xml,
      Type type,
      ref IObject item,
      string typeContext)
    {
      while (xml.Read())
      {
        if (xml.NodeType == XmlNodeType.Element && xml.Name == "Item")
        {
          if (!xml.MoveToAttribute("id"))
          {
            Logger.AddError("Id not found");
            break;
          }
          ulong id = DefaultConverter.ParseUlong(xml.Value);
          if (xml.MoveToAttribute(nameof (type)))
            type = DataFactoryAttribute.GetTypeByName(xml.Value);
          item = this.GetOrCreateObjectThreadSave(type, id);
          if (item is IEditorDataReader editorDataReader)
            editorDataReader.EditorDataRead(xml, (IDataCreator) this, typeContext);
          else
            Logger.AddError("Type : " + (object) item.GetType() + " is not IEditorDataReader");
        }
      }
    }

    private IObject GetOrCreateObjectThreadSave(Type type, ulong id)
    {
      lock (XMLDataLoader.sync)
      {
        IObject instance;
        if (this.currentData.Objects.TryGetValue(id, out instance))
          return instance;
        instance = (IObject) Activator.CreateInstance(type, (object) id);
        this.currentData.Objects.Add(id, instance);
        return instance;
      }
    }

    public IObject GetOrCreateObjectThreadSave(ulong id)
    {
      lock (XMLDataLoader.sync)
      {
        IObject instance;
        if (this.currentData.Objects.TryGetValue(id, out instance))
          return instance;
        instance = (IObject) Activator.CreateInstance(XmlDataLoaderUtility.GetObjTypeById(id), (object) id);
        this.currentData.Objects.Add(id, instance);
        return instance;
      }
    }

    public override IObject GetOrCreateObject(ulong id)
    {
      IObject @object;
      if (this.currentData.Objects.TryGetValue(id, out @object))
        return @object;
      IObject instance = (IObject) Activator.CreateInstance(XmlDataLoaderUtility.GetObjTypeById(id), (object) id);
      this.currentData.Objects.Add(id, instance);
      return instance;
    }

    public override IObject GetObjectByGuid(ulong id)
    {
      IObject objectByGuid;
      this.currentData.Objects.TryGetValue(id, out objectByGuid);
      return objectByGuid;
    }

    public void Clear()
    {
      if (this.currentData == null)
        return;
      if (this.currentData.Root != null)
        this.currentData.Root.Clear();
      this.currentData.Objects.Clear();
      this.currentData.Objects = (Dictionary<ulong, IObject>) null;
      this.currentData.Root = (VMGameRoot) null;
      this.currentData = (GameDataInfo) null;
    }
  }
}

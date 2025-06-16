using Cofe.Serializations.Data;
using System;
using System.Collections;
using System.Xml;

namespace Engine.Common
{
  public interface IVirtualMachine
  {
    bool IsInitialized { get; }

    bool IsLoaded { get; }

    bool IsDataLoaded { get; }

    IEnumerator Initialize(bool debug, float maxEventQueueTimePerFrame = 0.0f);

    void Terminate();

    IEnumerator LoadData(string fileName, int threadCount, int capacity);

    void UnloadData();

    IEnumerator Load();

    IEnumerator Load(XmlElement element);

    void Save(IDataWriter writer);

    void Unload();

    void Update(TimeSpan delta);
  }
}

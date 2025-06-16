// Decompiled with JetBrains decompiler
// Type: Engine.Common.IVirtualMachine
// Assembly: Engine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3568A167-18A7-4983-8BC2-C25824901591
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Engine.Common.dll

using Cofe.Serializations.Data;
using System;
using System.Collections;
using System.Xml;

#nullable disable
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

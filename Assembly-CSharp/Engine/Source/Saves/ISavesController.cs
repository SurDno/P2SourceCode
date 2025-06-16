// Decompiled with JetBrains decompiler
// Type: Engine.Source.Saves.ISavesController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Serializations.Data;
using Engine.Source.Services.Saves;
using System.Collections;
using System.Xml;

#nullable disable
namespace Engine.Source.Saves
{
  public interface ISavesController
  {
    IEnumerator Load(IErrorLoadingHandler errorHandler);

    IEnumerator Load(XmlElement element, string context, IErrorLoadingHandler errorHandler);

    void Unload();

    void Save(IDataWriter element, string context);
  }
}

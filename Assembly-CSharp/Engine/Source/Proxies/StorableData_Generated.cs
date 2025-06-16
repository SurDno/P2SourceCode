// Decompiled with JetBrains decompiler
// Type: Engine.Source.Proxies.StorableData_Generated
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Converters;
using Engine.Common.Components;
using Engine.Source.Components.Saves;
using Scripts.Tools.Serializations.Converters;
using System;

#nullable disable
namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (StorableData))]
  public class StorableData_Generated : StorableData, ISerializeStateSave, ISerializeStateLoad
  {
    public void StateSave(IDataWriter writer)
    {
      CustomStateSaveUtility.SaveReference(writer, "Storage", (object) this.Storage);
      DefaultDataWriteUtility.Write(writer, "TemplateId", this.TemplateId);
    }

    public void StateLoad(IDataReader reader, Type type)
    {
      this.Storage = CustomStateLoadUtility.LoadReference<IStorageComponent>(reader, "Storage", this.Storage);
      this.TemplateId = DefaultDataReadUtility.Read(reader, "TemplateId", this.TemplateId);
    }
  }
}

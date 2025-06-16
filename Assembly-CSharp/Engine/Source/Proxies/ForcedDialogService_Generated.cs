// Decompiled with JetBrains decompiler
// Type: Engine.Source.Proxies.ForcedDialogService_Generated
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Converters;
using Engine.Source.Services;
using System;

#nullable disable
namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (ForcedDialogService))]
  public class ForcedDialogService_Generated : 
    ForcedDialogService,
    ISerializeStateSave,
    ISerializeStateLoad
  {
    public void StateSave(IDataWriter writer)
    {
      DefaultStateSaveUtility.SaveListSerialize<ForcedDialogCharacterInfo>(writer, "Characters", this.characters);
    }

    public void StateLoad(IDataReader reader, Type type)
    {
      this.characters = DefaultStateLoadUtility.ReadListSerialize<ForcedDialogCharacterInfo>(reader, "Characters", this.characters);
    }
  }
}

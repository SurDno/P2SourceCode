// Decompiled with JetBrains decompiler
// Type: Engine.Source.Proxies.ForcedDialogCharacterInfo_Generated
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common;
using Engine.Common.Commons.Converters;
using Engine.Source.Services;
using Scripts.Tools.Serializations.Converters;
using System;

#nullable disable
namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (ForcedDialogCharacterInfo))]
  public class ForcedDialogCharacterInfo_Generated : 
    ForcedDialogCharacterInfo,
    ISerializeStateSave,
    ISerializeStateLoad
  {
    public void StateSave(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "Distance", this.Distance);
      CustomStateSaveUtility.SaveReference(writer, "Character", (object) this.Character);
    }

    public void StateLoad(IDataReader reader, Type type)
    {
      this.Distance = DefaultDataReadUtility.Read(reader, "Distance", this.Distance);
      this.Character = CustomStateLoadUtility.LoadReference<IEntity>(reader, "Character", this.Character);
    }
  }
}

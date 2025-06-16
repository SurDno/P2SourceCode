// Decompiled with JetBrains decompiler
// Type: Engine.Source.Proxies.DropBagService_Generated
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common;
using Engine.Source.Services;
using Scripts.Tools.Serializations.Converters;
using System;

#nullable disable
namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (DropBagService))]
  public class DropBagService_Generated : DropBagService, ISerializeStateSave, ISerializeStateLoad
  {
    public void StateSave(IDataWriter writer)
    {
      CustomStateSaveUtility.SaveListReferences<IEntity>(writer, "Bags", this.bags);
    }

    public void StateLoad(IDataReader reader, Type type)
    {
      this.bags = CustomStateLoadUtility.LoadListReferences<IEntity>(reader, "Bags", this.bags);
    }
  }
}

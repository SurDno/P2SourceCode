// Decompiled with JetBrains decompiler
// Type: Engine.Source.Proxies.CollectControllerComponent_Generated
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Components;
using Scripts.Tools.Serializations.Converters;
using System;

#nullable disable
namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (CollectControllerComponent))]
  public class CollectControllerComponent_Generated : 
    CollectControllerComponent,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      CollectControllerComponent_Generated instance = Activator.CreateInstance<CollectControllerComponent_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      CollectControllerComponent_Generated componentGenerated = (CollectControllerComponent_Generated) target2;
      componentGenerated.itemEntity = this.itemEntity;
      componentGenerated.sendActionEvent = this.sendActionEvent;
    }

    public void DataWrite(IDataWriter writer)
    {
      UnityDataWriteUtility.Write<IEntity>(writer, "Storable", this.itemEntity);
      DefaultDataWriteUtility.Write(writer, "SendActionEvent", this.sendActionEvent);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.itemEntity = UnityDataReadUtility.Read<IEntity>(reader, "Storable", this.itemEntity);
      this.sendActionEvent = DefaultDataReadUtility.Read(reader, "SendActionEvent", this.sendActionEvent);
    }
  }
}

using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Components;
using Scripts.Tools.Serializations.Converters;
using System;

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

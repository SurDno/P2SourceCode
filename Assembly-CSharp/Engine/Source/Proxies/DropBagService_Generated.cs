using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common;
using Engine.Source.Services;
using Scripts.Tools.Serializations.Converters;
using System;

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

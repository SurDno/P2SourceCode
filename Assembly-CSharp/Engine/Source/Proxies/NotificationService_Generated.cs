using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Source.Services;
using System;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (NotificationService))]
  public class NotificationService_Generated : 
    NotificationService,
    ISerializeStateSave,
    ISerializeStateLoad
  {
    public void StateSave(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteListEnum<NotificationEnum>(writer, "BlockedTypes", this.blockedTypes);
    }

    public void StateLoad(IDataReader reader, Type type)
    {
      this.blockedTypes = DefaultDataReadUtility.ReadListEnum<NotificationEnum>(reader, "BlockedTypes", this.blockedTypes);
    }
  }
}

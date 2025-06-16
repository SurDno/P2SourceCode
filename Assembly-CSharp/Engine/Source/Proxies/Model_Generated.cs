using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Services;
using Engine.Source.Commons;
using Scripts.Tools.Serializations.Converters;
using System;
using UnityEngine;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (Model))]
  public class Model_Generated : 
    Model,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone() => (object) ServiceCache.Factory.Instantiate<Model_Generated>(this);

    public void CopyTo(object target2)
    {
      Model_Generated modelGenerated = (Model_Generated) target2;
      modelGenerated.name = this.name;
      modelGenerated.connection = this.connection;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "Id", this.id);
      UnityDataWriteUtility.Write<GameObject>(writer, "Connection", this.connection);
    }

    public void DataRead(IDataReader reader, System.Type type)
    {
      this.id = DefaultDataReadUtility.Read(reader, "Id", this.id);
      this.connection = UnityDataReadUtility.Read<GameObject>(reader, "Connection", this.connection);
    }
  }
}

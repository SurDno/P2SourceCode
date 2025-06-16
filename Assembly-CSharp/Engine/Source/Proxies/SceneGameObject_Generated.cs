using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Connections;
using System;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (SceneGameObject))]
  public class SceneGameObject_Generated : 
    SceneGameObject,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      SceneGameObject_Generated instance = Activator.CreateInstance<SceneGameObject_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2) => ((SceneGameObject) target2).id = this.id;

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "Id", this.id);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.id = DefaultDataReadUtility.Read(reader, "Id", this.id);
    }
  }
}

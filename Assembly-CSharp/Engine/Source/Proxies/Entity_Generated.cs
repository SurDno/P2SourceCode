using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Services;
using Engine.Source.Commons;
using Scripts.Tools.Serializations.Converters;
using System;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (Entity))]
  public class Entity_Generated : 
    Entity,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead,
    ISerializeStateSave,
    ISerializeStateLoad
  {
    public object Clone() => (object) ServiceCache.Factory.Instantiate<Entity_Generated>(this);

    public void CopyTo(object target2)
    {
      Entity_Generated entityGenerated = (Entity_Generated) target2;
      entityGenerated.name = this.name;
      CloneableObjectUtility.CopyListTo<IComponent>(entityGenerated.components, this.components);
      entityGenerated.isEnabled = this.isEnabled;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "Id", this.id);
      DefaultDataWriteUtility.WriteListSerialize<IComponent>(writer, "Components", this.components);
      DefaultDataWriteUtility.Write(writer, "IsEnabled", this.isEnabled);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.id = DefaultDataReadUtility.Read(reader, "Id", this.id);
      this.components = DefaultDataReadUtility.ReadListSerialize<IComponent>(reader, "Components", this.components);
      this.isEnabled = DefaultDataReadUtility.Read(reader, "IsEnabled", this.isEnabled);
    }

    public void StateSave(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "Id", this.id);
      DefaultDataWriteUtility.Write(writer, "Name", this.name);
      CustomStateSaveUtility.SaveListComponents(writer, "Components", this.components);
      DefaultDataWriteUtility.Write(writer, "IsEnabled", this.isEnabled);
      DefaultDataWriteUtility.Write(writer, "HierarchyPath", this.HierarchyPath);
    }

    public void StateLoad(IDataReader reader, Type type)
    {
      this.name = DefaultDataReadUtility.Read(reader, "Name", this.name);
      this.components = CustomStateLoadUtility.LoadListComponents(reader, "Components", this.components);
      this.isEnabled = DefaultDataReadUtility.Read(reader, "IsEnabled", this.isEnabled);
    }
  }
}

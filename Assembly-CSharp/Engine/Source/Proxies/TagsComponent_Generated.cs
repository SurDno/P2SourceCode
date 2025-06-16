using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Components;
using Engine.Source.Components;
using System;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (TagsComponent))]
  public class TagsComponent_Generated : 
    TagsComponent,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      TagsComponent_Generated instance = Activator.CreateInstance<TagsComponent_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      CloneableObjectUtility.FillListTo<EntityTagEnum>(((TagsComponent) target2).tags, this.tags);
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteListEnum<EntityTagEnum>(writer, "Tags", this.tags);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.tags = DefaultDataReadUtility.ReadListEnum<EntityTagEnum>(reader, "Tags", this.tags);
    }
  }
}

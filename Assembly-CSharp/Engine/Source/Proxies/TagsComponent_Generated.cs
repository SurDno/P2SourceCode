using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Components;

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
      CopyTo(instance);
      return instance;
    }

    public void CopyTo(object target2)
    {
      CloneableObjectUtility.FillListTo(((TagsComponent_Generated) target2).tags, tags);
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteListEnum(writer, "Tags", tags);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      tags = DefaultDataReadUtility.ReadListEnum(reader, "Tags", tags);
    }
  }
}

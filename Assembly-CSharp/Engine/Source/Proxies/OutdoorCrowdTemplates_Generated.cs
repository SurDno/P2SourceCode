using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.OutdoorCrowds;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (OutdoorCrowdTemplates))]
  public class OutdoorCrowdTemplates_Generated : 
    OutdoorCrowdTemplates,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      OutdoorCrowdTemplates_Generated instance = Activator.CreateInstance<OutdoorCrowdTemplates_Generated>();
      CopyTo(instance);
      return instance;
    }

    public void CopyTo(object target2)
    {
      OutdoorCrowdTemplates_Generated templatesGenerated = (OutdoorCrowdTemplates_Generated) target2;
      templatesGenerated.Name = Name;
      CloneableObjectUtility.CopyListTo(templatesGenerated.Templates, Templates);
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "Name", Name);
      DefaultDataWriteUtility.WriteListSerialize(writer, "Templates", Templates);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      Name = DefaultDataReadUtility.Read(reader, "Name", Name);
      Templates = DefaultDataReadUtility.ReadListSerialize(reader, "Templates", Templates);
    }
  }
}

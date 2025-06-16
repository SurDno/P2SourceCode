using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Components.Crowds;
using Scripts.Tools.Serializations.Converters;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (HerbRootsTemplate))]
  public class HerbRootsTemplate_Generated : 
    HerbRootsTemplate,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead,
    ISerializeStateSave,
    ISerializeStateLoad
  {
    public object Clone()
    {
      HerbRootsTemplate_Generated instance = Activator.CreateInstance<HerbRootsTemplate_Generated>();
      CopyTo(instance);
      return instance;
    }

    public void CopyTo(object target2)
    {
      HerbRootsTemplate_Generated templateGenerated = (HerbRootsTemplate_Generated) target2;
      templateGenerated.Template = Template;
      templateGenerated.Weight = Weight;
    }

    public void DataWrite(IDataWriter writer)
    {
      UnityDataWriteUtility.Write(writer, "Template", Template);
      DefaultDataWriteUtility.Write(writer, "Weight", Weight);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      Template = UnityDataReadUtility.Read(reader, "Template", Template);
      Weight = DefaultDataReadUtility.Read(reader, "Weight", Weight);
    }

    public void StateSave(IDataWriter writer)
    {
    }

    public void StateLoad(IDataReader reader, Type type)
    {
    }
  }
}

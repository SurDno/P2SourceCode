using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using System;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (StorableTooltipSimple))]
  public class StorableTooltipSimple_Generated : 
    StorableTooltipSimple,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      StorableTooltipSimple_Generated instance = Activator.CreateInstance<StorableTooltipSimple_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      StorableTooltipSimple_Generated tooltipSimpleGenerated = (StorableTooltipSimple_Generated) target2;
      tooltipSimpleGenerated.isEnabled = this.isEnabled;
      tooltipSimpleGenerated.info = CloneableObjectUtility.Clone<StorableTooltipInfo>(this.info);
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "IsEnabled", this.isEnabled);
      DefaultDataWriteUtility.WriteSerialize<StorableTooltipInfo>(writer, "Info", this.info);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.isEnabled = DefaultDataReadUtility.Read(reader, "IsEnabled", this.isEnabled);
      this.info = DefaultDataReadUtility.ReadSerialize<StorableTooltipInfo>(reader, "Info");
    }
  }
}

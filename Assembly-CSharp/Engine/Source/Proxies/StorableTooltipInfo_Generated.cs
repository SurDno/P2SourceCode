using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Scripts.Tools.Serializations.Converters;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (StorableTooltipInfo))]
  public class StorableTooltipInfo_Generated : 
    StorableTooltipInfo,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      StorableTooltipInfo_Generated instance = Activator.CreateInstance<StorableTooltipInfo_Generated>();
      CopyTo(instance);
      return instance;
    }

    public void CopyTo(object target2)
    {
      StorableTooltipInfo_Generated tooltipInfoGenerated = (StorableTooltipInfo_Generated) target2;
      tooltipInfoGenerated.Name = Name;
      tooltipInfoGenerated.Value = Value;
      tooltipInfoGenerated.Color = Color;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteEnum(writer, "Name", Name);
      DefaultDataWriteUtility.Write(writer, "Value", Value);
      UnityDataWriteUtility.Write(writer, "Color", Color);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      Name = DefaultDataReadUtility.ReadEnum<StorableTooltipNameEnum>(reader, "Name");
      Value = DefaultDataReadUtility.Read(reader, "Value", Value);
      Color = UnityDataReadUtility.Read(reader, "Color", Color);
    }
  }
}

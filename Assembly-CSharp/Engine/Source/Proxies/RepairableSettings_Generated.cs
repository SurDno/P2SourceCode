using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Services;
using Engine.Source.Components.Repairing;
using Scripts.Tools.Serializations.Converters;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (RepairableSettings))]
  public class RepairableSettings_Generated : 
    RepairableSettings,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      return ServiceCache.Factory.Instantiate(this);
    }

    public void CopyTo(object target2)
    {
      RepairableSettings_Generated settingsGenerated = (RepairableSettings_Generated) target2;
      settingsGenerated.name = name;
      CloneableObjectUtility.CopyListTo(settingsGenerated.levels, levels);
      settingsGenerated.nonItemImage = nonItemImage;
      settingsGenerated.repairSound = repairSound;
      settingsGenerated.verbOverride = verbOverride;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "Id", id);
      DefaultDataWriteUtility.WriteListSerialize(writer, "Levels", levels);
      UnityDataWriteUtility.Write(writer, "NonItemImage", nonItemImage);
      UnityDataWriteUtility.Write(writer, "RepairSound", repairSound);
      DefaultDataWriteUtility.Write(writer, "VerbOverride", verbOverride);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      id = DefaultDataReadUtility.Read(reader, "Id", id);
      levels = DefaultDataReadUtility.ReadListSerialize(reader, "Levels", levels);
      nonItemImage = UnityDataReadUtility.Read(reader, "NonItemImage", nonItemImage);
      repairSound = UnityDataReadUtility.Read(reader, "RepairSound", repairSound);
      verbOverride = DefaultDataReadUtility.Read(reader, "VerbOverride", verbOverride);
    }
  }
}

using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Difficulties;
using System;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (ExternalDifficultySettings))]
  public class ExternalDifficultySettings_Generated : 
    ExternalDifficultySettings,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      ExternalDifficultySettings_Generated instance = Activator.CreateInstance<ExternalDifficultySettings_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      ExternalDifficultySettings_Generated settingsGenerated = (ExternalDifficultySettings_Generated) target2;
      settingsGenerated.Version = this.Version;
      CloneableObjectUtility.CopyListTo<DifficultyItemData>(settingsGenerated.Items, this.Items);
      CloneableObjectUtility.CopyListTo<DifficultyGroupData>(settingsGenerated.Groups, this.Groups);
      CloneableObjectUtility.CopyListTo<DifficultyPresetData>(settingsGenerated.Presets, this.Presets);
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "Version", this.Version);
      DefaultDataWriteUtility.WriteListSerialize<DifficultyItemData>(writer, "Items", this.Items);
      DefaultDataWriteUtility.WriteListSerialize<DifficultyGroupData>(writer, "Groups", this.Groups);
      DefaultDataWriteUtility.WriteListSerialize<DifficultyPresetData>(writer, "Presets", this.Presets);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.Version = DefaultDataReadUtility.Read(reader, "Version", this.Version);
      this.Items = DefaultDataReadUtility.ReadListSerialize<DifficultyItemData>(reader, "Items", this.Items);
      this.Groups = DefaultDataReadUtility.ReadListSerialize<DifficultyGroupData>(reader, "Groups", this.Groups);
      this.Presets = DefaultDataReadUtility.ReadListSerialize<DifficultyPresetData>(reader, "Presets", this.Presets);
    }
  }
}

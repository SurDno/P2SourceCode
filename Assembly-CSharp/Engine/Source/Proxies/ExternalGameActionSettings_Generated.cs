using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Settings.External;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (ExternalGameActionSettings))]
  public class ExternalGameActionSettings_Generated : 
    ExternalGameActionSettings,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      ExternalGameActionSettings_Generated instance = Activator.CreateInstance<ExternalGameActionSettings_Generated>();
      CopyTo(instance);
      return instance;
    }

    public void CopyTo(object target2)
    {
      ExternalGameActionSettings_Generated settingsGenerated = (ExternalGameActionSettings_Generated) target2;
      settingsGenerated.Version = Version;
      CloneableObjectUtility.CopyListTo(settingsGenerated.Groups_Set_1, Groups_Set_1);
      CloneableObjectUtility.CopyListTo(settingsGenerated.Groups_Set_2, Groups_Set_2);
      CloneableObjectUtility.CopyListTo(settingsGenerated.Groups_Set_3, Groups_Set_3);
      CloneableObjectUtility.CopyListTo(settingsGenerated.Layouts, Layouts);
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "Version", Version);
      DefaultDataWriteUtility.WriteListSerialize(writer, "Groups_Set_1", Groups_Set_1);
      DefaultDataWriteUtility.WriteListSerialize(writer, "Groups_Set_2", Groups_Set_2);
      DefaultDataWriteUtility.WriteListSerialize(writer, "Groups_Set_3", Groups_Set_3);
      DefaultDataWriteUtility.WriteListSerialize(writer, "Layouts", Layouts);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      Version = DefaultDataReadUtility.Read(reader, "Version", Version);
      Groups_Set_1 = DefaultDataReadUtility.ReadListSerialize(reader, "Groups_Set_1", Groups_Set_1);
      Groups_Set_2 = DefaultDataReadUtility.ReadListSerialize(reader, "Groups_Set_2", Groups_Set_2);
      Groups_Set_3 = DefaultDataReadUtility.ReadListSerialize(reader, "Groups_Set_3", Groups_Set_3);
      Layouts = DefaultDataReadUtility.ReadListSerialize(reader, "Layouts", Layouts);
    }
  }
}

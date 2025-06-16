using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Services.Inputs;
using Engine.Source.Settings.External;
using System;

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
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      ExternalGameActionSettings_Generated settingsGenerated = (ExternalGameActionSettings_Generated) target2;
      settingsGenerated.Version = this.Version;
      CloneableObjectUtility.CopyListTo<ActionGroup>(settingsGenerated.Groups_Set_1, this.Groups_Set_1);
      CloneableObjectUtility.CopyListTo<ActionGroup>(settingsGenerated.Groups_Set_2, this.Groups_Set_2);
      CloneableObjectUtility.CopyListTo<ActionGroup>(settingsGenerated.Groups_Set_3, this.Groups_Set_3);
      CloneableObjectUtility.CopyListTo<JoystickLayout>(settingsGenerated.Layouts, this.Layouts);
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "Version", this.Version);
      DefaultDataWriteUtility.WriteListSerialize<ActionGroup>(writer, "Groups_Set_1", this.Groups_Set_1);
      DefaultDataWriteUtility.WriteListSerialize<ActionGroup>(writer, "Groups_Set_2", this.Groups_Set_2);
      DefaultDataWriteUtility.WriteListSerialize<ActionGroup>(writer, "Groups_Set_3", this.Groups_Set_3);
      DefaultDataWriteUtility.WriteListSerialize<JoystickLayout>(writer, "Layouts", this.Layouts);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.Version = DefaultDataReadUtility.Read(reader, "Version", this.Version);
      this.Groups_Set_1 = DefaultDataReadUtility.ReadListSerialize<ActionGroup>(reader, "Groups_Set_1", this.Groups_Set_1);
      this.Groups_Set_2 = DefaultDataReadUtility.ReadListSerialize<ActionGroup>(reader, "Groups_Set_2", this.Groups_Set_2);
      this.Groups_Set_3 = DefaultDataReadUtility.ReadListSerialize<ActionGroup>(reader, "Groups_Set_3", this.Groups_Set_3);
      this.Layouts = DefaultDataReadUtility.ReadListSerialize<JoystickLayout>(reader, "Layouts", this.Layouts);
    }
  }
}

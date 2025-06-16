using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.BoundCharacters;
using Engine.Common.Commons;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Components.BoundCharacters;
using Scripts.Tools.Serializations.Converters;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (BoundCharacterComponent))]
  public class BoundCharacterComponent_Generated : 
    BoundCharacterComponent,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead,
    ISerializeStateSave,
    ISerializeStateLoad
  {
    public object Clone()
    {
      BoundCharacterComponent_Generated instance = Activator.CreateInstance<BoundCharacterComponent_Generated>();
      CopyTo(instance);
      return instance;
    }

    public void CopyTo(object target2)
    {
      ((BoundCharacterComponent_Generated) target2).isEnabled = isEnabled;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "IsEnabled", isEnabled);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      isEnabled = DefaultDataReadUtility.Read(reader, "IsEnabled", isEnabled);
    }

    public void StateSave(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "SortOrder", SortOrder);
      DefaultDataWriteUtility.WriteEnum(writer, "SeenGroup", SeenGroup);
      EngineDataWriteUtility.Write(writer, "Name", Name);
      DefaultDataWriteUtility.WriteEnum(writer, "SeenBoundHealthState", SeenBoundHealthState);
      CustomStateSaveUtility.SaveReference(writer, "HomeRegion", HomeRegion);
      DefaultDataWriteUtility.Write(writer, "IsEnabled", isEnabled);
      DefaultDataWriteUtility.WriteEnum(writer, "Group", group);
      UnityDataWriteUtility.Write(writer, "Placeholder", placeholder);
    }

    public void StateLoad(IDataReader reader, Type type)
    {
      SortOrder = DefaultDataReadUtility.Read(reader, "SortOrder", SortOrder);
      SeenGroup = DefaultDataReadUtility.ReadEnum<BoundCharacterGroup>(reader, "SeenGroup");
      Name = EngineDataReadUtility.Read(reader, "Name", Name);
      SeenBoundHealthState = DefaultDataReadUtility.ReadEnum<BoundHealthStateEnum>(reader, "SeenBoundHealthState");
      HomeRegion = CustomStateLoadUtility.LoadReference(reader, "HomeRegion", HomeRegion);
      isEnabled = DefaultDataReadUtility.Read(reader, "IsEnabled", isEnabled);
      group = DefaultDataReadUtility.ReadEnum<BoundCharacterGroup>(reader, "Group");
      placeholder = UnityDataReadUtility.Read(reader, "Placeholder", placeholder);
    }
  }
}

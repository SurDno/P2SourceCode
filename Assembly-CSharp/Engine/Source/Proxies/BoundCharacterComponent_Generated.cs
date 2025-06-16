// Decompiled with JetBrains decompiler
// Type: Engine.Source.Proxies.BoundCharacterComponent_Generated
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common;
using Engine.Common.BoundCharacters;
using Engine.Common.Commons;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Components.BoundCharacters;
using Scripts.Tools.Serializations.Converters;
using System;

#nullable disable
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
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      ((BoundCharacterComponent) target2).isEnabled = this.isEnabled;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "IsEnabled", this.isEnabled);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.isEnabled = DefaultDataReadUtility.Read(reader, "IsEnabled", this.isEnabled);
    }

    public void StateSave(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "SortOrder", this.SortOrder);
      DefaultDataWriteUtility.WriteEnum<BoundCharacterGroup>(writer, "SeenGroup", this.SeenGroup);
      EngineDataWriteUtility.Write(writer, "Name", this.Name);
      DefaultDataWriteUtility.WriteEnum<BoundHealthStateEnum>(writer, "SeenBoundHealthState", this.SeenBoundHealthState);
      CustomStateSaveUtility.SaveReference(writer, "HomeRegion", (object) this.HomeRegion);
      DefaultDataWriteUtility.Write(writer, "IsEnabled", this.isEnabled);
      DefaultDataWriteUtility.WriteEnum<BoundCharacterGroup>(writer, "Group", this.group);
      UnityDataWriteUtility.Write<IBoundCharacterPlaceholder>(writer, "Placeholder", this.placeholder);
    }

    public void StateLoad(IDataReader reader, Type type)
    {
      this.SortOrder = DefaultDataReadUtility.Read(reader, "SortOrder", this.SortOrder);
      this.SeenGroup = DefaultDataReadUtility.ReadEnum<BoundCharacterGroup>(reader, "SeenGroup");
      this.Name = EngineDataReadUtility.Read(reader, "Name", this.Name);
      this.SeenBoundHealthState = DefaultDataReadUtility.ReadEnum<BoundHealthStateEnum>(reader, "SeenBoundHealthState");
      this.HomeRegion = CustomStateLoadUtility.LoadReference<IEntity>(reader, "HomeRegion", this.HomeRegion);
      this.isEnabled = DefaultDataReadUtility.Read(reader, "IsEnabled", this.isEnabled);
      this.group = DefaultDataReadUtility.ReadEnum<BoundCharacterGroup>(reader, "Group");
      this.placeholder = UnityDataReadUtility.Read<IBoundCharacterPlaceholder>(reader, "Placeholder", this.placeholder);
    }
  }
}

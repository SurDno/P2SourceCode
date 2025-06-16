// Decompiled with JetBrains decompiler
// Type: Engine.Source.Proxies.RepairableSettings_Generated
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Services;
using Engine.Source.Components.Repairing;
using Scripts.Tools.Serializations.Converters;
using System;
using UnityEngine;

#nullable disable
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
      return (object) ServiceCache.Factory.Instantiate<RepairableSettings_Generated>(this);
    }

    public void CopyTo(object target2)
    {
      RepairableSettings_Generated settingsGenerated = (RepairableSettings_Generated) target2;
      settingsGenerated.name = this.name;
      CloneableObjectUtility.CopyListTo<RepairableLevel>(settingsGenerated.levels, this.levels);
      settingsGenerated.nonItemImage = this.nonItemImage;
      settingsGenerated.repairSound = this.repairSound;
      settingsGenerated.verbOverride = this.verbOverride;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "Id", this.id);
      DefaultDataWriteUtility.WriteListSerialize<RepairableLevel>(writer, "Levels", this.levels);
      UnityDataWriteUtility.Write<Sprite>(writer, "NonItemImage", this.nonItemImage);
      UnityDataWriteUtility.Write<AudioClip>(writer, "RepairSound", this.repairSound);
      DefaultDataWriteUtility.Write(writer, "VerbOverride", this.verbOverride);
    }

    public void DataRead(IDataReader reader, System.Type type)
    {
      this.id = DefaultDataReadUtility.Read(reader, "Id", this.id);
      this.levels = DefaultDataReadUtility.ReadListSerialize<RepairableLevel>(reader, "Levels", this.levels);
      this.nonItemImage = UnityDataReadUtility.Read<Sprite>(reader, "NonItemImage", this.nonItemImage);
      this.repairSound = UnityDataReadUtility.Read<AudioClip>(reader, "RepairSound", this.repairSound);
      this.verbOverride = DefaultDataReadUtility.Read(reader, "VerbOverride", this.verbOverride);
    }
  }
}

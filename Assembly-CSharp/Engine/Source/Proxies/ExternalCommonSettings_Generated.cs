// Decompiled with JetBrains decompiler
// Type: Engine.Source.Proxies.ExternalCommonSettings_Generated
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Settings.External;
using System;

#nullable disable
namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (ExternalCommonSettings))]
  public class ExternalCommonSettings_Generated : 
    ExternalCommonSettings,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      ExternalCommonSettings_Generated instance = Activator.CreateInstance<ExternalCommonSettings_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      ExternalCommonSettings_Generated settingsGenerated = (ExternalCommonSettings_Generated) target2;
      settingsGenerated.Version = this.Version;
      settingsGenerated.DropBagSearchRadius = this.DropBagSearchRadius;
      settingsGenerated.DropBagOffset = this.DropBagOffset;
      settingsGenerated.ChildGunJam = this.ChildGunJam;
      settingsGenerated.StepsDistance = this.StepsDistance;
      settingsGenerated.InteractionDistance = this.InteractionDistance;
      settingsGenerated.AwayDistance = this.AwayDistance;
      settingsGenerated.DangerDistance = this.DangerDistance;
      settingsGenerated.DoorSpeed = this.DoorSpeed;
      settingsGenerated.IdleReplicsFrequencyMin = this.IdleReplicsFrequencyMin;
      settingsGenerated.IdleReplicsFrequencyMax = this.IdleReplicsFrequencyMax;
      settingsGenerated.IdleReplicsMaxRangeToPlayer = this.IdleReplicsMaxRangeToPlayer;
      settingsGenerated.IdleReplicsDistanceMin = this.IdleReplicsDistanceMin;
      settingsGenerated.IdleReplicsDistanceMax = this.IdleReplicsDistanceMax;
      settingsGenerated.EntitySubtitlesDistanceMax = this.EntitySubtitlesDistanceMax;
      settingsGenerated.BleuprintSubtitlesDistanceMax = this.BleuprintSubtitlesDistanceMax;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "Version", this.Version);
      DefaultDataWriteUtility.Write(writer, "DropBagSearchRadius", this.DropBagSearchRadius);
      DefaultDataWriteUtility.Write(writer, "DropBagOffset", this.DropBagOffset);
      DefaultDataWriteUtility.Write(writer, "ChildGunJam", this.ChildGunJam);
      DefaultDataWriteUtility.Write(writer, "StepsDistance", this.StepsDistance);
      DefaultDataWriteUtility.Write(writer, "InteractionDistance", this.InteractionDistance);
      DefaultDataWriteUtility.Write(writer, "AwayDistance", this.AwayDistance);
      DefaultDataWriteUtility.Write(writer, "DangerDistance", this.DangerDistance);
      DefaultDataWriteUtility.Write(writer, "DoorSpeed", this.DoorSpeed);
      DefaultDataWriteUtility.Write(writer, "IdleReplicsFrequencyMin", this.IdleReplicsFrequencyMin);
      DefaultDataWriteUtility.Write(writer, "IdleReplicsFrequencyMax", this.IdleReplicsFrequencyMax);
      DefaultDataWriteUtility.Write(writer, "IdleReplicsMaxRangeToPlayer", this.IdleReplicsMaxRangeToPlayer);
      DefaultDataWriteUtility.Write(writer, "IdleReplicsDistanceMin", this.IdleReplicsDistanceMin);
      DefaultDataWriteUtility.Write(writer, "IdleReplicsDistanceMax", this.IdleReplicsDistanceMax);
      DefaultDataWriteUtility.Write(writer, "EntitySubtitlesDistanceMax", this.EntitySubtitlesDistanceMax);
      DefaultDataWriteUtility.Write(writer, "BleuprintSubtitlesDistanceMax", this.BleuprintSubtitlesDistanceMax);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.Version = DefaultDataReadUtility.Read(reader, "Version", this.Version);
      this.DropBagSearchRadius = DefaultDataReadUtility.Read(reader, "DropBagSearchRadius", this.DropBagSearchRadius);
      this.DropBagOffset = DefaultDataReadUtility.Read(reader, "DropBagOffset", this.DropBagOffset);
      this.ChildGunJam = DefaultDataReadUtility.Read(reader, "ChildGunJam", this.ChildGunJam);
      this.StepsDistance = DefaultDataReadUtility.Read(reader, "StepsDistance", this.StepsDistance);
      this.InteractionDistance = DefaultDataReadUtility.Read(reader, "InteractionDistance", this.InteractionDistance);
      this.AwayDistance = DefaultDataReadUtility.Read(reader, "AwayDistance", this.AwayDistance);
      this.DangerDistance = DefaultDataReadUtility.Read(reader, "DangerDistance", this.DangerDistance);
      this.DoorSpeed = DefaultDataReadUtility.Read(reader, "DoorSpeed", this.DoorSpeed);
      this.IdleReplicsFrequencyMin = DefaultDataReadUtility.Read(reader, "IdleReplicsFrequencyMin", this.IdleReplicsFrequencyMin);
      this.IdleReplicsFrequencyMax = DefaultDataReadUtility.Read(reader, "IdleReplicsFrequencyMax", this.IdleReplicsFrequencyMax);
      this.IdleReplicsMaxRangeToPlayer = DefaultDataReadUtility.Read(reader, "IdleReplicsMaxRangeToPlayer", this.IdleReplicsMaxRangeToPlayer);
      this.IdleReplicsDistanceMin = DefaultDataReadUtility.Read(reader, "IdleReplicsDistanceMin", this.IdleReplicsDistanceMin);
      this.IdleReplicsDistanceMax = DefaultDataReadUtility.Read(reader, "IdleReplicsDistanceMax", this.IdleReplicsDistanceMax);
      this.EntitySubtitlesDistanceMax = DefaultDataReadUtility.Read(reader, "EntitySubtitlesDistanceMax", this.EntitySubtitlesDistanceMax);
      this.BleuprintSubtitlesDistanceMax = DefaultDataReadUtility.Read(reader, "BleuprintSubtitlesDistanceMax", this.BleuprintSubtitlesDistanceMax);
    }
  }
}

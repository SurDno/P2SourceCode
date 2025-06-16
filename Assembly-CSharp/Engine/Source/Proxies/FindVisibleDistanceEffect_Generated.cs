// Decompiled with JetBrains decompiler
// Type: Engine.Source.Proxies.FindVisibleDistanceEffect_Generated
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Components.Parameters;
using Engine.Source.Commons.Abilities;
using Engine.Source.Commons.Effects;
using Engine.Source.Effects;
using System;

#nullable disable
namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (FindVisibleDistanceEffect))]
  public class FindVisibleDistanceEffect_Generated : 
    FindVisibleDistanceEffect,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      FindVisibleDistanceEffect_Generated instance = Activator.CreateInstance<FindVisibleDistanceEffect_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      FindVisibleDistanceEffect_Generated distanceEffectGenerated = (FindVisibleDistanceEffect_Generated) target2;
      distanceEffectGenerated.queue = this.queue;
      distanceEffectGenerated.enable = this.enable;
      distanceEffectGenerated.durationType = this.durationType;
      distanceEffectGenerated.realTime = this.realTime;
      distanceEffectGenerated.duration = this.duration;
      distanceEffectGenerated.interval = this.interval;
      distanceEffectGenerated.VisibileDistanceParameterName = this.VisibileDistanceParameterName;
      distanceEffectGenerated.FlashlightOnParameterName = this.FlashlightOnParameterName;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteEnum<ParameterEffectQueueEnum>(writer, "Queue", this.queue);
      DefaultDataWriteUtility.Write(writer, "Enable", this.enable);
      DefaultDataWriteUtility.WriteEnum<DurationTypeEnum>(writer, "DurationType", this.durationType);
      DefaultDataWriteUtility.Write(writer, "RealTime", this.realTime);
      DefaultDataWriteUtility.Write(writer, "Duration", this.duration);
      DefaultDataWriteUtility.Write(writer, "Interval", this.interval);
      DefaultDataWriteUtility.WriteEnum<ParameterNameEnum>(writer, "VisibileDistanceParameterName", this.VisibileDistanceParameterName);
      DefaultDataWriteUtility.WriteEnum<ParameterNameEnum>(writer, "FlashlightOnParameterName", this.FlashlightOnParameterName);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.queue = DefaultDataReadUtility.ReadEnum<ParameterEffectQueueEnum>(reader, "Queue");
      this.enable = DefaultDataReadUtility.Read(reader, "Enable", this.enable);
      this.durationType = DefaultDataReadUtility.ReadEnum<DurationTypeEnum>(reader, "DurationType");
      this.realTime = DefaultDataReadUtility.Read(reader, "RealTime", this.realTime);
      this.duration = DefaultDataReadUtility.Read(reader, "Duration", this.duration);
      this.interval = DefaultDataReadUtility.Read(reader, "Interval", this.interval);
      this.VisibileDistanceParameterName = DefaultDataReadUtility.ReadEnum<ParameterNameEnum>(reader, "VisibileDistanceParameterName");
      this.FlashlightOnParameterName = DefaultDataReadUtility.ReadEnum<ParameterNameEnum>(reader, "FlashlightOnParameterName");
    }
  }
}

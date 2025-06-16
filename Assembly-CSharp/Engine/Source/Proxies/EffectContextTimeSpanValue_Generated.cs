// Decompiled with JetBrains decompiler
// Type: Engine.Source.Proxies.EffectContextTimeSpanValue_Generated
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Components.Parameters;
using Engine.Source.Effects.Engine;
using System;

#nullable disable
namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (EffectContextTimeSpanValue))]
  public class EffectContextTimeSpanValue_Generated : 
    EffectContextTimeSpanValue,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      EffectContextTimeSpanValue_Generated instance = Activator.CreateInstance<EffectContextTimeSpanValue_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      EffectContextTimeSpanValue_Generated spanValueGenerated = (EffectContextTimeSpanValue_Generated) target2;
      spanValueGenerated.effectContext = this.effectContext;
      spanValueGenerated.parameterName = this.parameterName;
      spanValueGenerated.parameterData = this.parameterData;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteEnum<EffectContextEnum>(writer, "EffectContext", this.effectContext);
      DefaultDataWriteUtility.WriteEnum<ParameterNameEnum>(writer, "ParameterName", this.parameterName);
      DefaultDataWriteUtility.WriteEnum<ParameterDataEnum>(writer, "ParameterData", this.parameterData);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.effectContext = DefaultDataReadUtility.ReadEnum<EffectContextEnum>(reader, "EffectContext");
      this.parameterName = DefaultDataReadUtility.ReadEnum<ParameterNameEnum>(reader, "ParameterName");
      this.parameterData = DefaultDataReadUtility.ReadEnum<ParameterDataEnum>(reader, "ParameterData");
    }
  }
}

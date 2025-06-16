// Decompiled with JetBrains decompiler
// Type: Engine.Source.Proxies.ResetableDetectTypeParameter_Generated
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Components.Detectors;
using Engine.Common.Components.Parameters;
using Engine.Source.Commons;
using Engine.Source.Commons.Parameters;
using System;

#nullable disable
namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (ResetableDetectTypeParameter))]
  public class ResetableDetectTypeParameter_Generated : 
    ResetableDetectTypeParameter,
    IComputeNeedSave,
    INeedSave,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead,
    ISerializeStateSave,
    ISerializeStateLoad
  {
    public bool NeedSave { get; private set; } = true;

    public void ComputeNeedSave(object target2)
    {
      this.NeedSave = true;
      ResetableDetectTypeParameter_Generated parameterGenerated = (ResetableDetectTypeParameter_Generated) target2;
      if (parameterGenerated.name != this.name || parameterGenerated.value != this.value || parameterGenerated.baseValue != this.baseValue)
        return;
      this.NeedSave = false;
    }

    public object Clone()
    {
      ResetableDetectTypeParameter_Generated instance = Activator.CreateInstance<ResetableDetectTypeParameter_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      ResetableDetectTypeParameter_Generated parameterGenerated = (ResetableDetectTypeParameter_Generated) target2;
      parameterGenerated.name = this.name;
      parameterGenerated.value = this.value;
      parameterGenerated.baseValue = this.baseValue;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteEnum<ParameterNameEnum>(writer, "Name", this.name);
      DefaultDataWriteUtility.WriteEnum<DetectType>(writer, "Value", this.value);
      DefaultDataWriteUtility.WriteEnum<DetectType>(writer, "BaseValue", this.baseValue);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.name = DefaultDataReadUtility.ReadEnum<ParameterNameEnum>(reader, "Name");
      this.value = DefaultDataReadUtility.ReadEnum<DetectType>(reader, "Value");
      this.baseValue = DefaultDataReadUtility.ReadEnum<DetectType>(reader, "BaseValue");
    }

    public void StateSave(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteEnum<ParameterNameEnum>(writer, "Name", this.name);
      DefaultDataWriteUtility.WriteEnum<DetectType>(writer, "Value", this.value);
      DefaultDataWriteUtility.WriteEnum<DetectType>(writer, "BaseValue", this.baseValue);
    }

    public void StateLoad(IDataReader reader, Type type)
    {
      this.value = DefaultDataReadUtility.ReadEnum<DetectType>(reader, "Value");
      this.baseValue = DefaultDataReadUtility.ReadEnum<DetectType>(reader, "BaseValue");
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Engine.Source.Proxies.ResetableIntParameter_Generated
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Components.Parameters;
using Engine.Source.Commons;
using Engine.Source.Commons.Parameters;
using System;

#nullable disable
namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (ResetableIntParameter))]
  public class ResetableIntParameter_Generated : 
    ResetableIntParameter,
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
      ResetableIntParameter_Generated parameterGenerated = (ResetableIntParameter_Generated) target2;
      if (parameterGenerated.name != this.name || parameterGenerated.value != this.value || parameterGenerated.baseValue != this.baseValue || parameterGenerated.minValue != this.minValue || parameterGenerated.maxValue != this.maxValue)
        return;
      this.NeedSave = false;
    }

    public object Clone()
    {
      ResetableIntParameter_Generated instance = Activator.CreateInstance<ResetableIntParameter_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      ResetableIntParameter_Generated parameterGenerated = (ResetableIntParameter_Generated) target2;
      parameterGenerated.name = this.name;
      parameterGenerated.value = this.value;
      parameterGenerated.baseValue = this.baseValue;
      parameterGenerated.minValue = this.minValue;
      parameterGenerated.maxValue = this.maxValue;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteEnum<ParameterNameEnum>(writer, "Name", this.name);
      DefaultDataWriteUtility.Write(writer, "Value", this.value);
      DefaultDataWriteUtility.Write(writer, "BaseValue", this.baseValue);
      DefaultDataWriteUtility.Write(writer, "MinValue", this.minValue);
      DefaultDataWriteUtility.Write(writer, "MaxValue", this.maxValue);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.name = DefaultDataReadUtility.ReadEnum<ParameterNameEnum>(reader, "Name");
      this.value = DefaultDataReadUtility.Read(reader, "Value", this.value);
      this.baseValue = DefaultDataReadUtility.Read(reader, "BaseValue", this.baseValue);
      this.minValue = DefaultDataReadUtility.Read(reader, "MinValue", this.minValue);
      this.maxValue = DefaultDataReadUtility.Read(reader, "MaxValue", this.maxValue);
    }

    public void StateSave(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteEnum<ParameterNameEnum>(writer, "Name", this.name);
      DefaultDataWriteUtility.Write(writer, "Value", this.value);
      DefaultDataWriteUtility.Write(writer, "BaseValue", this.baseValue);
      DefaultDataWriteUtility.Write(writer, "MinValue", this.minValue);
      DefaultDataWriteUtility.Write(writer, "MaxValue", this.maxValue);
    }

    public void StateLoad(IDataReader reader, Type type)
    {
      this.value = DefaultDataReadUtility.Read(reader, "Value", this.value);
      this.baseValue = DefaultDataReadUtility.Read(reader, "BaseValue", this.baseValue);
      this.minValue = DefaultDataReadUtility.Read(reader, "MinValue", this.minValue);
      this.maxValue = DefaultDataReadUtility.Read(reader, "MaxValue", this.maxValue);
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Engine.Source.Proxies.LiquidTypeParameter_Generated
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
  [FactoryProxy(typeof (LiquidTypeParameter))]
  public class LiquidTypeParameter_Generated : 
    LiquidTypeParameter,
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
      LiquidTypeParameter_Generated parameterGenerated = (LiquidTypeParameter_Generated) target2;
      if (parameterGenerated.name != this.name || parameterGenerated.value != this.value)
        return;
      this.NeedSave = false;
    }

    public object Clone()
    {
      LiquidTypeParameter_Generated instance = Activator.CreateInstance<LiquidTypeParameter_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      LiquidTypeParameter_Generated parameterGenerated = (LiquidTypeParameter_Generated) target2;
      parameterGenerated.name = this.name;
      parameterGenerated.value = this.value;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteEnum<ParameterNameEnum>(writer, "Name", this.name);
      DefaultDataWriteUtility.WriteEnum<LiquidTypeEnum>(writer, "Value", this.value);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.name = DefaultDataReadUtility.ReadEnum<ParameterNameEnum>(reader, "Name");
      this.value = DefaultDataReadUtility.ReadEnum<LiquidTypeEnum>(reader, "Value");
    }

    public void StateSave(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteEnum<ParameterNameEnum>(writer, "Name", this.name);
      DefaultDataWriteUtility.WriteEnum<LiquidTypeEnum>(writer, "Value", this.value);
    }

    public void StateLoad(IDataReader reader, Type type)
    {
      this.value = DefaultDataReadUtility.ReadEnum<LiquidTypeEnum>(reader, "Value");
    }
  }
}

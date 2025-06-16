// Decompiled with JetBrains decompiler
// Type: Engine.Source.Proxies.ConditionTimeSpanOperation_Generated
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Expressions;
using System;

#nullable disable
namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (ConditionTimeSpanOperation))]
  public class ConditionTimeSpanOperation_Generated : 
    ConditionTimeSpanOperation,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      ConditionTimeSpanOperation_Generated instance = Activator.CreateInstance<ConditionTimeSpanOperation_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      ConditionTimeSpanOperation_Generated operationGenerated = (ConditionTimeSpanOperation_Generated) target2;
      operationGenerated.condition = CloneableObjectUtility.Clone<IValue<bool>>(this.condition);
      operationGenerated.trueResult = CloneableObjectUtility.Clone<IValue<TimeSpan>>(this.trueResult);
      operationGenerated.falseResult = CloneableObjectUtility.Clone<IValue<TimeSpan>>(this.falseResult);
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize<IValue<bool>>(writer, "Condition", this.condition);
      DefaultDataWriteUtility.WriteSerialize<IValue<TimeSpan>>(writer, "True", this.trueResult);
      DefaultDataWriteUtility.WriteSerialize<IValue<TimeSpan>>(writer, "False", this.falseResult);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.condition = DefaultDataReadUtility.ReadSerialize<IValue<bool>>(reader, "Condition");
      this.trueResult = DefaultDataReadUtility.ReadSerialize<IValue<TimeSpan>>(reader, "True");
      this.falseResult = DefaultDataReadUtility.ReadSerialize<IValue<TimeSpan>>(reader, "False");
    }
  }
}

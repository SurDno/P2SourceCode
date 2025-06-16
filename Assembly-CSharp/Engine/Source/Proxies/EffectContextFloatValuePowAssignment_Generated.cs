// Decompiled with JetBrains decompiler
// Type: Engine.Source.Proxies.EffectContextFloatValuePowAssignment_Generated
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Effects.Engine;
using Expressions;
using System;

#nullable disable
namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (EffectContextFloatValuePowAssignment))]
  public class EffectContextFloatValuePowAssignment_Generated : 
    EffectContextFloatValuePowAssignment,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      EffectContextFloatValuePowAssignment_Generated instance = Activator.CreateInstance<EffectContextFloatValuePowAssignment_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      EffectContextFloatValuePowAssignment_Generated assignmentGenerated = (EffectContextFloatValuePowAssignment_Generated) target2;
      assignmentGenerated.a = CloneableObjectUtility.Clone<IValueSetter<float>>(this.a);
      assignmentGenerated.b = CloneableObjectUtility.Clone<IValue<float>>(this.b);
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize<IValueSetter<float>>(writer, "A", this.a);
      DefaultDataWriteUtility.WriteSerialize<IValue<float>>(writer, "Source", this.b);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.a = DefaultDataReadUtility.ReadSerialize<IValueSetter<float>>(reader, "A");
      this.b = DefaultDataReadUtility.ReadSerialize<IValue<float>>(reader, "Source");
    }
  }
}

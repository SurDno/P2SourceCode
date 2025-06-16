// Decompiled with JetBrains decompiler
// Type: Engine.Source.Proxies.EffectContextIntValueSubtractionAssignment_Generated
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
  [FactoryProxy(typeof (EffectContextIntValueSubtractionAssignment))]
  public class EffectContextIntValueSubtractionAssignment_Generated : 
    EffectContextIntValueSubtractionAssignment,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      EffectContextIntValueSubtractionAssignment_Generated instance = Activator.CreateInstance<EffectContextIntValueSubtractionAssignment_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      EffectContextIntValueSubtractionAssignment_Generated assignmentGenerated = (EffectContextIntValueSubtractionAssignment_Generated) target2;
      assignmentGenerated.a = CloneableObjectUtility.Clone<IValueSetter<int>>(this.a);
      assignmentGenerated.b = CloneableObjectUtility.Clone<IValue<int>>(this.b);
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize<IValueSetter<int>>(writer, "A", this.a);
      DefaultDataWriteUtility.WriteSerialize<IValue<int>>(writer, "Source", this.b);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.a = DefaultDataReadUtility.ReadSerialize<IValueSetter<int>>(reader, "A");
      this.b = DefaultDataReadUtility.ReadSerialize<IValue<int>>(reader, "Source");
    }
  }
}

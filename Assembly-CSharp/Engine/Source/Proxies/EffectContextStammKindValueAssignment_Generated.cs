// Decompiled with JetBrains decompiler
// Type: Engine.Source.Proxies.EffectContextStammKindValueAssignment_Generated
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Effects.Engine;
using Expressions;
using System;

#nullable disable
namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (EffectContextStammKindValueAssignment))]
  public class EffectContextStammKindValueAssignment_Generated : 
    EffectContextStammKindValueAssignment,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      EffectContextStammKindValueAssignment_Generated instance = Activator.CreateInstance<EffectContextStammKindValueAssignment_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      EffectContextStammKindValueAssignment_Generated assignmentGenerated = (EffectContextStammKindValueAssignment_Generated) target2;
      assignmentGenerated.a = CloneableObjectUtility.Clone<IValueSetter<StammKind>>(this.a);
      assignmentGenerated.b = CloneableObjectUtility.Clone<IValue<StammKind>>(this.b);
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize<IValueSetter<StammKind>>(writer, "A", this.a);
      DefaultDataWriteUtility.WriteSerialize<IValue<StammKind>>(writer, "Source", this.b);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.a = DefaultDataReadUtility.ReadSerialize<IValueSetter<StammKind>>(reader, "A");
      this.b = DefaultDataReadUtility.ReadSerialize<IValue<StammKind>>(reader, "Source");
    }
  }
}

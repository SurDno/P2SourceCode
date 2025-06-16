// Decompiled with JetBrains decompiler
// Type: Engine.Source.Proxies.EffectContextBoolValueAssignment_Generated
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
  [FactoryProxy(typeof (EffectContextBoolValueAssignment))]
  public class EffectContextBoolValueAssignment_Generated : 
    EffectContextBoolValueAssignment,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      EffectContextBoolValueAssignment_Generated instance = Activator.CreateInstance<EffectContextBoolValueAssignment_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      EffectContextBoolValueAssignment_Generated assignmentGenerated = (EffectContextBoolValueAssignment_Generated) target2;
      assignmentGenerated.a = CloneableObjectUtility.Clone<IValueSetter<bool>>(this.a);
      assignmentGenerated.b = CloneableObjectUtility.Clone<IValue<bool>>(this.b);
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize<IValueSetter<bool>>(writer, "A", this.a);
      DefaultDataWriteUtility.WriteSerialize<IValue<bool>>(writer, "Source", this.b);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.a = DefaultDataReadUtility.ReadSerialize<IValueSetter<bool>>(reader, "A");
      this.b = DefaultDataReadUtility.ReadSerialize<IValue<bool>>(reader, "Source");
    }
  }
}

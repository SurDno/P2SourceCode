// Decompiled with JetBrains decompiler
// Type: Engine.Source.Proxies.NotEqualStammKindOperations_Generated
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Expressions;
using System;

#nullable disable
namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (NotEqualStammKindOperations))]
  public class NotEqualStammKindOperations_Generated : 
    NotEqualStammKindOperations,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      NotEqualStammKindOperations_Generated instance = Activator.CreateInstance<NotEqualStammKindOperations_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      NotEqualStammKindOperations_Generated operationsGenerated = (NotEqualStammKindOperations_Generated) target2;
      operationsGenerated.a = CloneableObjectUtility.Clone<IValue<StammKind>>(this.a);
      operationsGenerated.b = CloneableObjectUtility.Clone<IValue<StammKind>>(this.b);
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize<IValue<StammKind>>(writer, "Left", this.a);
      DefaultDataWriteUtility.WriteSerialize<IValue<StammKind>>(writer, "Right", this.b);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.a = DefaultDataReadUtility.ReadSerialize<IValue<StammKind>>(reader, "Left");
      this.b = DefaultDataReadUtility.ReadSerialize<IValue<StammKind>>(reader, "Right");
    }
  }
}

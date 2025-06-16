// Decompiled with JetBrains decompiler
// Type: Engine.Source.Proxies.EqualBlockTypeOperations_Generated
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
  [FactoryProxy(typeof (EqualBlockTypeOperations))]
  public class EqualBlockTypeOperations_Generated : 
    EqualBlockTypeOperations,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      EqualBlockTypeOperations_Generated instance = Activator.CreateInstance<EqualBlockTypeOperations_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      EqualBlockTypeOperations_Generated operationsGenerated = (EqualBlockTypeOperations_Generated) target2;
      operationsGenerated.a = CloneableObjectUtility.Clone<IValue<BlockTypeEnum>>(this.a);
      operationsGenerated.b = CloneableObjectUtility.Clone<IValue<BlockTypeEnum>>(this.b);
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize<IValue<BlockTypeEnum>>(writer, "Left", this.a);
      DefaultDataWriteUtility.WriteSerialize<IValue<BlockTypeEnum>>(writer, "Right", this.b);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.a = DefaultDataReadUtility.ReadSerialize<IValue<BlockTypeEnum>>(reader, "Left");
      this.b = DefaultDataReadUtility.ReadSerialize<IValue<BlockTypeEnum>>(reader, "Right");
    }
  }
}

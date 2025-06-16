// Decompiled with JetBrains decompiler
// Type: Engine.Source.Proxies.PlusIntPolyOperation_Generated
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
  [FactoryProxy(typeof (PlusIntPolyOperation))]
  public class PlusIntPolyOperation_Generated : 
    PlusIntPolyOperation,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      PlusIntPolyOperation_Generated instance = Activator.CreateInstance<PlusIntPolyOperation_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      CloneableObjectUtility.CopyListTo<IValue<int>>(((PolyOperation<int>) target2).values, this.values);
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteListSerialize<IValue<int>>(writer, "Parameters", this.values);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.values = DefaultDataReadUtility.ReadListSerialize<IValue<int>>(reader, "Parameters", this.values);
    }
  }
}

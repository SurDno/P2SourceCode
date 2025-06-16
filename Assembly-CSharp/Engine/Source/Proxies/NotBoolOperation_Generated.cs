// Decompiled with JetBrains decompiler
// Type: Engine.Source.Proxies.NotBoolOperation_Generated
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
  [FactoryProxy(typeof (NotBoolOperation))]
  public class NotBoolOperation_Generated : 
    NotBoolOperation,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      NotBoolOperation_Generated instance = Activator.CreateInstance<NotBoolOperation_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      ((UnaryOperation<bool>) target2).value = CloneableObjectUtility.Clone<IValue<bool>>(this.value);
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize<IValue<bool>>(writer, "Value", this.value);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.value = DefaultDataReadUtility.ReadSerialize<IValue<bool>>(reader, "Value");
    }
  }
}

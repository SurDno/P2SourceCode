// Decompiled with JetBrains decompiler
// Type: BehaviorDesigner.Runtime.SharedFloat
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using System;

#nullable disable
namespace BehaviorDesigner.Runtime
{
  [FactoryProxy(typeof (SharedFloat))]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  [Serializable]
  public class SharedFloat : SharedVariable<float>, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "IsShared", this.mIsShared);
      DefaultDataWriteUtility.Write(writer, "Name", this.mName);
      DefaultDataWriteUtility.Write(writer, "Value", this.mValue);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.mIsShared = DefaultDataReadUtility.Read(reader, "IsShared", this.mIsShared);
      this.mName = DefaultDataReadUtility.Read(reader, "Name", this.mName);
      this.mValue = DefaultDataReadUtility.Read(reader, "Value", this.mValue);
    }

    public static implicit operator SharedFloat(float value)
    {
      SharedFloat sharedFloat = new SharedFloat();
      sharedFloat.Value = value;
      return sharedFloat;
    }
  }
}

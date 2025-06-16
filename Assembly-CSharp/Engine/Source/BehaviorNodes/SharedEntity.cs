// Decompiled with JetBrains decompiler
// Type: Engine.Source.BehaviorNodes.SharedEntity
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using BehaviorDesigner.Runtime;
using Cofe.Proxies;
using Cofe.Serializations.Converters;
using Cofe.Serializations.Data;
using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Common.Services;
using Engine.Impl.Services.Factories;
using System;

#nullable disable
namespace Engine.Source.BehaviorNodes
{
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  [FactoryProxy(typeof (SharedEntity))]
  [Serializable]
  public class SharedEntity : SharedVariable<string>, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    public IEntity Entity
    {
      get
      {
        if (this.mValue == null)
          return (IEntity) null;
        Guid guid = DefaultConverter.ParseGuid(this.mValue);
        return ServiceLocator.GetService<ISimulation>().Get(guid);
      }
      set => this.mValue = value != null ? value.Id.ToString() : "";
    }

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
  }
}

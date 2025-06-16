// Decompiled with JetBrains decompiler
// Type: BehaviorDesigner.Runtime.SharedTransformList
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Scripts.Tools.Serializations.Converters;
using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace BehaviorDesigner.Runtime
{
  [FactoryProxy(typeof (SharedTransformList))]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  [Serializable]
  public class SharedTransformList : 
    SharedVariable<List<Transform>>,
    IStub,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "IsShared", this.mIsShared);
      DefaultDataWriteUtility.Write(writer, "Name", this.mName);
      BehaviorTreeDataWriteUtility.WriteUnityList<Transform>(writer, "Value", this.mValue);
    }

    public void DataRead(IDataReader reader, System.Type type)
    {
      this.mIsShared = DefaultDataReadUtility.Read(reader, "IsShared", this.mIsShared);
      this.mName = DefaultDataReadUtility.Read(reader, "Name", this.mName);
      this.mValue = BehaviorTreeDataReadUtility.ReadUnityList<Transform>(reader, "Value", this.mValue);
    }

    public static implicit operator SharedTransformList(List<Transform> value)
    {
      SharedTransformList sharedTransformList = new SharedTransformList();
      sharedTransformList.mValue = value;
      return sharedTransformList;
    }
  }
}

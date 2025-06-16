// Decompiled with JetBrains decompiler
// Type: BehaviorDesigner.Runtime.BehaviorSourceData
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using BehaviorDesigner.Runtime.Tasks;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Scripts.Tools.Serializations.Converters;
using System;
using System.Collections.Generic;

#nullable disable
namespace BehaviorDesigner.Runtime
{
  [FactoryProxy(typeof (BehaviorSourceData))]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class BehaviorSourceData : IStub, ISerializeDataWrite, ISerializeDataRead
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    public Task EntryTask;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    public Task RootTask;
    [DataReadProxy(MemberEnum.CustomCommonSharedList)]
    [DataWriteProxy(MemberEnum.CustomCommonSharedList)]
    [CopyableProxy(MemberEnum.None)]
    public List<SharedVariable> Variables;

    public void DataWrite(IDataWriter writer)
    {
      BehaviorTreeDataWriteUtility.WriteTask<Task>(writer, "EntryTask", this.EntryTask);
      BehaviorTreeDataWriteUtility.WriteTask<Task>(writer, "RootTask", this.RootTask);
      BehaviorTreeDataWriteUtility.WriteCommonSharedList<SharedVariable>(writer, "Variables", this.Variables);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.EntryTask = BehaviorTreeDataReadUtility.ReadTask<Task>(reader, "EntryTask", this.EntryTask);
      this.RootTask = BehaviorTreeDataReadUtility.ReadTask<Task>(reader, "RootTask", this.RootTask);
      this.Variables = BehaviorTreeDataReadUtility.ReadCommonSharedList<SharedVariable>(reader, "Variables", this.Variables);
    }
  }
}

using BehaviorDesigner.Runtime.Tasks;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Scripts.Tools.Serializations.Converters;
using System;
using System.Collections.Generic;

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

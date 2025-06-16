using System;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Scripts.Tools.Serializations.Converters;

namespace BehaviorDesigner.Runtime;

[FactoryProxy(typeof(BehaviorSourceData))]
[Factory]
[GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
public class BehaviorSourceData : IStub, ISerializeDataWrite, ISerializeDataRead {
	[DataReadProxy] [DataWriteProxy] [CopyableProxy]
	public Task EntryTask;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy]
	public Task RootTask;

	[DataReadProxy(MemberEnum.CustomCommonSharedList)]
	[DataWriteProxy(MemberEnum.CustomCommonSharedList)]
	[CopyableProxy()]
	public List<SharedVariable> Variables;

	public void DataWrite(IDataWriter writer) {
		BehaviorTreeDataWriteUtility.WriteTask(writer, "EntryTask", EntryTask);
		BehaviorTreeDataWriteUtility.WriteTask(writer, "RootTask", RootTask);
		BehaviorTreeDataWriteUtility.WriteCommonSharedList(writer, "Variables", Variables);
	}

	public void DataRead(IDataReader reader, Type type) {
		EntryTask = BehaviorTreeDataReadUtility.ReadTask(reader, "EntryTask", EntryTask);
		RootTask = BehaviorTreeDataReadUtility.ReadTask(reader, "RootTask", RootTask);
		Variables = BehaviorTreeDataReadUtility.ReadCommonSharedList(reader, "Variables", Variables);
	}
}
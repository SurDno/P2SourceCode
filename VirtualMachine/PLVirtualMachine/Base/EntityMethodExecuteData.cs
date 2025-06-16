using System;
using System.Reflection;
using PLVirtualMachine.Common.EngineAPI.VMECS;
using PLVirtualMachine.Dynamic;

namespace PLVirtualMachine.Base;

public class EntityMethodExecuteData {
	public void Initialize(
		VMEntity entity,
		VMComponent comp,
		MethodInfo methodInfo,
		object[] dParams) {
		TargetEntity = entity;
		TargetComponent = comp;
		ExecMethodInfo = methodInfo;
		InputParams = dParams;
	}

	public VMEntity TargetEntity { get; private set; }

	public Guid TargetEntityGuid => TargetEntity.EngineGuid;

	public MethodInfo ExecMethodInfo { get; private set; }

	public object[] InputParams { get; private set; }

	public VMComponent TargetComponent { get; private set; }

	public void Clear() {
		TargetEntity = null;
	}
}
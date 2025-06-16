using PLVirtualMachine.Common.EngineAPI.VMECS;
using PLVirtualMachine.Dynamic;
using System;
using System.Reflection;

namespace PLVirtualMachine.Base
{
  public class EntityMethodExecuteData
  {
    public void Initialize(
      VMEntity entity,
      VMComponent comp,
      MethodInfo methodInfo,
      object[] dParams)
    {
      this.TargetEntity = entity;
      this.TargetComponent = comp;
      this.ExecMethodInfo = methodInfo;
      this.InputParams = dParams;
    }

    public VMEntity TargetEntity { get; private set; }

    public Guid TargetEntityGuid => this.TargetEntity.EngineGuid;

    public MethodInfo ExecMethodInfo { get; private set; }

    public object[] InputParams { get; private set; }

    public VMComponent TargetComponent { get; private set; }

    public void Clear() => this.TargetEntity = (VMEntity) null;
  }
}

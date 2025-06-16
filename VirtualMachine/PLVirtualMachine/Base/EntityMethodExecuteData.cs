// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Base.EntityMethodExecuteData
// Assembly: VirtualMachine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4141C12C-9CB3-4BEE-B86E-276A0762C9CD
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.dll

using PLVirtualMachine.Common.EngineAPI.VMECS;
using PLVirtualMachine.Dynamic;
using System;
using System.Reflection;

#nullable disable
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

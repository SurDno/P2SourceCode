// Decompiled with JetBrains decompiler
// Type: BehaviorDesigner.Runtime.Tasks.ReturnFailure
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

#nullable disable
namespace BehaviorDesigner.Runtime.Tasks
{
  [FactoryProxy(typeof (ReturnFailure))]
  [TaskDescription("The return failure task will always return failure except when the child task is running.")]
  [HelpURL("http://www.opsive.com/assets/BehaviorDesigner/documentation.php?id=38")]
  [TaskIcon("{SkinColor}ReturnFailureIcon.png")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class ReturnFailure : Decorator, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    private TaskStatus executionStatus = TaskStatus.Inactive;

    public new void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize<NodeData>(writer, "NodeData", this.nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", this.id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", this.friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", this.instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", this.disabled);
      BehaviorTreeDataWriteUtility.WriteTaskList<Task>(writer, "Children", this.children);
    }

    public new void DataRead(IDataReader reader, Type type)
    {
      this.nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      this.id = DefaultDataReadUtility.Read(reader, "Id", this.id);
      this.friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", this.friendlyName);
      this.instant = DefaultDataReadUtility.Read(reader, "Instant", this.instant);
      this.disabled = DefaultDataReadUtility.Read(reader, "Disabled", this.disabled);
      this.children = BehaviorTreeDataReadUtility.ReadTaskList<Task>(reader, "Children", this.children);
    }

    public override bool CanExecute()
    {
      return this.executionStatus == TaskStatus.Inactive || this.executionStatus == TaskStatus.Running;
    }

    public override void OnChildExecuted(TaskStatus childStatus)
    {
      this.executionStatus = childStatus;
    }

    public override TaskStatus Decorate(TaskStatus status)
    {
      return status == TaskStatus.Success ? TaskStatus.Failure : status;
    }

    public override void OnEnd() => this.executionStatus = TaskStatus.Inactive;
  }
}

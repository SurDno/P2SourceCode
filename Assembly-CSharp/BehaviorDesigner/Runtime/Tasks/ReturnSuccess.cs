﻿using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Scripts.Tools.Serializations.Converters;

namespace BehaviorDesigner.Runtime.Tasks
{
  [FactoryProxy(typeof (ReturnSuccess))]
  [TaskDescription("The return success task will always return success except when the child task is running.")]
  [HelpURL("http://www.opsive.com/assets/BehaviorDesigner/documentation.php?id=39")]
  [TaskIcon("{SkinColor}ReturnSuccessIcon.png")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class ReturnSuccess : Decorator, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    private TaskStatus executionStatus = TaskStatus.Inactive;

    public new void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize(writer, "NodeData", nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", disabled);
      BehaviorTreeDataWriteUtility.WriteTaskList(writer, "Children", children);
    }

    public new void DataRead(IDataReader reader, Type type)
    {
      nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      id = DefaultDataReadUtility.Read(reader, "Id", id);
      friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", friendlyName);
      instant = DefaultDataReadUtility.Read(reader, "Instant", instant);
      disabled = DefaultDataReadUtility.Read(reader, "Disabled", disabled);
      children = BehaviorTreeDataReadUtility.ReadTaskList(reader, "Children", children);
    }

    public override bool CanExecute()
    {
      return executionStatus == TaskStatus.Inactive || executionStatus == TaskStatus.Running;
    }

    public override void OnChildExecuted(TaskStatus childStatus)
    {
      executionStatus = childStatus;
    }

    public override TaskStatus Decorate(TaskStatus status)
    {
      return status == TaskStatus.Failure ? TaskStatus.Success : status;
    }

    public override void OnEnd() => executionStatus = TaskStatus.Inactive;
  }
}

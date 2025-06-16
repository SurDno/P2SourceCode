using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Common.Services;
using Engine.Impl.Services.Factories;
using Engine.Source.Services;
using Scripts.Tools.Serializations.Converters;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Pathologic
{
  [TaskDescription("Sequence that locks Group Point")]
  [TaskIcon("{SkinColor}SequenceIcon.png")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  [FactoryProxy(typeof (GetGroupPointTransform))]
  public class GetGroupPointTransform : Composite, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedTransform GroupPointTransform;
    private GroupPoint point;
    private int currentChildIndex = 0;
    protected TaskStatus executionStatus = TaskStatus.Inactive;

    public override int CurrentChildIndex() => this.currentChildIndex;

    public override bool CanExecute()
    {
      return this.currentChildIndex < this.children.Count && this.executionStatus != TaskStatus.Failure;
    }

    public override void OnChildExecuted(TaskStatus childStatus)
    {
      ++this.currentChildIndex;
      this.executionStatus = childStatus;
    }

    public override void OnConditionalAbort(int childIndex)
    {
      this.currentChildIndex = childIndex;
      this.executionStatus = TaskStatus.Inactive;
    }

    public override void OnStart()
    {
      GroupPointsService service = ServiceLocator.GetService<GroupPointsService>();
      if ((UnityEngine.Object) this.point != (UnityEngine.Object) null)
      {
        service.AddPoint(this.point);
        this.point = (GroupPoint) null;
      }
      this.point = service.GetFreePoint();
      if (!((UnityEngine.Object) this.point != (UnityEngine.Object) null))
        return;
      this.GroupPointTransform.Value = this.point.transform;
    }

    public override TaskStatus OnUpdate()
    {
      return !(bool) (UnityEngine.Object) this.GroupPointTransform.Value ? TaskStatus.Failure : TaskStatus.Success;
    }

    public override void OnEnd()
    {
      this.executionStatus = TaskStatus.Inactive;
      this.currentChildIndex = 0;
      if (!((UnityEngine.Object) this.point != (UnityEngine.Object) null))
        return;
      ServiceLocator.GetService<GroupPointsService>().AddPoint(this.point);
      this.point = (GroupPoint) null;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize<NodeData>(writer, "NodeData", this.nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", this.id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", this.friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", this.instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", this.disabled);
      BehaviorTreeDataWriteUtility.WriteTaskList<Task>(writer, "Children", this.children);
      DefaultDataWriteUtility.WriteEnum<AbortType>(writer, "AbortType", this.abortType);
      BehaviorTreeDataWriteUtility.WriteShared<SharedTransform>(writer, "GroupPointTransform", this.GroupPointTransform);
    }

    public void DataRead(IDataReader reader, System.Type type)
    {
      this.nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      this.id = DefaultDataReadUtility.Read(reader, "Id", this.id);
      this.friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", this.friendlyName);
      this.instant = DefaultDataReadUtility.Read(reader, "Instant", this.instant);
      this.disabled = DefaultDataReadUtility.Read(reader, "Disabled", this.disabled);
      this.children = BehaviorTreeDataReadUtility.ReadTaskList<Task>(reader, "Children", this.children);
      this.abortType = DefaultDataReadUtility.ReadEnum<AbortType>(reader, "AbortType");
      this.GroupPointTransform = BehaviorTreeDataReadUtility.ReadShared<SharedTransform>(reader, "GroupPointTransform", this.GroupPointTransform);
    }
  }
}

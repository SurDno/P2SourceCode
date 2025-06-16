using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Services;
using FlowCanvas;
using Scripts.Tools.Serializations.Converters;
using System.Reflection;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
  [TaskDescription("Запустить блюпринт на N секунд. На старте отсылается ивент Strt, на выходе End")]
  [TaskIcon("{SkinColor}WaitIcon.png")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  [FactoryProxy(typeof (RunBlueprint))]
  public class RunBlueprint : Action, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    [Tooltip("Сколько ждать секунд (0 - бесконечно)")]
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedFloat waitTime = (SharedFloat) 0.0f;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedGameObject bluerintPrefab;
    private float waitDuration;
    private float startTime;
    private float pauseTime;
    private FlowScriptController blueprintController;

    public override void OnStart()
    {
      this.startTime = Time.time;
      this.waitDuration = this.waitTime.Value;
      IEntity entity = EntityUtility.GetEntity(this.gameObject);
      if (entity == null)
      {
        Debug.LogWarning((object) (this.gameObject.name + " : entity not found, method : " + this.GetType().Name + ":" + MethodBase.GetCurrentMethod().Name), (UnityEngine.Object) this.gameObject);
        this.blueprintController = (FlowScriptController) null;
      }
      else
      {
        this.blueprintController = BlueprintServiceUtility.Start(this.bluerintPrefab.Value, entity, (System.Action) null, (string) null);
        if (!(bool) (UnityEngine.Object) this.blueprintController)
          return;
        this.blueprintController.SendEvent("Start");
      }
    }

    public override void OnEnd()
    {
      if (!(bool) (UnityEngine.Object) this.blueprintController)
        return;
      this.blueprintController.SendEvent("End");
    }

    public override TaskStatus OnUpdate()
    {
      if ((UnityEngine.Object) this.blueprintController == (UnityEngine.Object) null)
        return TaskStatus.Failure;
      return (double) this.waitDuration != 0.0 && (double) this.startTime + (double) this.waitDuration < (double) Time.time ? TaskStatus.Success : TaskStatus.Running;
    }

    public override void OnPause(bool paused)
    {
      if (paused)
        this.pauseTime = Time.time;
      else
        this.startTime += Time.time - this.pauseTime;
    }

    public override void OnReset() => this.waitTime = (SharedFloat) 1f;

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize<NodeData>(writer, "NodeData", this.nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", this.id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", this.friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", this.instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", this.disabled);
      BehaviorTreeDataWriteUtility.WriteShared<SharedFloat>(writer, "WaitTime", this.waitTime);
      BehaviorTreeDataWriteUtility.WriteShared<SharedGameObject>(writer, "BluerintPrefab", this.bluerintPrefab);
    }

    public void DataRead(IDataReader reader, System.Type type)
    {
      this.nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      this.id = DefaultDataReadUtility.Read(reader, "Id", this.id);
      this.friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", this.friendlyName);
      this.instant = DefaultDataReadUtility.Read(reader, "Instant", this.instant);
      this.disabled = DefaultDataReadUtility.Read(reader, "Disabled", this.disabled);
      this.waitTime = BehaviorTreeDataReadUtility.ReadShared<SharedFloat>(reader, "WaitTime", this.waitTime);
      this.bluerintPrefab = BehaviorTreeDataReadUtility.ReadShared<SharedGameObject>(reader, "BluerintPrefab", this.bluerintPrefab);
    }
  }
}

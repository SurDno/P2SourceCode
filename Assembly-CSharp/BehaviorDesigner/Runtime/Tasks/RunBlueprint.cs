using System;
using System.Reflection;
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
using UnityEngine;
using Object = UnityEngine.Object;

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
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [SerializeField]
    public SharedFloat waitTime = 0.0f;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy()]
    [SerializeField]
    public SharedGameObject bluerintPrefab;
    private float waitDuration;
    private float startTime;
    private float pauseTime;
    private FlowScriptController blueprintController;

    public override void OnStart()
    {
      startTime = Time.time;
      waitDuration = waitTime.Value;
      IEntity entity = EntityUtility.GetEntity(gameObject);
      if (entity == null)
      {
        Debug.LogWarning(gameObject.name + " : entity not found, method : " + GetType().Name + ":" + MethodBase.GetCurrentMethod().Name, gameObject);
        blueprintController = null;
      }
      else
      {
        blueprintController = BlueprintServiceUtility.Start(bluerintPrefab.Value, entity, null, null);
        if (!(bool) (Object) blueprintController)
          return;
        blueprintController.SendEvent("Start");
      }
    }

    public override void OnEnd()
    {
      if (!(bool) (Object) blueprintController)
        return;
      blueprintController.SendEvent("End");
    }

    public override TaskStatus OnUpdate()
    {
      if (blueprintController == null)
        return TaskStatus.Failure;
      return waitDuration != 0.0 && startTime + (double) waitDuration < Time.time ? TaskStatus.Success : TaskStatus.Running;
    }

    public override void OnPause(bool paused)
    {
      if (paused)
        pauseTime = Time.time;
      else
        startTime += Time.time - pauseTime;
    }

    public override void OnReset() => waitTime = 1f;

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize(writer, "NodeData", nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", disabled);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "WaitTime", waitTime);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "BluerintPrefab", bluerintPrefab);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      id = DefaultDataReadUtility.Read(reader, "Id", id);
      friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", friendlyName);
      instant = DefaultDataReadUtility.Read(reader, "Instant", instant);
      disabled = DefaultDataReadUtility.Read(reader, "Disabled", disabled);
      waitTime = BehaviorTreeDataReadUtility.ReadShared(reader, "WaitTime", waitTime);
      bluerintPrefab = BehaviorTreeDataReadUtility.ReadShared(reader, "BluerintPrefab", bluerintPrefab);
    }
  }
}

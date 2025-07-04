﻿using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Scripts.Tools.Serializations.Converters;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityTransform
{
  [FactoryProxy(typeof (GetRotation))]
  [TaskCategory("Basic/Transform")]
  [TaskDescription("Stores the rotation of the Transform. Returns Success.")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class GetRotation : Action, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    [Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [SerializeField]
    public SharedGameObject targetGameObject;
    [Tooltip("The rotation of the Transform")]
    [RequiredField]
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy()]
    [SerializeField]
    public SharedQuaternion storeValue;
    private Transform targetTransform;
    private GameObject prevGameObject;

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize(writer, "NodeData", nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", disabled);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "TargetGameObject", targetGameObject);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "StoreValue", storeValue);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      id = DefaultDataReadUtility.Read(reader, "Id", id);
      friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", friendlyName);
      instant = DefaultDataReadUtility.Read(reader, "Instant", instant);
      disabled = DefaultDataReadUtility.Read(reader, "Disabled", disabled);
      targetGameObject = BehaviorTreeDataReadUtility.ReadShared(reader, "TargetGameObject", targetGameObject);
      storeValue = BehaviorTreeDataReadUtility.ReadShared(reader, "StoreValue", storeValue);
    }

    public override void OnStart()
    {
      GameObject defaultGameObject = GetDefaultGameObject(targetGameObject.Value);
      if (!(defaultGameObject != prevGameObject))
        return;
      targetTransform = defaultGameObject.GetComponent<Transform>();
      prevGameObject = defaultGameObject;
    }

    public override TaskStatus OnUpdate()
    {
      if (targetTransform == null)
      {
        Debug.LogWarning("Transform is null");
        return TaskStatus.Failure;
      }
      storeValue.Value = targetTransform.rotation;
      return TaskStatus.Success;
    }

    public override void OnReset()
    {
      targetGameObject = null;
      storeValue = Quaternion.identity;
    }
  }
}

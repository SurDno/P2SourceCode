using System;
using System.Reflection;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Components.Parameters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Components;
using Scripts.Tools.Serializations.Converters;
using UnityEngine;

namespace Engine.BehaviourNodes.Conditionals
{
  [TaskDescription("Find closest infected in list and write to Result")]
  [TaskCategory("Pathologic")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  [FactoryProxy(typeof (FindClosestInfected))]
  public class FindClosestInfected : 
    FindClosestInListBase,
    IStub,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [SerializeField]
    public SharedFloat Threshold = 0.0f;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy()]
    [SerializeField]
    public bool FindLivingNpc;

    protected override bool Filter(GameObject gameObject)
    {
      IEntity entity = EntityUtility.GetEntity(gameObject);
      EnemyBase component1 = gameObject.GetComponent<EnemyBase>();
      if (FindLivingNpc && component1 == null || !FindLivingNpc && component1 != null)
        return false;
      if (entity == null)
      {
        Debug.LogWarning(gameObject.name + " : entity not found, method : " + GetType().Name + ":" + MethodBase.GetCurrentMethod().Name, gameObject);
        return false;
      }
      ParametersComponent component2 = entity.GetComponent<ParametersComponent>();
      if (component2 != null)
      {
        IParameter<bool> byName1 = component2.GetByName<bool>(ParameterNameEnum.IsCombatIgnored);
        if (byName1 != null && byName1.Value)
          return false;
        IParameter<float> byName2 = component2.GetByName<float>(ParameterNameEnum.Infection);
        if (byName2 != null)
          return byName2.Value > (double) Threshold.Value;
      }
      return false;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize(writer, "NodeData", nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", disabled);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "InputList", InputList);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "Result", Result);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "Threshold", Threshold);
      DefaultDataWriteUtility.Write(writer, "FindLivingNpc", FindLivingNpc);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      id = DefaultDataReadUtility.Read(reader, "Id", id);
      friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", friendlyName);
      instant = DefaultDataReadUtility.Read(reader, "Instant", instant);
      disabled = DefaultDataReadUtility.Read(reader, "Disabled", disabled);
      InputList = BehaviorTreeDataReadUtility.ReadShared(reader, "InputList", InputList);
      Result = BehaviorTreeDataReadUtility.ReadShared(reader, "Result", Result);
      Threshold = BehaviorTreeDataReadUtility.ReadShared(reader, "Threshold", Threshold);
      FindLivingNpc = DefaultDataReadUtility.Read(reader, "FindLivingNpc", FindLivingNpc);
    }
  }
}

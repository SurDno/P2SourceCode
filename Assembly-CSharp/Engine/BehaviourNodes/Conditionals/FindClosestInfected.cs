// Decompiled with JetBrains decompiler
// Type: Engine.BehaviourNodes.Conditionals.FindClosestInfected
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

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
using System.Reflection;
using UnityEngine;

#nullable disable
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
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedFloat Threshold = (SharedFloat) 0.0f;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public bool FindLivingNpc;

    protected override bool Filter(GameObject gameObject)
    {
      IEntity entity = EntityUtility.GetEntity(gameObject);
      EnemyBase component1 = gameObject.GetComponent<EnemyBase>();
      if (this.FindLivingNpc && (UnityEngine.Object) component1 == (UnityEngine.Object) null || !this.FindLivingNpc && (UnityEngine.Object) component1 != (UnityEngine.Object) null)
        return false;
      if (entity == null)
      {
        Debug.LogWarning((object) (gameObject.name + " : entity not found, method : " + this.GetType().Name + ":" + MethodBase.GetCurrentMethod().Name), (UnityEngine.Object) gameObject);
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
          return (double) byName2.Value > (double) this.Threshold.Value;
      }
      return false;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize<NodeData>(writer, "NodeData", this.nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", this.id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", this.friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", this.instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", this.disabled);
      BehaviorTreeDataWriteUtility.WriteShared<SharedTransformList>(writer, "InputList", this.InputList);
      BehaviorTreeDataWriteUtility.WriteShared<SharedTransform>(writer, "Result", this.Result);
      BehaviorTreeDataWriteUtility.WriteShared<SharedFloat>(writer, "Threshold", this.Threshold);
      DefaultDataWriteUtility.Write(writer, "FindLivingNpc", this.FindLivingNpc);
    }

    public void DataRead(IDataReader reader, System.Type type)
    {
      this.nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      this.id = DefaultDataReadUtility.Read(reader, "Id", this.id);
      this.friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", this.friendlyName);
      this.instant = DefaultDataReadUtility.Read(reader, "Instant", this.instant);
      this.disabled = DefaultDataReadUtility.Read(reader, "Disabled", this.disabled);
      this.InputList = BehaviorTreeDataReadUtility.ReadShared<SharedTransformList>(reader, "InputList", this.InputList);
      this.Result = BehaviorTreeDataReadUtility.ReadShared<SharedTransform>(reader, "Result", this.Result);
      this.Threshold = BehaviorTreeDataReadUtility.ReadShared<SharedFloat>(reader, "Threshold", this.Threshold);
      this.FindLivingNpc = DefaultDataReadUtility.Read(reader, "FindLivingNpc", this.FindLivingNpc);
    }
  }
}

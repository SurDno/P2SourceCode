// Decompiled with JetBrains decompiler
// Type: BehaviorDesigner.Runtime.Tasks.Pathologic.Idle2
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
namespace BehaviorDesigner.Runtime.Tasks.Pathologic
{
  [TaskDescription("Idle (puts on closest navmesh position)")]
  [TaskCategory("Pathologic")]
  [TaskIcon("Pathologic_IdleIcon.png")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  [FactoryProxy(typeof (Idle2))]
  public class Idle2 : IdleBase, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    protected override void DoIdle(NpcState state, float primaryIdleProbability)
    {
      state.Idle(primaryIdleProbability, this.MakeObstacle.Value);
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize<NodeData>(writer, "NodeData", this.nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", this.id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", this.friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", this.instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", this.disabled);
      BehaviorTreeDataWriteUtility.WriteShared<SharedFloat>(writer, "PrimaryIdleProbability", this.primaryIdleProbability);
      BehaviorTreeDataWriteUtility.WriteShared<SharedFloat>(writer, "IdleTime", this.idleTime);
      BehaviorTreeDataWriteUtility.WriteShared<SharedBool>(writer, "RandomIdle", this.randomIdle);
      BehaviorTreeDataWriteUtility.WriteShared<SharedFloat>(writer, "RandomIdleMin", this.randomIdleMin);
      BehaviorTreeDataWriteUtility.WriteShared<SharedFloat>(writer, "RandomIdleMax", this.randomIdleMax);
      BehaviorTreeDataWriteUtility.WriteShared<SharedBool>(writer, "MakeObstacle", this.MakeObstacle);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      this.id = DefaultDataReadUtility.Read(reader, "Id", this.id);
      this.friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", this.friendlyName);
      this.instant = DefaultDataReadUtility.Read(reader, "Instant", this.instant);
      this.disabled = DefaultDataReadUtility.Read(reader, "Disabled", this.disabled);
      this.primaryIdleProbability = BehaviorTreeDataReadUtility.ReadShared<SharedFloat>(reader, "PrimaryIdleProbability", this.primaryIdleProbability);
      this.idleTime = BehaviorTreeDataReadUtility.ReadShared<SharedFloat>(reader, "IdleTime", this.idleTime);
      this.randomIdle = BehaviorTreeDataReadUtility.ReadShared<SharedBool>(reader, "RandomIdle", this.randomIdle);
      this.randomIdleMin = BehaviorTreeDataReadUtility.ReadShared<SharedFloat>(reader, "RandomIdleMin", this.randomIdleMin);
      this.randomIdleMax = BehaviorTreeDataReadUtility.ReadShared<SharedFloat>(reader, "RandomIdleMax", this.randomIdleMax);
      this.MakeObstacle = BehaviorTreeDataReadUtility.ReadShared<SharedBool>(reader, "MakeObstacle", this.MakeObstacle);
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: BehaviorDesigner.Runtime.Tasks.Pathologic.GetPathFromTransform
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
using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace BehaviorDesigner.Runtime.Tasks.Pathologic
{
  [TaskDescription("Get path from transform")]
  [TaskCategory("Pathologic/Movement")]
  [TaskIcon("Pathologic_InstantIcon.png")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  [FactoryProxy(typeof (GetPathFromTransform))]
  public class GetPathFromTransform : BehaviorDesigner.Runtime.Tasks.Action, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedTransformList Result;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedTransform Target;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedInt PointIndex;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedBool InversePath = (SharedBool) false;
    private bool success = false;

    public override void OnStart()
    {
      this.success = false;
      PathPart component1 = this.Target.Value.GetComponent<PathPart>();
      if ((UnityEngine.Object) component1 != (UnityEngine.Object) null)
      {
        this.Result.Value = component1.PointsList;
        this.success = true;
      }
      else
      {
        PatrolPath component2 = this.Target.Value.GetComponent<PatrolPath>();
        if ((UnityEngine.Object) component2 != (UnityEngine.Object) null)
        {
          List<Transform> presetPath = component2.GetPresetPath(this.PointIndex.Value, this.InversePath.Value);
          if (presetPath != null)
          {
            this.Result.Value = presetPath;
            this.success = true;
            return;
          }
        }
        Debug.LogError((object) (this.gameObject.name + "  has wrong path!"));
      }
    }

    public override TaskStatus OnUpdate()
    {
      return !this.success ? TaskStatus.Failure : TaskStatus.Success;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize<NodeData>(writer, "NodeData", this.nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", this.id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", this.friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", this.instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", this.disabled);
      BehaviorTreeDataWriteUtility.WriteShared<SharedTransformList>(writer, "Result", this.Result);
      BehaviorTreeDataWriteUtility.WriteShared<SharedTransform>(writer, "Target", this.Target);
      BehaviorTreeDataWriteUtility.WriteShared<SharedInt>(writer, "PointIndex", this.PointIndex);
      BehaviorTreeDataWriteUtility.WriteShared<SharedBool>(writer, "InversePath", this.InversePath);
    }

    public void DataRead(IDataReader reader, System.Type type)
    {
      this.nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      this.id = DefaultDataReadUtility.Read(reader, "Id", this.id);
      this.friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", this.friendlyName);
      this.instant = DefaultDataReadUtility.Read(reader, "Instant", this.instant);
      this.disabled = DefaultDataReadUtility.Read(reader, "Disabled", this.disabled);
      this.Result = BehaviorTreeDataReadUtility.ReadShared<SharedTransformList>(reader, "Result", this.Result);
      this.Target = BehaviorTreeDataReadUtility.ReadShared<SharedTransform>(reader, "Target", this.Target);
      this.PointIndex = BehaviorTreeDataReadUtility.ReadShared<SharedInt>(reader, "PointIndex", this.PointIndex);
      this.InversePath = BehaviorTreeDataReadUtility.ReadShared<SharedBool>(reader, "InversePath", this.InversePath);
    }
  }
}

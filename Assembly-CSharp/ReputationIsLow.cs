// Decompiled with JetBrains decompiler
// Type: ReputationIsLow
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
using Engine.Common.Components;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Components;
using Scripts.Tools.Serializations.Converters;
using System.Reflection;
using UnityEngine;

#nullable disable
[TaskDescription("Player reputation is low")]
[TaskCategory("Pathologic")]
[Factory]
[GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
[FactoryProxy(typeof (ReputationIsLow))]
public class ReputationIsLow : Conditional, IStub, ISerializeDataWrite, ISerializeDataRead
{
  [DataReadProxy(MemberEnum.None)]
  [DataWriteProxy(MemberEnum.None)]
  [CopyableProxy(MemberEnum.None)]
  [SerializeField]
  public SharedFloat Threshold;
  private NavigationComponent navigation;
  private IEntity entity;

  public override void OnAwake()
  {
    this.entity = EntityUtility.GetEntity(this.gameObject);
    if (this.entity == null)
    {
      Debug.LogWarning((object) (this.gameObject.name + " : entity not found, method : " + this.GetType().Name + ":" + MethodBase.GetCurrentMethod().Name), (UnityEngine.Object) this.gameObject);
    }
    else
    {
      this.navigation = this.entity.GetComponent<NavigationComponent>();
      if (this.navigation != null)
        return;
      Debug.LogWarningFormat("{0}: doesn't contain {1} engine component", (object) this.gameObject.name, (object) typeof (INavigationComponent).Name);
    }
  }

  public override TaskStatus OnUpdate()
  {
    return this.entity == null || this.navigation == null || this.navigation.Region == null || this.navigation.Region.Reputation == null ? TaskStatus.Failure : ((double) this.navigation.Region.Reputation.Value < (double) this.Threshold.Value ? TaskStatus.Success : TaskStatus.Failure);
  }

  public void DataWrite(IDataWriter writer)
  {
    DefaultDataWriteUtility.WriteSerialize<NodeData>(writer, "NodeData", this.nodeData);
    DefaultDataWriteUtility.Write(writer, "Id", this.id);
    DefaultDataWriteUtility.Write(writer, "FriendlyName", this.friendlyName);
    DefaultDataWriteUtility.Write(writer, "Instant", this.instant);
    DefaultDataWriteUtility.Write(writer, "Disabled", this.disabled);
    BehaviorTreeDataWriteUtility.WriteShared<SharedFloat>(writer, "Threshold", this.Threshold);
  }

  public void DataRead(IDataReader reader, System.Type type)
  {
    this.nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
    this.id = DefaultDataReadUtility.Read(reader, "Id", this.id);
    this.friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", this.friendlyName);
    this.instant = DefaultDataReadUtility.Read(reader, "Instant", this.instant);
    this.disabled = DefaultDataReadUtility.Read(reader, "Disabled", this.disabled);
    this.Threshold = BehaviorTreeDataReadUtility.ReadShared<SharedFloat>(reader, "Threshold", this.Threshold);
  }
}

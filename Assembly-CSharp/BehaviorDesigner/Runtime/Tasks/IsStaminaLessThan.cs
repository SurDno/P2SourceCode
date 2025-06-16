// Decompiled with JetBrains decompiler
// Type: BehaviorDesigner.Runtime.Tasks.IsStaminaLessThan
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

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

#nullable disable
namespace BehaviorDesigner.Runtime.Tasks
{
  [TaskDescription("")]
  [TaskCategory("Pathologic/Fight")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  [FactoryProxy(typeof (IsStaminaLessThan))]
  public class IsStaminaLessThan : Conditional, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedFloat StaminaThreshold = (SharedFloat) 0.0f;
    private IParameter<float> staminaParameter;

    public override void OnAwake()
    {
      if ((UnityEngine.Object) this.gameObject == (UnityEngine.Object) null)
        return;
      EngineGameObject component1 = this.gameObject.GetComponent<EngineGameObject>();
      if ((UnityEngine.Object) component1 == (UnityEngine.Object) null)
        return;
      IEntity owner = component1.Owner;
      if (owner == null)
        return;
      ParametersComponent component2 = owner.GetComponent<ParametersComponent>();
      if (component2 == null)
        return;
      this.staminaParameter = component2.GetByName<float>(ParameterNameEnum.Stamina);
    }

    public override TaskStatus OnUpdate()
    {
      return this.staminaParameter == null ? TaskStatus.Failure : ((double) this.staminaParameter.Value <= (double) this.StaminaThreshold.Value ? TaskStatus.Success : TaskStatus.Failure);
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize<NodeData>(writer, "NodeData", this.nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", this.id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", this.friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", this.instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", this.disabled);
      BehaviorTreeDataWriteUtility.WriteShared<SharedFloat>(writer, "StaminaThreshold", this.StaminaThreshold);
    }

    public void DataRead(IDataReader reader, System.Type type)
    {
      this.nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      this.id = DefaultDataReadUtility.Read(reader, "Id", this.id);
      this.friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", this.friendlyName);
      this.instant = DefaultDataReadUtility.Read(reader, "Instant", this.instant);
      this.disabled = DefaultDataReadUtility.Read(reader, "Disabled", this.disabled);
      this.StaminaThreshold = BehaviorTreeDataReadUtility.ReadShared<SharedFloat>(reader, "StaminaThreshold", this.StaminaThreshold);
    }
  }
}

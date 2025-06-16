// Decompiled with JetBrains decompiler
// Type: BehaviorDesigner.Runtime.Tasks.Pathologic.TargetWeaponOn
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Behaviours.Components;
using Engine.Behaviours.Engines.Controllers;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Scripts.Tools.Serializations.Converters;
using UnityEngine;

#nullable disable
namespace BehaviorDesigner.Runtime.Tasks.Pathologic
{
  [TaskDescription("Target weapon On")]
  [TaskCategory("Pathologic/IK")]
  [TaskIcon("Pathologic_InstantIcon.png")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  [FactoryProxy(typeof (TargetWeaponOn))]
  public class TargetWeaponOn : BehaviorDesigner.Runtime.Tasks.Action, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedTransform Target;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public Pivot.AimWeaponType AimTo = Pivot.AimWeaponType.Unknown;
    private IKController ikController;

    public override TaskStatus OnUpdate()
    {
      if ((UnityEngine.Object) this.ikController == (UnityEngine.Object) null)
      {
        this.ikController = this.gameObject.GetComponent<IKController>();
        if ((UnityEngine.Object) this.ikController == (UnityEngine.Object) null)
        {
          Debug.LogError((object) (this.gameObject.name + ": doesn't contain " + typeof (IKController).Name + " unity component"), (UnityEngine.Object) this.gameObject);
          return TaskStatus.Failure;
        }
      }
      if ((UnityEngine.Object) this.Target.Value == (UnityEngine.Object) null)
      {
        Debug.LogWarning((object) (this.gameObject.name + ": null target"), (UnityEngine.Object) this.gameObject);
        return TaskStatus.Failure;
      }
      this.ikController.WeaponTarget = this.Target.Value;
      this.ikController.WeaponAimTo = this.AimTo;
      return TaskStatus.Success;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize<NodeData>(writer, "NodeData", this.nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", this.id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", this.friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", this.instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", this.disabled);
      BehaviorTreeDataWriteUtility.WriteShared<SharedTransform>(writer, "Target", this.Target);
      DefaultDataWriteUtility.WriteEnum<Pivot.AimWeaponType>(writer, "AimTo", this.AimTo);
    }

    public void DataRead(IDataReader reader, System.Type type)
    {
      this.nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      this.id = DefaultDataReadUtility.Read(reader, "Id", this.id);
      this.friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", this.friendlyName);
      this.instant = DefaultDataReadUtility.Read(reader, "Instant", this.instant);
      this.disabled = DefaultDataReadUtility.Read(reader, "Disabled", this.disabled);
      this.Target = BehaviorTreeDataReadUtility.ReadShared<SharedTransform>(reader, "Target", this.Target);
      this.AimTo = DefaultDataReadUtility.ReadEnum<Pivot.AimWeaponType>(reader, "AimTo");
    }
  }
}

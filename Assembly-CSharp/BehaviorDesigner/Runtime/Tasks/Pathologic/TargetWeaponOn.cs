using System;
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

namespace BehaviorDesigner.Runtime.Tasks.Pathologic;

[TaskDescription("Target weapon On")]
[TaskCategory("Pathologic/IK")]
[TaskIcon("Pathologic_InstantIcon.png")]
[Factory]
[GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
[FactoryProxy(typeof(TargetWeaponOn))]
public class TargetWeaponOn : Action, IStub, ISerializeDataWrite, ISerializeDataRead {
	[DataReadProxy] [DataWriteProxy] [CopyableProxy] [SerializeField]
	public SharedTransform Target;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy()] [SerializeField]
	public Pivot.AimWeaponType AimTo = Pivot.AimWeaponType.Unknown;

	private IKController ikController;

	public override TaskStatus OnUpdate() {
		if (ikController == null) {
			ikController = gameObject.GetComponent<IKController>();
			if (ikController == null) {
				Debug.LogError(gameObject.name + ": doesn't contain " + typeof(IKController).Name + " unity component",
					gameObject);
				return TaskStatus.Failure;
			}
		}

		if (Target.Value == null) {
			Debug.LogWarning(gameObject.name + ": null target", gameObject);
			return TaskStatus.Failure;
		}

		ikController.WeaponTarget = Target.Value;
		ikController.WeaponAimTo = AimTo;
		return TaskStatus.Success;
	}

	public void DataWrite(IDataWriter writer) {
		DefaultDataWriteUtility.WriteSerialize(writer, "NodeData", nodeData);
		DefaultDataWriteUtility.Write(writer, "Id", id);
		DefaultDataWriteUtility.Write(writer, "FriendlyName", friendlyName);
		DefaultDataWriteUtility.Write(writer, "Instant", instant);
		DefaultDataWriteUtility.Write(writer, "Disabled", disabled);
		BehaviorTreeDataWriteUtility.WriteShared(writer, "Target", Target);
		DefaultDataWriteUtility.WriteEnum(writer, "AimTo", AimTo);
	}

	public void DataRead(IDataReader reader, Type type) {
		nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
		id = DefaultDataReadUtility.Read(reader, "Id", id);
		friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", friendlyName);
		instant = DefaultDataReadUtility.Read(reader, "Instant", instant);
		disabled = DefaultDataReadUtility.Read(reader, "Disabled", disabled);
		Target = BehaviorTreeDataReadUtility.ReadShared(reader, "Target", Target);
		AimTo = DefaultDataReadUtility.ReadEnum<Pivot.AimWeaponType>(reader, "AimTo");
	}
}
using System;
using System.Reflection;
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
using UnityEngine;

[TaskDescription("Player reputation is low")]
[TaskCategory("Pathologic")]
[Factory]
[GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
[FactoryProxy(typeof(ReputationIsLow))]
public class ReputationIsLow : Conditional, IStub, ISerializeDataWrite, ISerializeDataRead {
	[DataReadProxy] [DataWriteProxy] [CopyableProxy()] [SerializeField]
	public SharedFloat Threshold;

	private NavigationComponent navigation;
	private IEntity entity;

	public override void OnAwake() {
		entity = EntityUtility.GetEntity(gameObject);
		if (entity == null)
			Debug.LogWarning(
				gameObject.name + " : entity not found, method : " + GetType().Name + ":" +
				MethodBase.GetCurrentMethod().Name, gameObject);
		else {
			navigation = entity.GetComponent<NavigationComponent>();
			if (navigation != null)
				return;
			Debug.LogWarningFormat("{0}: doesn't contain {1} engine component", gameObject.name,
				typeof(INavigationComponent).Name);
		}
	}

	public override TaskStatus OnUpdate() {
		return entity == null || navigation == null || navigation.Region == null ||
		       navigation.Region.Reputation == null ? TaskStatus.Failure :
			navigation.Region.Reputation.Value < (double)Threshold.Value ? TaskStatus.Success : TaskStatus.Failure;
	}

	public void DataWrite(IDataWriter writer) {
		DefaultDataWriteUtility.WriteSerialize(writer, "NodeData", nodeData);
		DefaultDataWriteUtility.Write(writer, "Id", id);
		DefaultDataWriteUtility.Write(writer, "FriendlyName", friendlyName);
		DefaultDataWriteUtility.Write(writer, "Instant", instant);
		DefaultDataWriteUtility.Write(writer, "Disabled", disabled);
		BehaviorTreeDataWriteUtility.WriteShared(writer, "Threshold", Threshold);
	}

	public void DataRead(IDataReader reader, Type type) {
		nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
		id = DefaultDataReadUtility.Read(reader, "Id", id);
		friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", friendlyName);
		instant = DefaultDataReadUtility.Read(reader, "Instant", instant);
		disabled = DefaultDataReadUtility.Read(reader, "Disabled", disabled);
		Threshold = BehaviorTreeDataReadUtility.ReadShared(reader, "Threshold", Threshold);
	}
}
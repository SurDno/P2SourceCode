using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Behaviours.Components;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks;

[Factory]
[GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
[FactoryProxy(typeof(FightBase))]
public class FightBase : Action, IStub, ISerializeDataWrite, ISerializeDataRead {
	protected NPCEnemy owner;
	protected float waitDuration;
	protected float startTime;
	protected float lastTime;
	private float pauseTime;
	private bool initialized;

	public virtual TaskStatus DoUpdate(float deltaTime) {
		return TaskStatus.Running;
	}

	public override void OnPause(bool paused) {
		if (paused)
			pauseTime = Time.time;
		else {
			startTime += Time.time - pauseTime;
			lastTime += Time.time - pauseTime;
		}
	}

	public override void OnStart() {
		if (!initialized) {
			var component = GetComponent<Pivot>();
			if (component == null) {
				Debug.LogWarning(gameObject.name + ": doesn't contain " + typeof(Pivot).Name + " engine component",
					gameObject);
				return;
			}

			owner = component.GetNpcEnemy();
			if (owner == null) {
				Debug.LogWarning(gameObject.name + ": doesn't contain " + typeof(NPCEnemy).Name + " engine component",
					gameObject);
				return;
			}

			initialized = true;
		}

		lastTime = startTime = Time.time;
	}

	public override TaskStatus OnUpdate() {
		if (!initialized || owner.Enemy == null)
			return TaskStatus.Failure;
		var deltaTime = Time.time - lastTime;
		lastTime = Time.time;
		return DoUpdate(deltaTime);
	}

	public override void OnReset() { }

	public override void OnEnd() { }

	public void DataWrite(IDataWriter writer) {
		DefaultDataWriteUtility.WriteSerialize(writer, "NodeData", nodeData);
		DefaultDataWriteUtility.Write(writer, "Id", id);
		DefaultDataWriteUtility.Write(writer, "FriendlyName", friendlyName);
		DefaultDataWriteUtility.Write(writer, "Instant", instant);
		DefaultDataWriteUtility.Write(writer, "Disabled", disabled);
	}

	public void DataRead(IDataReader reader, Type type) {
		nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
		id = DefaultDataReadUtility.Read(reader, "Id", id);
		friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", friendlyName);
		instant = DefaultDataReadUtility.Read(reader, "Instant", instant);
		disabled = DefaultDataReadUtility.Read(reader, "Disabled", disabled);
	}
}
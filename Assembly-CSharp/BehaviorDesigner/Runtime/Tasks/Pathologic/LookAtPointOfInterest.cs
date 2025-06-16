using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Behaviours.Components;
using Engine.Behaviours.Engines.Controllers;
using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Components;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;
using Engine.Source.Components;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Pathologic;

[TaskDescription("Look at points of interest. Run this as parallel selector task")]
[TaskCategory("Pathologic")]
[TaskIcon("Pathologic_LongIcon.png")]
[Factory]
[GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
[FactoryProxy(typeof(LookAtPointOfInterest))]
public class LookAtPointOfInterest : Action, IStub, ISerializeDataWrite, ISerializeDataRead {
	protected DetectorComponent detector;
	protected IEntity entity;
	private IKController lookAtController;
	private const float updateFrequency = 1f;
	private bool inited;
	private float timeLeft;
	private List<Transform> potentialPOIs = new();
	private List<Transform> potentialPriorityPOIs = new();

	public override void OnAwake() {
		inited = false;
		entity = EntityUtility.GetEntity(gameObject);
		if (entity == null)
			Debug.LogWarning(
				gameObject.name + " : entity not found, method : " + GetType().Name + ":" +
				MethodBase.GetCurrentMethod().Name, gameObject);
		else {
			detector = (DetectorComponent)entity.GetComponent<IDetectorComponent>();
			if (detector == null)
				Debug.LogWarningFormat("{0}: doesn't contain {1} engine component", gameObject.name,
					typeof(IDetectorComponent).Name);
			else {
				lookAtController = gameObject.GetComponent<IKController>();
				if (lookAtController == null)
					Debug.LogErrorFormat("{0}: doesn't contain {1} unity component", gameObject.name,
						typeof(IKController).Name);
				else
					inited = true;
			}
		}
	}

	public override void OnStart() {
		if (!inited)
			return;
		timeLeft = 0.0f;
	}

	public override void OnEnd() {
		if (!inited)
			return;
		lookAtController.LookTarget = null;
	}

	public override TaskStatus OnUpdate() {
		if (!inited)
			return TaskStatus.Failure;
		timeLeft -= Time.deltaTime;
		if (timeLeft < 0.0) {
			timeLeft = 1f;
			UpdateLookAtTarget();
		}

		return TaskStatus.Running;
	}

	private void UpdateLookAtTarget() {
		potentialPOIs.Clear();
		potentialPriorityPOIs.Clear();
		foreach (var detectableComponent in detector.Visible)
			if (!detectableComponent.IsDisposed) {
				var owner = (IEntityView)((EngineComponent)detectableComponent).Owner;
				if (!(owner.GameObject == null)) {
					var component1 = owner.GameObject
						.GetComponent<Engine.Behaviours.Engines.PointsOfInterest.LookAtPointOfInterest>();
					if (component1 != null) {
						if (component1.POI != null)
							potentialPOIs.Add(component1.POI);
						else
							potentialPOIs.Add(component1.transform);
					}

					var component2 = owner.GameObject.GetComponent<Pivot>();
					if (component2 != null && component2.Head != null)
						potentialPriorityPOIs.Add(component2.Head.transform);
				}
			}

		if (potentialPriorityPOIs.Count > 0) {
			potentialPriorityPOIs.Sort(ComparePOIs);
			lookAtController.LookTarget = potentialPriorityPOIs[0];
		} else if (potentialPOIs.Count > 0) {
			potentialPOIs.Sort(ComparePOIs);
			lookAtController.LookTarget = potentialPOIs[0];
		} else
			lookAtController.LookTarget = null;
	}

	private int ComparePOIs(Transform a, Transform b) {
		var sqrMagnitude1 = (a.position - gameObject.transform.position).sqrMagnitude;
		var sqrMagnitude2 = (b.position - gameObject.transform.position).sqrMagnitude;
		return a == b ? 0 : sqrMagnitude1 < (double)sqrMagnitude2 ? -1 : 1;
	}

	public IEnumerator ExecuteSecond(float delay, System.Action action) {
		var time = Time.unscaledTime;
		while (time + (double)delay > Time.unscaledTime) {
			var action1 = action;
			if (action1 != null)
				action1();
			yield return null;
		}
	}

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
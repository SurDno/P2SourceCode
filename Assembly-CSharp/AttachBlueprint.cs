using Engine.Common;
using Engine.Source.Commons;
using Engine.Source.Services;
using FlowCanvas;
using Inspectors;
using UnityEngine;

public class AttachBlueprint : MonoBehaviour, IEntityAttachable {
	[SerializeField] private FlowScriptController prefab;
	[Inspected] private FlowScriptController controller;

	public void Attach(IEntity owner) {
		controller = BlueprintServiceUtility.Start(prefab?.gameObject, owner, null, owner.GetInfo());
	}

	public void Detach() {
		if (!(controller != null))
			return;
		Destroy(controller.gameObject);
		controller = null;
	}
}
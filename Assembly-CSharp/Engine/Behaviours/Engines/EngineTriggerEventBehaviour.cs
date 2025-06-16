using Engine.Common;
using Engine.Source.Commons;
using Engine.Source.Components;
using UnityEngine;

namespace Engine.Behaviours.Engines;

public class EngineTriggerEventBehaviour : MonoBehaviour, IEntityAttachable {
	private TriggerComponent component;

	public void Attach(IEntity owner) {
		component = owner.GetComponent<TriggerComponent>();
		if (component == null)
			return;
		component.Attach();
	}

	public void Detach() {
		if (component == null)
			return;
		component.Detach();
		component = null;
	}

	private void OnTriggerEnter(Collider collider) {
		if (component == null)
			return;
		component.Enter(collider.gameObject);
	}

	private void OnTriggerExit(Collider collider) {
		if (component == null)
			return;
		component.Exit(collider.gameObject);
	}
}
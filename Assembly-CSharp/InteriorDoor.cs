using System.Collections.Generic;
using System.Linq;
using Engine.Common;
using Engine.Common.Components.Gate;
using Engine.Common.Services;
using Engine.Source.Commons;
using Engine.Source.Components;
using Engine.Source.Settings.External;
using Inspectors;
using SoundPropagation;
using UnityEngine;
using UnityEngine.AI;

public class InteriorDoor : MonoBehaviour, IEntityAttachable {
	[SerializeField] private float angle;
	[SerializeField] private OcclusionPortal occlusionPortal;
	[SerializeField] private SPPortal soundPropagationPortal;
	[SerializeField] private NavMeshObstacle navMeshObstacle;
	[Inspected] private DoorComponent gate;
	[Inspected] private float currentAngle;
	[Inspected] private bool update;
	[Inspected] private bool openned;
	private HashSet<IEntity> targets = new();

	private bool IsOppening => gate.Opened.Value || (targets.Count != 0 && gate.LockState.Value == LockState.Unlocked);

	public void Attach(IEntity owner) {
		update = true;
		gate = owner.GetComponent<DoorComponent>();
		if (gate != null) {
			openned = IsOppening;
			UpdateAngle();
		}

		UpdateOcclusion();
		UpdateObstacle();
	}

	public void Detach() {
		gate = null;
	}

	public void Update() {
		if (gate == null)
			return;
		if (openned != IsOppening) {
			openned = IsOppening;
			UpdateAngle();
		}

		if (!update)
			return;
		var num = Time.deltaTime * ExternalSettingsInstance<ExternalCommonSettings>.Instance.DoorSpeed;
		var f = IsOppening ? GetAngle() : 0.0f;
		if (float.IsNaN(f))
			return;
		if (currentAngle > (double)f) {
			currentAngle -= num;
			if (currentAngle <= (double)f) {
				currentAngle = f;
				update = false;
			}
		} else if (currentAngle < (double)f) {
			currentAngle += num;
			if (currentAngle >= (double)f) {
				currentAngle = f;
				update = false;
			}
		} else
			update = false;

		transform.localRotation = Quaternion.Euler(0.0f, currentAngle, 0.0f);
		UpdateOcclusion();
		UpdateObstacle();
	}

	private void UpdateOcclusion() {
		if (occlusionPortal != null)
			occlusionPortal.open = currentAngle != 0.0;
		if (!(soundPropagationPortal != null))
			return;
		soundPropagationPortal.Occlusion = (float)((1.0 - Mathf.Min(1f, Mathf.Abs(currentAngle) / 45f)) * 1.5);
	}

	private void UpdateObstacle() {
		if (!(navMeshObstacle != null))
			return;
		var flag = Mathf.Approximately(Mathf.Abs(currentAngle), Mathf.Abs(angle));
		if (navMeshObstacle.enabled != flag)
			navMeshObstacle.enabled = flag;
	}

	private float GetAngle() {
		return targets.Count != 0
			? GetAngle(targets.First())
			: GetAngle(ServiceLocator.GetService<ISimulation>().Player);
	}

	private void UpdateAngle() {
		if (update || (IsOppening ? GetAngle() : 0.0) == currentAngle)
			return;
		update = true;
	}

	private float GetAngle(IEntity entity) {
		var gameObject = ((IEntityView)entity).GameObject;
		if (gameObject == null)
			return float.NaN;
		var num = Mathf.Sign(Vector3.Dot(transform.parent.rotation * Vector3.forward,
			gameObject.transform.rotation * Vector3.forward));
		return Mathf.Abs(angle) * -num;
	}

	public void Invalidate(HashSet<IEntity> targets) {
		this.targets = targets;
		update = true;
	}
}
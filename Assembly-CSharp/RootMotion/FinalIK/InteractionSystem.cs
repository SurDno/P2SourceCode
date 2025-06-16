using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace RootMotion.FinalIK;

[HelpURL("https://www.youtube.com/watch?v=r5jiZnsDH3M")]
[AddComponentMenu("Scripts/RootMotion.FinalIK/Interaction System/Interaction System")]
public class InteractionSystem : MonoBehaviour {
	[Tooltip("If not empty, only the targets with the specified tag will be used by this Interaction System.")]
	public string targetTag = "";

	[Tooltip("The fade in time of the interaction.")]
	public float fadeInTime = 0.3f;

	[Tooltip("The master speed for all interactions.")]
	public float speed = 1f;

	[Tooltip(
		"If > 0, lerps all the FBBIK channels used by the Interaction System back to their default or initial values when not in interaction.")]
	public float resetToDefaultsSpeed = 1f;

	[Header("Triggering")]
	[Tooltip("The collider that registers OnTriggerEnter and OnTriggerExit events with InteractionTriggers.")]
	[FormerlySerializedAs("collider")]
	public Collider characterCollider;

	[Tooltip(
		"Will be used by Interaction Triggers that need the camera's position. Assign the first person view character camera.")]
	[FormerlySerializedAs("camera")]
	public Transform FPSCamera;

	[Tooltip(
		"The layers that will be raycasted from the camera (along camera.forward). All InteractionTrigger look at target colliders should be included.")]
	public LayerMask camRaycastLayers;

	[Tooltip("Max distance of raycasting from the camera.")]
	public float camRaycastDistance = 1f;

	private List<InteractionTrigger> inContact = new();
	private List<int> bestRangeIndexes = new();
	public InteractionDelegate OnInteractionStart;
	public InteractionDelegate OnInteractionPause;
	public InteractionDelegate OnInteractionPickUp;
	public InteractionDelegate OnInteractionResume;
	public InteractionDelegate OnInteractionStop;
	public InteractionEventDelegate OnInteractionEvent;
	public RaycastHit raycastHit;

	[Space(10f)] [Tooltip("Reference to the FBBIK component.")] [SerializeField]
	private FullBodyBipedIK fullBody;

	[Tooltip("Handles looking at the interactions.")]
	public InteractionLookAt lookAt = new();

	private InteractionEffector[] interactionEffectors = new InteractionEffector[9] {
		new(FullBodyBipedEffector.Body),
		new(FullBodyBipedEffector.LeftFoot),
		new(FullBodyBipedEffector.LeftHand),
		new(FullBodyBipedEffector.LeftShoulder),
		new(FullBodyBipedEffector.LeftThigh),
		new(FullBodyBipedEffector.RightFoot),
		new(FullBodyBipedEffector.RightHand),
		new(FullBodyBipedEffector.RightShoulder),
		new(FullBodyBipedEffector.RightThigh)
	};

	private bool initiated;
	private Collider lastCollider;
	private Collider c;

	[ContextMenu("TUTORIAL VIDEO (PART 1: BASICS)")]
	private void OpenTutorial1() {
		Application.OpenURL("https://www.youtube.com/watch?v=r5jiZnsDH3M");
	}

	[ContextMenu("TUTORIAL VIDEO (PART 2: PICKING UP...)")]
	private void OpenTutorial2() {
		Application.OpenURL("https://www.youtube.com/watch?v=eP9-zycoHLk");
	}

	[ContextMenu("TUTORIAL VIDEO (PART 3: ANIMATION)")]
	private void OpenTutorial3() {
		Application.OpenURL(
			"https://www.youtube.com/watch?v=sQfB2RcT1T4&index=14&list=PLVxSIA1OaTOu8Nos3CalXbJ2DrKnntMv6");
	}

	[ContextMenu("TUTORIAL VIDEO (PART 4: TRIGGERS)")]
	private void OpenTutorial4() {
		Application.OpenURL(
			"https://www.youtube.com/watch?v=-TDZpNjt2mk&index=15&list=PLVxSIA1OaTOu8Nos3CalXbJ2DrKnntMv6");
	}

	[ContextMenu("Support Group")]
	private void SupportGroup() {
		Application.OpenURL("https://groups.google.com/forum/#!forum/final-ik");
	}

	[ContextMenu("Asset Store Thread")]
	private void ASThread() {
		Application.OpenURL(
			"http://forum.unity3d.com/threads/final-ik-full-body-ik-aim-look-at-fabrik-ccd-ik-1-0-released.222685/");
	}

	public bool inInteraction {
		get {
			if (!IsValid(true))
				return false;
			for (var index = 0; index < interactionEffectors.Length; ++index)
				if (interactionEffectors[index].inInteraction && !interactionEffectors[index].isPaused)
					return true;
			return false;
		}
	}

	public bool IsInInteraction(FullBodyBipedEffector effectorType) {
		if (!IsValid(true))
			return false;
		for (var index = 0; index < interactionEffectors.Length; ++index)
			if (interactionEffectors[index].effectorType == effectorType)
				return interactionEffectors[index].inInteraction && !interactionEffectors[index].isPaused;
		return false;
	}

	public bool IsPaused(FullBodyBipedEffector effectorType) {
		if (!IsValid(true))
			return false;
		for (var index = 0; index < interactionEffectors.Length; ++index)
			if (interactionEffectors[index].effectorType == effectorType)
				return interactionEffectors[index].inInteraction && interactionEffectors[index].isPaused;
		return false;
	}

	public bool IsPaused() {
		if (!IsValid(true))
			return false;
		for (var index = 0; index < interactionEffectors.Length; ++index)
			if (interactionEffectors[index].inInteraction && interactionEffectors[index].isPaused)
				return true;
		return false;
	}

	public bool IsInSync() {
		if (!IsValid(true))
			return false;
		for (var index1 = 0; index1 < interactionEffectors.Length; ++index1)
			if (interactionEffectors[index1].isPaused)
				for (var index2 = 0; index2 < interactionEffectors.Length; ++index2)
					if (index2 != index1 && interactionEffectors[index2].inInteraction &&
					    !interactionEffectors[index2].isPaused)
						return false;
		return true;
	}

	public bool StartInteraction(
		FullBodyBipedEffector effectorType,
		InteractionObject interactionObject,
		bool interrupt) {
		if (!IsValid(true) || interactionObject == null)
			return false;
		for (var index = 0; index < interactionEffectors.Length; ++index)
			if (interactionEffectors[index].effectorType == effectorType)
				return interactionEffectors[index].Start(interactionObject, targetTag, fadeInTime, interrupt);
		return false;
	}

	public bool PauseInteraction(FullBodyBipedEffector effectorType) {
		if (!IsValid(true))
			return false;
		for (var index = 0; index < interactionEffectors.Length; ++index)
			if (interactionEffectors[index].effectorType == effectorType)
				return interactionEffectors[index].Pause();
		return false;
	}

	public bool ResumeInteraction(FullBodyBipedEffector effectorType) {
		if (!IsValid(true))
			return false;
		for (var index = 0; index < interactionEffectors.Length; ++index)
			if (interactionEffectors[index].effectorType == effectorType)
				return interactionEffectors[index].Resume();
		return false;
	}

	public bool StopInteraction(FullBodyBipedEffector effectorType) {
		if (!IsValid(true))
			return false;
		for (var index = 0; index < interactionEffectors.Length; ++index)
			if (interactionEffectors[index].effectorType == effectorType)
				return interactionEffectors[index].Stop();
		return false;
	}

	public void PauseAll() {
		if (!IsValid(true))
			return;
		for (var index = 0; index < interactionEffectors.Length; ++index)
			interactionEffectors[index].Pause();
	}

	public void ResumeAll() {
		if (!IsValid(true))
			return;
		for (var index = 0; index < interactionEffectors.Length; ++index)
			interactionEffectors[index].Resume();
	}

	public void StopAll() {
		for (var index = 0; index < interactionEffectors.Length; ++index)
			interactionEffectors[index].Stop();
	}

	public InteractionObject GetInteractionObject(FullBodyBipedEffector effectorType) {
		if (!IsValid(true))
			return null;
		for (var index = 0; index < interactionEffectors.Length; ++index)
			if (interactionEffectors[index].effectorType == effectorType)
				return interactionEffectors[index].interactionObject;
		return null;
	}

	public float GetProgress(FullBodyBipedEffector effectorType) {
		if (!IsValid(true))
			return 0.0f;
		for (var index = 0; index < interactionEffectors.Length; ++index)
			if (interactionEffectors[index].effectorType == effectorType)
				return interactionEffectors[index].progress;
		return 0.0f;
	}

	public float GetMinActiveProgress() {
		if (!IsValid(true))
			return 0.0f;
		var minActiveProgress = 1f;
		for (var index = 0; index < interactionEffectors.Length; ++index)
			if (interactionEffectors[index].inInteraction) {
				var progress = interactionEffectors[index].progress;
				if (progress > 0.0 && progress < (double)minActiveProgress)
					minActiveProgress = progress;
			}

		return minActiveProgress;
	}

	public bool TriggerInteraction(int index, bool interrupt) {
		if (!IsValid(true) || !TriggerIndexIsValid(index))
			return false;
		var flag = true;
		var range = triggersInRange[index].ranges[bestRangeIndexes[index]];
		for (var index1 = 0; index1 < range.interactions.Length; ++index1) {
			for (var index2 = 0; index2 < range.interactions[index1].effectors.Length; ++index2)
				if (!StartInteraction(range.interactions[index1].effectors[index2],
					    range.interactions[index1].interactionObject, interrupt))
					flag = false;
		}

		return flag;
	}

	public bool TriggerInteraction(
		int index,
		bool interrupt,
		out InteractionObject interactionObject) {
		interactionObject = null;
		if (!IsValid(true) || !TriggerIndexIsValid(index))
			return false;
		var flag = true;
		var range = triggersInRange[index].ranges[bestRangeIndexes[index]];
		for (var index1 = 0; index1 < range.interactions.Length; ++index1) {
			for (var index2 = 0; index2 < range.interactions[index1].effectors.Length; ++index2) {
				interactionObject = range.interactions[index1].interactionObject;
				if (!StartInteraction(range.interactions[index1].effectors[index2], interactionObject, interrupt))
					flag = false;
			}
		}

		return flag;
	}

	public bool TriggerInteraction(
		int index,
		bool interrupt,
		out InteractionTarget interactionTarget) {
		interactionTarget = null;
		if (!IsValid(true) || !TriggerIndexIsValid(index))
			return false;
		var flag = true;
		var range = triggersInRange[index].ranges[bestRangeIndexes[index]];
		for (var index1 = 0; index1 < range.interactions.Length; ++index1) {
			for (var index2 = 0; index2 < range.interactions[index1].effectors.Length; ++index2) {
				var interactionObject = range.interactions[index1].interactionObject;
				var target = interactionObject.GetTarget(range.interactions[index1].effectors[index2], tag);
				if (target != null)
					interactionTarget = target.GetComponent<InteractionTarget>();
				if (!StartInteraction(range.interactions[index1].effectors[index2], interactionObject, interrupt))
					flag = false;
			}
		}

		return flag;
	}

	public InteractionTrigger.Range GetClosestInteractionRange() {
		if (!IsValid(true))
			return null;
		var closestTriggerIndex = GetClosestTriggerIndex();
		return closestTriggerIndex < 0 || closestTriggerIndex >= triggersInRange.Count
			? null
			: triggersInRange[closestTriggerIndex].ranges[bestRangeIndexes[closestTriggerIndex]];
	}

	public InteractionObject GetClosestInteractionObjectInRange() {
		return GetClosestInteractionRange()?.interactions[0].interactionObject;
	}

	public InteractionTarget GetClosestInteractionTargetInRange() {
		var interactionRange = GetClosestInteractionRange();
		return interactionRange?.interactions[0].interactionObject
			.GetTarget(interactionRange.interactions[0].effectors[0], this);
	}

	public InteractionObject[] GetClosestInteractionObjectsInRange() {
		var interactionRange = GetClosestInteractionRange();
		if (interactionRange == null)
			return new InteractionObject[0];
		var interactionObjectsInRange = new InteractionObject[interactionRange.interactions.Length];
		for (var index = 0; index < interactionRange.interactions.Length; ++index)
			interactionObjectsInRange[index] = interactionRange.interactions[index].interactionObject;
		return interactionObjectsInRange;
	}

	public InteractionTarget[] GetClosestInteractionTargetsInRange() {
		var interactionRange = GetClosestInteractionRange();
		if (interactionRange == null)
			return new InteractionTarget[0];
		var interactionTargetList = new List<InteractionTarget>();
		foreach (var interaction in interactionRange.interactions) {
			foreach (var effector in interaction.effectors)
				interactionTargetList.Add(interaction.interactionObject.GetTarget(effector, this));
		}

		return interactionTargetList.ToArray();
	}

	public bool TriggerEffectorsReady(int index) {
		if (!IsValid(true) || !TriggerIndexIsValid(index))
			return false;
		for (var index1 = 0; index1 < triggersInRange[index].ranges.Length; ++index1) {
			var range = triggersInRange[index].ranges[index1];
			for (var index2 = 0; index2 < range.interactions.Length; ++index2) {
				for (var index3 = 0; index3 < range.interactions[index2].effectors.Length; ++index3)
					if (IsInInteraction(range.interactions[index2].effectors[index3]))
						return false;
			}

			for (var index4 = 0; index4 < range.interactions.Length; ++index4) {
				for (var index5 = 0; index5 < range.interactions[index4].effectors.Length; ++index5)
					if (IsPaused(range.interactions[index4].effectors[index5]))
						for (var index6 = 0; index6 < range.interactions[index4].effectors.Length; ++index6)
							if (index6 != index5 && !IsPaused(range.interactions[index4].effectors[index6]))
								return false;
			}
		}

		return true;
	}

	public InteractionTrigger.Range GetTriggerRange(int index) {
		if (!IsValid(true))
			return null;
		if (index >= 0 && index < bestRangeIndexes.Count)
			return triggersInRange[index].ranges[bestRangeIndexes[index]];
		Warning.Log("Index out of range.", transform);
		return null;
	}

	public int GetClosestTriggerIndex() {
		if (!IsValid(true) || triggersInRange.Count == 0)
			return -1;
		if (triggersInRange.Count == 1)
			return 0;
		var closestTriggerIndex = -1;
		var num1 = float.PositiveInfinity;
		for (var index = 0; index < triggersInRange.Count; ++index)
			if (triggersInRange[index] != null) {
				var num2 = Vector3.SqrMagnitude(triggersInRange[index].transform.position - transform.position);
				if (num2 < (double)num1) {
					closestTriggerIndex = index;
					num1 = num2;
				}
			}

		return closestTriggerIndex;
	}

	public FullBodyBipedIK ik {
		get => fullBody;
		set => fullBody = value;
	}

	public List<InteractionTrigger> triggersInRange { get; private set; }

	protected virtual void Start() {
		if (fullBody == null)
			fullBody = GetComponent<FullBodyBipedIK>();
		if (fullBody == null)
			Warning.Log("InteractionSystem can not find a FullBodyBipedIK component", transform);
		else {
			var solver1 = fullBody.solver;
			solver1.OnPreUpdate = solver1.OnPreUpdate + OnPreFBBIK;
			var solver2 = fullBody.solver;
			solver2.OnPostUpdate = solver2.OnPostUpdate + OnPostFBBIK;
			var solver3 = fullBody.solver;
			solver3.OnFixTransforms = solver3.OnFixTransforms + OnFixTransforms;
			OnInteractionStart += LookAtInteraction;
			OnInteractionPause += InteractionPause;
			OnInteractionResume += InteractionResume;
			OnInteractionStop += InteractionStop;
			foreach (var interactionEffector in interactionEffectors)
				interactionEffector.Initiate(this);
			triggersInRange = new List<InteractionTrigger>();
			c = GetComponent<Collider>();
			UpdateTriggerEventBroadcasting();
			initiated = true;
		}
	}

	private void InteractionPause(
		FullBodyBipedEffector effector,
		InteractionObject interactionObject) {
		lookAt.isPaused = true;
	}

	private void InteractionResume(
		FullBodyBipedEffector effector,
		InteractionObject interactionObject) {
		lookAt.isPaused = false;
	}

	private void InteractionStop(
		FullBodyBipedEffector effector,
		InteractionObject interactionObject) {
		lookAt.isPaused = false;
	}

	private void LookAtInteraction(
		FullBodyBipedEffector effector,
		InteractionObject interactionObject) {
		lookAt.Look(interactionObject.lookAtTarget, Time.time + interactionObject.length * 0.5f);
	}

	public void OnTriggerEnter(Collider c) {
		if (fullBody == null)
			return;
		var component = c.GetComponent<InteractionTrigger>();
		if (component == null || inContact.Contains(component))
			return;
		inContact.Add(component);
	}

	public void OnTriggerExit(Collider c) {
		if (fullBody == null)
			return;
		var component = c.GetComponent<InteractionTrigger>();
		if (component == null)
			return;
		inContact.Remove(component);
	}

	private bool ContactIsInRange(int index, out int bestRangeIndex) {
		bestRangeIndex = -1;
		if (!IsValid(true))
			return false;
		if (index < 0 || index >= inContact.Count) {
			Warning.Log("Index out of range.", transform);
			return false;
		}

		if (inContact[index] == null) {
			Warning.Log("The InteractionTrigger in the list 'inContact' has been destroyed", transform);
			return false;
		}

		bestRangeIndex = inContact[index].GetBestRangeIndex(transform, FPSCamera, raycastHit);
		return bestRangeIndex != -1;
	}

	private void OnDrawGizmosSelected() {
		if (Application.isPlaying)
			return;
		if (fullBody == null)
			fullBody = GetComponent<FullBodyBipedIK>();
		if (!(characterCollider == null))
			return;
		characterCollider = GetComponent<Collider>();
	}

	private void Update() {
		if (fullBody == null)
			return;
		UpdateTriggerEventBroadcasting();
		Raycasting();
		triggersInRange.Clear();
		bestRangeIndexes.Clear();
		for (var index = 0; index < inContact.Count; ++index) {
			var bestRangeIndex = -1;
			if (inContact[index] != null && inContact[index].gameObject.activeInHierarchy && inContact[index].enabled &&
			    ContactIsInRange(index, out bestRangeIndex)) {
				triggersInRange.Add(inContact[index]);
				bestRangeIndexes.Add(bestRangeIndex);
			}
		}

		lookAt.Update();
	}

	private void Raycasting() {
		if (camRaycastLayers == -1 || FPSCamera == null)
			return;
		Physics.Raycast(FPSCamera.position, FPSCamera.forward, out raycastHit, camRaycastDistance, camRaycastLayers);
	}

	private void UpdateTriggerEventBroadcasting() {
		if (characterCollider == null)
			characterCollider = c;
		if (characterCollider != null && characterCollider != c) {
			if (characterCollider.GetComponent<TriggerEventBroadcaster>() == null)
				characterCollider.gameObject.AddComponent<TriggerEventBroadcaster>().target = gameObject;
			if (lastCollider != null && lastCollider != c && lastCollider != characterCollider) {
				var component = lastCollider.GetComponent<TriggerEventBroadcaster>();
				if (component != null)
					Destroy(component);
			}
		}

		lastCollider = characterCollider;
	}

	private void UpdateEffectors() {
		if (fullBody == null)
			return;
		for (var index = 0; index < interactionEffectors.Length; ++index)
			interactionEffectors[index].Update(transform, speed);
		for (var index = 0; index < interactionEffectors.Length; ++index)
			interactionEffectors[index].ResetToDefaults(resetToDefaultsSpeed * speed);
	}

	private void OnPreFBBIK() {
		if (!enabled || fullBody == null || !fullBody.enabled)
			return;
		lookAt.SolveSpine();
		UpdateEffectors();
	}

	private void OnPostFBBIK() {
		if (!enabled || fullBody == null || !fullBody.enabled)
			return;
		for (var index = 0; index < interactionEffectors.Length; ++index)
			interactionEffectors[index].OnPostFBBIK();
		lookAt.SolveHead();
	}

	private void OnFixTransforms() {
		lookAt.OnFixTransforms();
	}

	private void OnDestroy() {
		if (fullBody == null)
			return;
		var solver1 = fullBody.solver;
		solver1.OnPreUpdate = solver1.OnPreUpdate - OnPreFBBIK;
		var solver2 = fullBody.solver;
		solver2.OnPostUpdate = solver2.OnPostUpdate - OnPostFBBIK;
		var solver3 = fullBody.solver;
		solver3.OnFixTransforms = solver3.OnFixTransforms - OnFixTransforms;
		OnInteractionStart -= LookAtInteraction;
		OnInteractionPause -= InteractionPause;
		OnInteractionResume -= InteractionResume;
		OnInteractionStop -= InteractionStop;
	}

	private bool IsValid(bool log) {
		if (fullBody == null) {
			if (log)
				Warning.Log("FBBIK is null. Will not update the InteractionSystem", transform);
			return false;
		}

		if (initiated)
			return true;
		if (log)
			Warning.Log("The InteractionSystem has not been initiated yet.", transform);
		return false;
	}

	private bool TriggerIndexIsValid(int index) {
		if (index < 0 || index >= triggersInRange.Count) {
			Warning.Log("Index out of range.", transform);
			return false;
		}

		if (!(triggersInRange[index] == null))
			return true;
		Warning.Log("The InteractionTrigger in the list 'inContact' has been destroyed", transform);
		return false;
	}

	[ContextMenu("User Manual")]
	private void OpenUserManual() {
		Application.OpenURL("http://www.root-motion.com/finalikdox/html/page10.html");
	}

	[ContextMenu("Scrpt Reference")]
	private void OpenScriptReference() {
		Application.OpenURL(
			"http://www.root-motion.com/finalikdox/html/class_root_motion_1_1_final_i_k_1_1_interaction_system.html");
	}

	public delegate void InteractionDelegate(
		FullBodyBipedEffector effectorType,
		InteractionObject interactionObject);

	public delegate void InteractionEventDelegate(
		FullBodyBipedEffector effectorType,
		InteractionObject interactionObject,
		InteractionObject.InteractionEvent interactionEvent);
}
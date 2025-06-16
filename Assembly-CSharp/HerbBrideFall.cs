using Engine.Behaviours.Components;
using Engine.Common.Services;
using Engine.Source.Audio;
using Engine.Source.Commons;
using Engine.Source.Services;
using Engine.Source.Services.CameraServices;
using Inspectors;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HerbBrideFall : MonoBehaviour {
	[SerializeField] private AudioClip impactAudioClip;
	[SerializeField] private AudioSource termitnikAudiosource;
	[SerializeField] private AudioSource womanCryAudiosource;
	private float jumpTime = 2f;
	private float maxPlayerDistance = 40f;
	[Inspected] private bool IsGame;
	private Animator animator;
	private StateEnum state;
	private Quaternion initialRotation;
	private Vector3 initialPosition;
	private Pivot pivot;
	private AudioSource audioSource;
	private float lookTime;
	private float termitnikVolume;
	private Collider collider;
	private Collider[] colliders = new Collider[64];

	public void Wait() {
		SetState(StateEnum.Waiting);
	}

	public void Jump() {
		SetState(StateEnum.Falling);
	}

	private void Start() {
		IsGame = SceneManager.GetActiveScene().name != "TermitnikFall";
		initialRotation = transform.rotation;
		initialPosition = transform.position;
		pivot = GetComponent<Pivot>();
		animator = pivot.GetAnimator();
		audioSource = GetComponent<AudioSource>();
		pivot.GetAnimatorEventProxy().AnimatorMoveEvent += HerbBrideFall_AnimatorMoveEvent;
		collider = GetComponent<Collider>();
		SetState(StateEnum.Waiting);
	}

	private void HerbBrideFall_AnimatorMoveEvent() {
		transform.position += animator.deltaPosition;
		transform.rotation *= animator.deltaRotation;
	}

	private void SetState(StateEnum state) {
		switch (state) {
			case StateEnum.Waiting:
				animator.SetTrigger("Triggers/Reset");
				animator.ResetTrigger("Triggers/Fall");
				transform.SetPositionAndRotation(initialPosition, initialRotation);
				break;
			case StateEnum.Falling:
				if (IsGame)
					ServiceLocator.GetService<LogicEventService>().FireCommonEvent("Termitnik_Jump");
				animator.ResetTrigger("Triggers/Reset");
				animator.SetTrigger("Triggers/Fall");
				audioSource.PlayAndCheck();
				break;
			case StateEnum.Dead:
				audioSource.Stop();
				audioSource.PlayOneShot(impactAudioClip);
				break;
		}

		this.state = state;
	}

	private void Update() {
		switch (state) {
			case StateEnum.Waiting:
				UpdatePlayerSeesHerbBride();
				collider.enabled = true;
				break;
			case StateEnum.Falling:
				collider.enabled = false;
				lookTime = 0.0f;
				if (IsAboutToCollideWithTerrain()) SetState(StateEnum.Dead);
				break;
			case StateEnum.Dead:
				collider.enabled = false;
				lookTime = 0.0f;
				if (!IsGame && Input.GetKeyDown(KeyCode.E)) SetState(StateEnum.Waiting);
				break;
		}

		UpdateTermitnikAudio();
		UpdateCryingAudio();
	}

	private void UpdateTermitnikAudio() {
		if (termitnikAudiosource == null)
			return;
		var playerGameObject = GetPlayerGameObject();
		if (playerGameObject == null)
			return;
		var magnitude = (playerGameObject.transform.position - transform.position).magnitude;
		if (state == StateEnum.Waiting) {
			termitnikAudiosource.spatialBlend = Mathf.Clamp01(magnitude / 50f);
			termitnikVolume = Mathf.MoveTowards(termitnikVolume, 1f, Time.deltaTime / 4f);
			termitnikAudiosource.volume = termitnikVolume;
			if (termitnikAudiosource.isPlaying)
				return;
			termitnikAudiosource.PlayAndCheck();
		} else {
			termitnikVolume = Mathf.MoveTowards(termitnikVolume, 0.0f, Time.deltaTime / 4f);
			termitnikAudiosource.volume = termitnikVolume;
		}
	}

	private void UpdateCryingAudio() {
		if (womanCryAudiosource == null)
			return;
		var playerGameObject = GetPlayerGameObject();
		if (playerGameObject == null)
			return;
		var magnitude = (playerGameObject.transform.position - transform.position).magnitude;
		if (state == StateEnum.Waiting) {
			womanCryAudiosource.volume = 1f;
			if (womanCryAudiosource.isPlaying)
				return;
			womanCryAudiosource.PlayAndCheck();
		} else {
			if (!womanCryAudiosource.isPlaying)
				return;
			womanCryAudiosource.Stop();
		}
	}

	private bool IsAboutToCollideWithTerrain() {
		var num = Physics.OverlapSphereNonAlloc(animator.gameObject.transform.position, 3f, colliders);
		for (var index = 0; index < num; ++index)
			if (colliders[index].gameObject.GetComponent<TerrainCollider>() != null)
				return true;
		return false;
	}

	private Camera GetCamera() {
		if (!IsGame)
			return Camera.main;
		return ServiceLocator.GetService<CameraService>().Kind != CameraKindEnum.FirstPerson_Controlling
			? null
			: GameCamera.Instance.Camera;
	}

	private GameObject GetPlayerGameObject() {
		if (!IsGame)
			return GameObject.Find("FPSController");
		return ((IEntityView)ServiceLocator.GetService<ISimulation>().Player)?.GameObject;
	}

	private void OnDrawGizmos() {
		var camera = GetCamera();
		var playerGameObject = GetPlayerGameObject();
		if (camera == null || playerGameObject == null)
			return;
		var forward = camera.transform.forward;
		var vector3_1 = transform.position + 1f * transform.forward;
		var vector3_2 = playerGameObject.transform.position + Vector3.up + forward * 1f;
		var vector3_3 = vector3_2 - vector3_1;
		var magnitude = vector3_3.magnitude;
		var vector3_4 = vector3_3 / magnitude;
		Gizmos.color = Color.red;
		Gizmos.DrawSphere(vector3_1, 0.2f);
		Gizmos.DrawSphere(vector3_2 + forward * 2f, 0.2f);
		Gizmos.color = Color.green;
		Gizmos.DrawLine(vector3_1, vector3_1 + maxPlayerDistance * vector3_4);
	}

	private void UpdatePlayerSeesHerbBride() {
		var camera = GetCamera();
		var playerGameObject = GetPlayerGameObject();
		if (camera == null || playerGameObject == null)
			return;
		var forward = camera.transform.forward;
		var origin = transform.position + 1f * transform.forward;
		var vector3 = playerGameObject.transform.position + Vector3.up + forward * 1f - origin;
		var magnitude = vector3.magnitude;
		var direction = vector3 / magnitude;
		if (magnitude > (double)maxPlayerDistance)
			lookTime = 0.0f;
		else if (Vector3.Dot(-direction, forward) < (double)Mathf.Cos(0.5235988f))
			lookTime = 0.0f;
		else {
			RaycastHit hitInfo;
			if (Physics.Raycast(origin, direction, out hitInfo, magnitude)) {
				Debug.Log(hitInfo.collider.gameObject);
				lookTime = 0.0f;
			} else {
				lookTime += Time.deltaTime;
				if (lookTime <= (double)jumpTime)
					return;
				Jump();
			}
		}
	}

	private enum StateEnum {
		Waiting,
		Falling,
		Dead
	}
}
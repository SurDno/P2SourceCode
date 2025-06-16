using Engine.Common;
using Engine.Common.Services;
using Engine.Source.Audio;
using Engine.Source.Commons;
using Engine.Source.Components.Utilities;
using Inspectors;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlagueSpotWithPlayerTrigger : MonoBehaviour {
	[SerializeField] private float growTime = 5f;
	[Inspected] private bool IsGame;
	private Material plagueSpotMaterial;
	[Inspected] private State state = State.Unknown;
	[Inspected] private float level;
	private AudioSource audioSource;
	private float volume;

	private void SetState(State state) {
		switch (state) {
			case State.Waiting:
				level = 0.0f;
				plagueSpotMaterial.SetFloat("_Level", 0.0f);
				break;
			case State.Growing:
				level = 0.0f;
				audioSource.PlayAndCheck();
				plagueSpotMaterial.SetFloat("_Level", 0.0f);
				break;
		}

		this.state = state;
	}

	private void UpdateState() {
		switch (state) {
			case State.Growing:
				level = Mathf.Clamp01(level + Time.deltaTime / growTime);
				if (Mathf.Approximately(level, 1f))
					SetState(State.Done);
				plagueSpotMaterial.SetFloat("_Level", Mathf.Clamp01(SmoothUtility.Smooth22(Mathf.Sqrt(level))));
				if (!audioSource.isPlaying)
					audioSource.PlayAndCheck();
				volume = Mathf.MoveTowards(volume, 1f, Time.deltaTime / 2f);
				audioSource.volume = volume;
				break;
			case State.Done:
				volume = Mathf.MoveTowards(volume, 0.0f, Time.deltaTime / 2f);
				audioSource.volume = volume;
				break;
		}
	}

	private void Start() {
		plagueSpotMaterial = gameObject.GetComponent<MeshRenderer>().material;
		IsGame = SceneManager.GetActiveScene().name != "PlagueIntro_Riot_Loader";
		audioSource = GetComponent<AudioSource>();
		SetState(State.Waiting);
	}

	private void Update() {
		UpdateState();
	}

	private void OnTriggerEnter(Collider other) {
		var playerGameObject = GetPlayerGameObject();
		if (playerGameObject == null || !(playerGameObject == other.gameObject) || state != State.Waiting)
			return;
		SetState(State.Growing);
	}

	private GameObject GetPlayerGameObject() {
		if (!IsGame)
			return GameObject.Find("FPSController");
		var player = ServiceLocator.GetService<ISimulation>().Player;
		if (player == null)
			return null;
		return ((IEntityView)player)?.GameObject;
	}

	private enum State {
		Unknown,
		Waiting,
		Growing,
		Done
	}
}
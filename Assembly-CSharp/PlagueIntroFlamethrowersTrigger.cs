using Engine.Common.Services;
using Engine.Source.Commons;
using Engine.Source.Services;
using Inspectors;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.Timeline;

public class PlagueIntroFlamethrowersTrigger : MonoBehaviour {
	[Inspected] private bool IsGame;
	[SerializeField] private PlayableDirector director;
	[SerializeField] private TimelineAsset timelineIdle;
	[SerializeField] private TimelineAsset timeline;
	private bool done;

	private void OnEnable() {
		IsGame = SceneManager.GetActiveScene().name != "PlagueIntro_Riot_Loader";
		director.Play(timelineIdle, DirectorWrapMode.Loop);
		if (!IsGame)
			return;
		InstanceByRequest<EngineApplication>.Instance.OnPauseEvent += OnPauseEvent;
	}

	private void OnDisable() {
		if (!IsGame)
			return;
		InstanceByRequest<EngineApplication>.Instance.OnPauseEvent -= OnPauseEvent;
	}

	private void OnPauseEvent() {
		if (!IsGame)
			return;
		if (InstanceByRequest<EngineApplication>.Instance.IsPaused)
			director.Pause();
		else
			director.Resume();
	}

	private void OnTriggerEnter(Collider other) {
		if (done)
			return;
		var playerGameObject = GetPlayerGameObject();
		if (playerGameObject == null || !(playerGameObject == other.gameObject))
			return;
		director.Play(timeline, DirectorWrapMode.Hold);
		done = true;
		if (IsGame)
			ServiceLocator.GetService<LogicEventService>().FireCommonEvent("PlagueIntroFlamethrowersDone");
	}

	private GameObject GetPlayerGameObject() {
		if (!IsGame)
			return GameObject.Find("FPSController");
		return ((IEntityView)ServiceLocator.GetService<ISimulation>().Player)?.GameObject;
	}
}
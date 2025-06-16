using Engine.Common.Services;
using Engine.Source.Commons;
using Engine.Source.Services;
using Inspectors;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class PlagueIntroRiotTrigger : MonoBehaviour
{
  [Inspected]
  private bool IsGame;
  [SerializeField]
  private PlayableDirector director;
  private bool done;

  private void OnEnable()
  {
    IsGame = SceneManager.GetActiveScene().name != "PlagueIntro_Riot_Loader";
    if (!IsGame)
      return;
    InstanceByRequest<EngineApplication>.Instance.OnPauseEvent += OnPauseEvent;
  }

  private void OnPauseEvent()
  {
    if (!IsGame)
      return;
    if (InstanceByRequest<EngineApplication>.Instance.IsPaused)
      director.Pause();
    else
      director.Resume();
  }

  private void OnDisable()
  {
    if (!IsGame)
      return;
    InstanceByRequest<EngineApplication>.Instance.OnPauseEvent -= OnPauseEvent;
  }

  private void OnTriggerEnter(Collider other)
  {
    if (done)
      return;
    GameObject playerGameObject = GetPlayerGameObject();
    if (playerGameObject == null || !(playerGameObject == other.gameObject))
      return;
    director.Play();
    done = true;
    if (IsGame)
      ServiceLocator.GetService<LogicEventService>().FireCommonEvent("PlagueIntroRiotDone");
  }

  private GameObject GetPlayerGameObject()
  {
    if (!IsGame)
      return GameObject.Find("FPSController");
    return ((IEntityView) ServiceLocator.GetService<ISimulation>().Player)?.GameObject;
  }
}

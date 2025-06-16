using Engine.Common.Services;
using Engine.Source.Commons;
using Engine.Source.Services;
using Inspectors;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class PlagueIntroExplosionTrigger : MonoBehaviour
{
  [Inspected]
  private bool IsGame;
  [SerializeField]
  private PlayableDirector director;
  private bool done;

  private void Start()
  {
    IsGame = SceneManager.GetActiveScene().name != "PlagueIntro_Riot_Loader";
  }

  private void OnTriggerEnter(Collider other)
  {
    if (done)
      return;
    GameObject playerGameObject = GetPlayerGameObject();
    if (playerGameObject == null || !(playerGameObject == other.gameObject))
      return;
    director.Play();
    director.stopped += Director_stopped;
    done = true;
    if (IsGame)
      ServiceLocator.GetService<LogicEventService>().FireCommonEvent("PlagueIntroWindowDone");
  }

  private void Director_stopped(PlayableDirector obj)
  {
    director.stopped -= Director_stopped;
  }

  private GameObject GetPlayerGameObject()
  {
    if (!IsGame)
      return GameObject.Find("FPSController");
    return ((IEntityView) ServiceLocator.GetService<ISimulation>().Player)?.GameObject;
  }
}

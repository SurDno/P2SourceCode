using Engine.Common.Services;
using Engine.Source.Commons;
using Engine.Source.Services;
using Inspectors;
using System;
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
    this.IsGame = SceneManager.GetActiveScene().name != "PlagueIntro_Riot_Loader";
  }

  private void OnTriggerEnter(Collider other)
  {
    if (this.done)
      return;
    GameObject playerGameObject = this.GetPlayerGameObject();
    if ((UnityEngine.Object) playerGameObject == (UnityEngine.Object) null || !((UnityEngine.Object) playerGameObject == (UnityEngine.Object) other.gameObject))
      return;
    this.director.Play();
    this.director.stopped += new Action<PlayableDirector>(this.Director_stopped);
    this.done = true;
    if (this.IsGame)
      ServiceLocator.GetService<LogicEventService>().FireCommonEvent("PlagueIntroWindowDone");
  }

  private void Director_stopped(PlayableDirector obj)
  {
    this.director.stopped -= new Action<PlayableDirector>(this.Director_stopped);
  }

  private GameObject GetPlayerGameObject()
  {
    if (!this.IsGame)
      return GameObject.Find("FPSController");
    return ((IEntityView) ServiceLocator.GetService<ISimulation>().Player)?.GameObject;
  }
}

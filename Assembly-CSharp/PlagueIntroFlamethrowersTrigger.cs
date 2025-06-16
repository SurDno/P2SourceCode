using Engine.Common.Services;
using Engine.Source.Commons;
using Engine.Source.Services;
using Inspectors;
using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.Timeline;

public class PlagueIntroFlamethrowersTrigger : MonoBehaviour
{
  [Inspected]
  private bool IsGame;
  [SerializeField]
  private PlayableDirector director;
  [SerializeField]
  private TimelineAsset timelineIdle;
  [SerializeField]
  private TimelineAsset timeline;
  private bool done;

  private void OnEnable()
  {
    this.IsGame = SceneManager.GetActiveScene().name != "PlagueIntro_Riot_Loader";
    this.director.Play((PlayableAsset) this.timelineIdle, DirectorWrapMode.Loop);
    if (!this.IsGame)
      return;
    InstanceByRequest<EngineApplication>.Instance.OnPauseEvent += new Action(this.OnPauseEvent);
  }

  private void OnDisable()
  {
    if (!this.IsGame)
      return;
    InstanceByRequest<EngineApplication>.Instance.OnPauseEvent -= new Action(this.OnPauseEvent);
  }

  private void OnPauseEvent()
  {
    if (!this.IsGame)
      return;
    if (InstanceByRequest<EngineApplication>.Instance.IsPaused)
      this.director.Pause();
    else
      this.director.Resume();
  }

  private void OnTriggerEnter(Collider other)
  {
    if (this.done)
      return;
    GameObject playerGameObject = this.GetPlayerGameObject();
    if ((UnityEngine.Object) playerGameObject == (UnityEngine.Object) null || !((UnityEngine.Object) playerGameObject == (UnityEngine.Object) other.gameObject))
      return;
    this.director.Play((PlayableAsset) this.timeline, DirectorWrapMode.Hold);
    this.done = true;
    if (this.IsGame)
      ServiceLocator.GetService<LogicEventService>().FireCommonEvent("PlagueIntroFlamethrowersDone");
  }

  private GameObject GetPlayerGameObject()
  {
    if (!this.IsGame)
      return GameObject.Find("FPSController");
    return ((IEntityView) ServiceLocator.GetService<ISimulation>().Player)?.GameObject;
  }
}

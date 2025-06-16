using Engine.Common;
using Engine.Common.Components;
using Engine.Common.Services;
using Engine.Source.Audio;
using Engine.Source.Commons;
using Engine.Source.Components;
using Engine.Source.Components.Crowds;
using Engine.Source.Debugs;
using Engine.Source.Services.Gizmos;
using Inspectors;
using System;
using UnityEngine;
using UnityEngine.Audio;

public class HerbRoots : MonoBehaviour, IEntityAttachable
{
  [Header("Топология")]
  [Tooltip("Геометрия корней, она будет подниматься и опускаться")]
  [SerializeField]
  private GameObject rootsGeometry;
  [Tooltip("На сколько нужно погрузить корень под землю (положительное значение)")]
  [SerializeField]
  private float verticalOffset = 1f;
  [Tooltip("Радиус триггера активации")]
  [SerializeField]
  private float radius = 5f;
  [Header("Геймплей")]
  [Tooltip("Время активации в секундах")]
  [SerializeField]
  private float activationStayTime = 25f;
  [Tooltip("время выползания корней из земли в секундах")]
  [SerializeField]
  private float activationTime = 10f;
  [Header("Звуки привлечения")]
  [SerializeField]
  private AudioMixerGroup mixer;
  [SerializeField]
  private float attractMinDistance = 1f;
  [SerializeField]
  private float attractMaxDistance = 50f;
  [Tooltip("3д звук, привлекающий внимание к корню")]
  [SerializeField]
  private AudioClip attractLoopSound;
  private float attractLoopSoundVolume;
  [Header("Звуки внутри триггера")]
  [SerializeField]
  private float enterTriggerMinDistance = 2f;
  [SerializeField]
  private float enterTriggerMaxDistance = 10f;
  [Tooltip("3д звук входа в триггер")]
  [SerializeField]
  private AudioClip enterTriggerOneshotSound;
  [Tooltip("3д звук стояния в триггере")]
  [SerializeField]
  private AudioClip enterTriggerLoopSound;
  private float enterTriggerLoopSoundVolume;
  [Tooltip("3д прорастания корня")]
  [SerializeField]
  private AudioClip rootReleaseOneshotSound;
  [Header("Звуки взаимодействия")]
  [SerializeField]
  private AudioClip giveBloodSound;
  [Inspected]
  private HerbRootsStateEnum state;
  private HerbRootsComponent herbRootsComponent;
  private Vector3 initialPosition;
  [Inspected]
  private float activationStayTimeLeft;
  [Inspected]
  private float activationTimeLeft;
  private AudioSource attractAudiosource;
  private AudioSource enterTriggerAudiosource;
  private HerbRootsTrigger trigger;

  public float VerticalOffset => this.verticalOffset;

  [Inspected]
  public IEntity Owner { get; private set; }

  private void Awake()
  {
    if (!((UnityEngine.Object) this.rootsGeometry != (UnityEngine.Object) null))
      return;
    this.initialPosition = this.transform.position;
  }

  void IEntityAttachable.Attach(IEntity owner)
  {
    this.Owner = owner;
    this.herbRootsComponent = (HerbRootsComponent) owner.GetComponent<IHerbRootsComponent>();
    if (this.herbRootsComponent == null)
    {
      Debug.LogError((object) (typeof (HerbRootsComponent).Name + " : " + this.Owner.GetInfo()), (UnityEngine.Object) this.gameObject);
    }
    else
    {
      switch (this.herbRootsComponent.State)
      {
        case HerbRootsComponentStateEnum.Sleeping:
          this.SetState(HerbRootsStateEnum.Sleeping);
          break;
        case HerbRootsComponentStateEnum.Active:
          this.SetState(HerbRootsStateEnum.Active);
          break;
        default:
          this.SetState(HerbRootsStateEnum.Unknown);
          break;
      }
    }
  }

  void IEntityAttachable.Detach()
  {
    this.Owner = (IEntity) null;
    this.herbRootsComponent = (HerbRootsComponent) null;
  }

  private void SetState(HerbRootsStateEnum state)
  {
    switch (state)
    {
      case HerbRootsStateEnum.Sleeping:
        this.activationStayTimeLeft = this.activationStayTime;
        if ((UnityEngine.Object) this.rootsGeometry != (UnityEngine.Object) null)
        {
          this.rootsGeometry.transform.position = this.initialPosition + new Vector3(0.0f, -this.verticalOffset, 0.0f);
          break;
        }
        break;
      case HerbRootsStateEnum.MovingFromEarth:
        this.herbRootsComponent?.FireOnActivateStartEvent();
        this.activationTimeLeft = this.activationTime;
        SoundUtility.PlayAudioClip3D(this.transform, this.rootReleaseOneshotSound, this.mixer, 1f, this.enterTriggerMinDistance, this.enterTriggerMaxDistance, true, 0.0f);
        break;
      case HerbRootsStateEnum.Active:
        if ((UnityEngine.Object) this.rootsGeometry != (UnityEngine.Object) null)
          this.transform.position = this.initialPosition;
        this.herbRootsComponent.SetState(HerbRootsComponentStateEnum.Active);
        break;
    }
    this.state = state;
  }

  private void OnEnable()
  {
    GameObject gameObject = new GameObject("[Trigger] Player");
    gameObject.transform.SetParent(this.transform, false);
    this.trigger = gameObject.AddComponent<HerbRootsTrigger>();
    this.trigger.Radius = this.radius;
    this.trigger.PlayerEnterEvent += new Action(this.OnPlayerEnter);
    this.trigger.PlayerExitEvent += new Action(this.OnPlayerExit);
    this.attractAudiosource = this.gameObject.AddComponent<AudioSource>();
    this.attractAudiosource.clip = this.attractLoopSound;
    this.attractAudiosource.outputAudioMixerGroup = this.mixer;
    this.attractAudiosource.minDistance = this.attractMinDistance;
    this.attractAudiosource.maxDistance = this.attractMaxDistance;
    this.attractAudiosource.loop = true;
    this.attractAudiosource.rolloffMode = AudioRolloffMode.Linear;
    this.attractAudiosource.spatialBlend = 1f;
    this.attractLoopSoundVolume = 0.0f;
    this.attractAudiosource.Stop();
    this.enterTriggerAudiosource = this.gameObject.AddComponent<AudioSource>();
    this.enterTriggerAudiosource.clip = this.enterTriggerLoopSound;
    this.enterTriggerAudiosource.outputAudioMixerGroup = this.mixer;
    this.enterTriggerAudiosource.minDistance = this.enterTriggerMinDistance;
    this.enterTriggerAudiosource.maxDistance = this.enterTriggerMaxDistance;
    this.enterTriggerAudiosource.loop = true;
    this.enterTriggerAudiosource.spatialBlend = 0.5f;
    this.enterTriggerLoopSoundVolume = 0.0f;
    this.enterTriggerAudiosource.Stop();
  }

  private void OnDisable()
  {
    this.trigger.PlayerEnterEvent -= new Action(this.OnPlayerEnter);
    this.trigger.PlayerExitEvent -= new Action(this.OnPlayerExit);
    UnityEngine.Object.Destroy((UnityEngine.Object) this.trigger.gameObject);
    UnityEngine.Object.Destroy((UnityEngine.Object) this.attractAudiosource);
    UnityEngine.Object.Destroy((UnityEngine.Object) this.enterTriggerAudiosource);
  }

  public void PlayGiveBloodSound()
  {
    SoundUtility.PlayAudioClip2D(this.giveBloodSound, this.mixer, 1f, 0.0f);
  }

  private void OnPlayerEnter()
  {
    SoundUtility.PlayAudioClip3D(this.transform, this.enterTriggerOneshotSound, this.mixer, 1f, this.enterTriggerMinDistance, this.enterTriggerMaxDistance, true, 0.0f);
    if (this.state != HerbRootsStateEnum.Sleeping)
      return;
    this.herbRootsComponent?.FireOnTriggerEnterEvent();
  }

  private void OnPlayerExit()
  {
    this.activationStayTimeLeft = this.activationStayTime;
    if (this.state != HerbRootsStateEnum.Sleeping)
      return;
    this.herbRootsComponent?.FireOnTriggerLeaveEvent();
  }

  private void Update()
  {
    this.УбериМЕняОТсюдава_МЕнТутБытьНидолжно();
    if (InstanceByRequest<EngineApplication>.Instance.IsPaused || this.herbRootsComponent == null)
      return;
    switch (this.state)
    {
      case HerbRootsStateEnum.Sleeping:
        if (this.trigger.IsPlayerInside)
        {
          this.activationStayTimeLeft -= Time.deltaTime;
          if ((double) this.activationStayTimeLeft < 0.0)
            this.SetState(HerbRootsStateEnum.MovingFromEarth);
          this.attractLoopSoundVolume = Mathf.MoveTowards(this.attractLoopSoundVolume, 0.0f, Time.deltaTime / 2f);
          this.attractAudiosource.volume = this.attractLoopSoundVolume;
        }
        else
        {
          this.activationStayTimeLeft = this.activationStayTime;
          this.attractLoopSoundVolume = Mathf.MoveTowards(this.attractLoopSoundVolume, 1f, Time.deltaTime / 2f);
          this.attractAudiosource.volume = this.attractLoopSoundVolume;
        }
        this.enterTriggerLoopSoundVolume = Mathf.MoveTowards(this.enterTriggerLoopSoundVolume, Mathf.Sqrt(Mathf.Clamp01((float) (1.0 - (double) this.activationStayTimeLeft / (double) this.activationStayTime))), Time.deltaTime / 2f);
        this.enterTriggerAudiosource.volume = this.enterTriggerLoopSoundVolume;
        if ((double) this.enterTriggerLoopSoundVolume > 0.05000000074505806 && !this.enterTriggerAudiosource.isPlaying)
        {
          this.enterTriggerAudiosource.PlayAndCheck();
          break;
        }
        break;
      case HerbRootsStateEnum.MovingFromEarth:
        this.activationTimeLeft -= Time.deltaTime;
        if ((double) this.activationTimeLeft < 0.0)
        {
          this.SetState(HerbRootsStateEnum.Active);
          break;
        }
        float t = (float) (1.0 - (double) this.activationTimeLeft / (double) this.activationTime);
        if ((UnityEngine.Object) this.rootsGeometry != (UnityEngine.Object) null)
          this.rootsGeometry.transform.position = Vector3.Lerp(this.initialPosition + new Vector3(0.0f, -this.verticalOffset, 0.0f), this.initialPosition, t);
        break;
      case HerbRootsStateEnum.Active:
        this.attractLoopSoundVolume = Mathf.MoveTowards(this.attractLoopSoundVolume, 0.0f, Time.deltaTime / 2f);
        this.attractAudiosource.volume = this.attractLoopSoundVolume;
        this.enterTriggerLoopSoundVolume = Mathf.MoveTowards(this.enterTriggerLoopSoundVolume, 0.0f, Time.deltaTime / 2f);
        this.enterTriggerAudiosource.volume = this.enterTriggerLoopSoundVolume;
        break;
    }
    if (this.attractAudiosource.enabled)
    {
      if (this.attractAudiosource.isPlaying)
      {
        if ((double) this.attractLoopSoundVolume < 0.05000000074505806)
          this.attractAudiosource.Stop();
      }
      else if ((double) this.attractLoopSoundVolume > 0.05000000074505806)
        this.attractAudiosource.PlayAndCheck();
    }
    this.UpdateAudioEnable();
  }

  private void UpdateAudioEnable()
  {
    bool flag = (double) (this.transform.position - EngineApplication.PlayerPosition).sqrMagnitude < (double) (this.attractAudiosource.maxDistance * this.attractAudiosource.maxDistance);
    if (this.attractAudiosource.enabled == flag)
      return;
    this.attractAudiosource.enabled = flag;
  }

  private void УбериМЕняОТсюдава_МЕнТутБытьНидолжно()
  {
    if (!HerbRootsGroupDebug.IsGroupVisible)
      return;
    HerbRootsGroupDebug.DrawHeader();
    GizmoService service = ServiceLocator.GetService<GizmoService>();
    service.DrawCircle(this.transform.position, this.radius, Color.blue);
    string str = string.Format("{0}: - {1}, state = {2}", (object) typeof (HerbRoots).Name, (object) this.gameObject.name, (object) this.state);
    switch (this.state)
    {
      case HerbRootsStateEnum.Sleeping:
        service.DrawText(string.Format("{0}, timeLeft = {1}", (object) str, (object) this.activationStayTimeLeft), Color.white);
        break;
      case HerbRootsStateEnum.MovingFromEarth:
        service.DrawText(string.Format("{0}, timeLeft = {1}", (object) str, (object) this.activationTimeLeft), Color.white);
        break;
      case HerbRootsStateEnum.Active:
        service.DrawText(str ?? "", Color.white);
        break;
      default:
        service.DrawText(str ?? "", Color.red);
        break;
    }
    for (int index = 0; index < this.transform.childCount; ++index)
    {
      Transform child = this.transform.GetChild(index);
      if (!((UnityEngine.Object) child.GetComponentNonAlloc<HerbRootsSpawnPoint>() == (UnityEngine.Object) null))
      {
        service.DrawBox(child.position + new Vector3(0.025f, 0.0f, 0.025f), child.position + Vector3.up + new Vector3(-0.025f, 0.0f, -0.025f), Color.white);
        service.DrawCircle(child.position, 0.2f, Color.white);
        service.DrawBox(child.position + Vector3.up + new Vector3(0.1f, 0.0f, 0.1f), child.position + Vector3.up + new Vector3(-0.1f, 0.0f, -0.1f), Color.red);
      }
    }
  }

  private void OnDrawGizmos()
  {
    UnityEngine.Gizmos.color = Color.blue;
    UnityEngine.Gizmos.DrawWireSphere(this.transform.position, this.radius);
  }
}

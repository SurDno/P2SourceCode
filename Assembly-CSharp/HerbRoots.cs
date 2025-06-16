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

  public float VerticalOffset => verticalOffset;

  [Inspected]
  public IEntity Owner { get; private set; }

  private void Awake()
  {
    if (!((UnityEngine.Object) rootsGeometry != (UnityEngine.Object) null))
      return;
    initialPosition = this.transform.position;
  }

  void IEntityAttachable.Attach(IEntity owner)
  {
    Owner = owner;
    herbRootsComponent = (HerbRootsComponent) owner.GetComponent<IHerbRootsComponent>();
    if (herbRootsComponent == null)
    {
      Debug.LogError((object) (typeof (HerbRootsComponent).Name + " : " + Owner.GetInfo()), (UnityEngine.Object) this.gameObject);
    }
    else
    {
      switch (herbRootsComponent.State)
      {
        case HerbRootsComponentStateEnum.Sleeping:
          SetState(HerbRootsStateEnum.Sleeping);
          break;
        case HerbRootsComponentStateEnum.Active:
          SetState(HerbRootsStateEnum.Active);
          break;
        default:
          SetState(HerbRootsStateEnum.Unknown);
          break;
      }
    }
  }

  void IEntityAttachable.Detach()
  {
    Owner = null;
    herbRootsComponent = null;
  }

  private void SetState(HerbRootsStateEnum state)
  {
    switch (state)
    {
      case HerbRootsStateEnum.Sleeping:
        activationStayTimeLeft = activationStayTime;
        if ((UnityEngine.Object) rootsGeometry != (UnityEngine.Object) null)
        {
          rootsGeometry.transform.position = initialPosition + new Vector3(0.0f, -verticalOffset, 0.0f);
        }
        break;
      case HerbRootsStateEnum.MovingFromEarth:
        herbRootsComponent?.FireOnActivateStartEvent();
        activationTimeLeft = activationTime;
        SoundUtility.PlayAudioClip3D(this.transform, rootReleaseOneshotSound, mixer, 1f, enterTriggerMinDistance, enterTriggerMaxDistance, true, 0.0f);
        break;
      case HerbRootsStateEnum.Active:
        if ((UnityEngine.Object) rootsGeometry != (UnityEngine.Object) null)
          this.transform.position = initialPosition;
        herbRootsComponent.SetState(HerbRootsComponentStateEnum.Active);
        break;
    }
    this.state = state;
  }

  private void OnEnable()
  {
    GameObject gameObject = new GameObject("[Trigger] Player");
    gameObject.transform.SetParent(this.transform, false);
    trigger = gameObject.AddComponent<HerbRootsTrigger>();
    trigger.Radius = radius;
    trigger.PlayerEnterEvent += OnPlayerEnter;
    trigger.PlayerExitEvent += OnPlayerExit;
    attractAudiosource = this.gameObject.AddComponent<AudioSource>();
    attractAudiosource.clip = attractLoopSound;
    attractAudiosource.outputAudioMixerGroup = mixer;
    attractAudiosource.minDistance = attractMinDistance;
    attractAudiosource.maxDistance = attractMaxDistance;
    attractAudiosource.loop = true;
    attractAudiosource.rolloffMode = AudioRolloffMode.Linear;
    attractAudiosource.spatialBlend = 1f;
    attractLoopSoundVolume = 0.0f;
    attractAudiosource.Stop();
    enterTriggerAudiosource = this.gameObject.AddComponent<AudioSource>();
    enterTriggerAudiosource.clip = enterTriggerLoopSound;
    enterTriggerAudiosource.outputAudioMixerGroup = mixer;
    enterTriggerAudiosource.minDistance = enterTriggerMinDistance;
    enterTriggerAudiosource.maxDistance = enterTriggerMaxDistance;
    enterTriggerAudiosource.loop = true;
    enterTriggerAudiosource.spatialBlend = 0.5f;
    enterTriggerLoopSoundVolume = 0.0f;
    enterTriggerAudiosource.Stop();
  }

  private void OnDisable()
  {
    trigger.PlayerEnterEvent -= OnPlayerEnter;
    trigger.PlayerExitEvent -= OnPlayerExit;
    UnityEngine.Object.Destroy((UnityEngine.Object) trigger.gameObject);
    UnityEngine.Object.Destroy((UnityEngine.Object) attractAudiosource);
    UnityEngine.Object.Destroy((UnityEngine.Object) enterTriggerAudiosource);
  }

  public void PlayGiveBloodSound()
  {
    SoundUtility.PlayAudioClip2D(giveBloodSound, mixer, 1f, 0.0f);
  }

  private void OnPlayerEnter()
  {
    SoundUtility.PlayAudioClip3D(this.transform, enterTriggerOneshotSound, mixer, 1f, enterTriggerMinDistance, enterTriggerMaxDistance, true, 0.0f);
    if (state != HerbRootsStateEnum.Sleeping)
      return;
    herbRootsComponent?.FireOnTriggerEnterEvent();
  }

  private void OnPlayerExit()
  {
    activationStayTimeLeft = activationStayTime;
    if (state != HerbRootsStateEnum.Sleeping)
      return;
    herbRootsComponent?.FireOnTriggerLeaveEvent();
  }

  private void Update()
  {
    УбериМЕняОТсюдава_МЕнТутБытьНидолжно();
    if (InstanceByRequest<EngineApplication>.Instance.IsPaused || herbRootsComponent == null)
      return;
    switch (state)
    {
      case HerbRootsStateEnum.Sleeping:
        if (trigger.IsPlayerInside)
        {
          activationStayTimeLeft -= Time.deltaTime;
          if (activationStayTimeLeft < 0.0)
            SetState(HerbRootsStateEnum.MovingFromEarth);
          attractLoopSoundVolume = Mathf.MoveTowards(attractLoopSoundVolume, 0.0f, Time.deltaTime / 2f);
          attractAudiosource.volume = attractLoopSoundVolume;
        }
        else
        {
          activationStayTimeLeft = activationStayTime;
          attractLoopSoundVolume = Mathf.MoveTowards(attractLoopSoundVolume, 1f, Time.deltaTime / 2f);
          attractAudiosource.volume = attractLoopSoundVolume;
        }
        enterTriggerLoopSoundVolume = Mathf.MoveTowards(enterTriggerLoopSoundVolume, Mathf.Sqrt(Mathf.Clamp01((float) (1.0 - activationStayTimeLeft / (double) activationStayTime))), Time.deltaTime / 2f);
        enterTriggerAudiosource.volume = enterTriggerLoopSoundVolume;
        if (enterTriggerLoopSoundVolume > 0.05000000074505806 && !enterTriggerAudiosource.isPlaying)
        {
          enterTriggerAudiosource.PlayAndCheck();
        }
        break;
      case HerbRootsStateEnum.MovingFromEarth:
        activationTimeLeft -= Time.deltaTime;
        if (activationTimeLeft < 0.0)
        {
          SetState(HerbRootsStateEnum.Active);
          break;
        }
        float t = (float) (1.0 - activationTimeLeft / (double) activationTime);
        if ((UnityEngine.Object) rootsGeometry != (UnityEngine.Object) null)
          rootsGeometry.transform.position = Vector3.Lerp(initialPosition + new Vector3(0.0f, -verticalOffset, 0.0f), initialPosition, t);
        break;
      case HerbRootsStateEnum.Active:
        attractLoopSoundVolume = Mathf.MoveTowards(attractLoopSoundVolume, 0.0f, Time.deltaTime / 2f);
        attractAudiosource.volume = attractLoopSoundVolume;
        enterTriggerLoopSoundVolume = Mathf.MoveTowards(enterTriggerLoopSoundVolume, 0.0f, Time.deltaTime / 2f);
        enterTriggerAudiosource.volume = enterTriggerLoopSoundVolume;
        break;
    }
    if (attractAudiosource.enabled)
    {
      if (attractAudiosource.isPlaying)
      {
        if (attractLoopSoundVolume < 0.05000000074505806)
          attractAudiosource.Stop();
      }
      else if (attractLoopSoundVolume > 0.05000000074505806)
        attractAudiosource.PlayAndCheck();
    }
    UpdateAudioEnable();
  }

  private void UpdateAudioEnable()
  {
    bool flag = (double) (this.transform.position - EngineApplication.PlayerPosition).sqrMagnitude < (double) (attractAudiosource.maxDistance * attractAudiosource.maxDistance);
    if (attractAudiosource.enabled == flag)
      return;
    attractAudiosource.enabled = flag;
  }

  private void УбериМЕняОТсюдава_МЕнТутБытьНидолжно()
  {
    if (!HerbRootsGroupDebug.IsGroupVisible)
      return;
    HerbRootsGroupDebug.DrawHeader();
    GizmoService service = ServiceLocator.GetService<GizmoService>();
    service.DrawCircle(this.transform.position, radius, Color.blue);
    string str = string.Format("{0}: - {1}, state = {2}", typeof (HerbRoots).Name, (object) this.gameObject.name, state);
    switch (state)
    {
      case HerbRootsStateEnum.Sleeping:
        service.DrawText(string.Format("{0}, timeLeft = {1}", str, activationStayTimeLeft), Color.white);
        break;
      case HerbRootsStateEnum.MovingFromEarth:
        service.DrawText(string.Format("{0}, timeLeft = {1}", str, activationTimeLeft), Color.white);
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
    UnityEngine.Gizmos.DrawWireSphere(this.transform.position, radius);
  }
}

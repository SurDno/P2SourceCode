using Engine.Behaviours.Components;
using Engine.Behaviours.Unity.Mecanim;
using Engine.Common;
using Engine.Common.Services;
using Engine.Impl.Tasks;
using Engine.Source.Commons;
using Engine.Source.Components;
using Engine.Source.Services;
using Inspectors;
using UnityEngine;
using UnityEngine.AI;

public class NpcStateDialogNpc : INpcState
{
  private NpcState npcState;
  private Pivot pivot;
  private EngineBehavior behavior;
  private POISetup poiSetup;
  private Animator animator;
  private Rigidbody rigidbody;
  private NavMeshAgent agent;
  private AnimatorState45 animatorState;
  private POIAnimationEnum animation;
  private int animationIndex;
  private int animationsCount;
  private bool inited;
  private bool failed;
  private bool infinite;
  private float timeLeft;
  private bool speaking;
  private LipSyncObject lipsyncObject;
  private LipSyncComponent lipsyncComponent;
  private Transform lipsyncTransform;
  private float timeToNextFrase;
  private float timeToNextRandomAnimationSet = 0.0f;
  private float timeToNextRandomAnimationSetMax = 2f;

  [Inspected]
  public NpcStateStatusEnum Status
  {
    get
    {
      return !this.infinite && (double) this.timeLeft <= 0.0 ? NpcStateStatusEnum.Success : NpcStateStatusEnum.Running;
    }
  }

  public GameObject GameObject { get; private set; }

  private bool TryInit()
  {
    if (this.inited)
      return true;
    this.behavior = this.pivot.GetBehavior();
    this.poiSetup = this.GameObject.GetComponent<POISetup>();
    this.agent = this.pivot.GetAgent();
    this.rigidbody = this.pivot.GetRigidbody();
    this.animator = this.pivot.GetAnimator();
    if ((Object) this.animator == (Object) null)
    {
      Debug.LogError((object) ("Null animator " + this.GameObject.name), (Object) this.GameObject);
      Debug.LogError((object) ("Null animator " + this.GameObject.GetFullName()));
      this.failed = true;
      return false;
    }
    this.animatorState = AnimatorState45.GetAnimatorState(this.animator);
    this.failed = false;
    this.inited = true;
    return true;
  }

  public NpcStateDialogNpc(NpcState npcState, Pivot pivot)
  {
    this.GameObject = npcState.gameObject;
    this.pivot = pivot;
    this.npcState = npcState;
  }

  public void Activate(GameObject targetCharacter, float time, bool speaking)
  {
    if (!this.TryInit())
      return;
    this.infinite = (double) time == 0.0;
    this.timeLeft = time;
    this.speaking = speaking;
    IEntity setupPoint = this.npcState.Owner.GetComponent<NavigationComponent>().SetupPoint;
    POIAnimationSetupBase animationSetup = this.poiSetup.GetAnimationSetup(POIAnimationEnum.S_Dialog);
    this.animationIndex = Random.Range(0, animationSetup.Elements.Count);
    if (animationSetup.Elements.Count > this.animationIndex && animationSetup.Elements[this.animationIndex] is POIAnimationSetupElementSlow)
      this.animationsCount = (animationSetup.Elements[this.animationIndex] as POIAnimationSetupElementSlow).MiddleAnimationClips.Count;
    this.animatorState.ControlMovableState = AnimatorState45.MovableState45.POI;
    this.animatorState.ControlPOIAnimationIndex = this.animationIndex;
    this.animatorState.ControlPOIMiddleAnimationsCount = this.animationsCount;
    this.animatorState.ControlPOIStartFromMiddle = false;
    this.animatorState.ControlPOIState = MecanimKinds.MovablePOIStateKind.S_Dialog;
    this.animatorState.MovableStop = false;
    if (!speaking)
      return;
    ServiceLocator.GetService<POIService>().StartDialog(this.GameObject, targetCharacter);
  }

  public void OnAnimatorMove()
  {
    if (this.failed || InstanceByRequest<EngineApplication>.Instance.IsPaused)
      return;
    if ((Object) this.agent != (Object) null)
      this.agent.nextPosition = this.animator.rootPosition;
    this.GameObject.transform.position = this.animator.rootPosition;
    this.GameObject.transform.rotation = this.animator.rootRotation;
  }

  public void Shutdown()
  {
    if (this.failed)
      return;
    this.animatorState.MovableStop = true;
    ServiceLocator.GetService<POIService>().CharacterCanceledDialogActivity(this.GameObject);
  }

  public void OnAnimatorEventEvent(string obj)
  {
    if (!this.failed)
      ;
  }

  public void Update()
  {
    if (this.failed)
      return;
    if (!this.infinite)
      this.timeLeft -= Time.deltaTime;
    if (this.animatorState != null && !this.animatorState.IsPOI)
      this.animatorState.ControlMovableState = AnimatorState45.MovableState45.POI;
    this.timeToNextRandomAnimationSet -= Time.deltaTime;
    if ((double) this.timeToNextRandomAnimationSet > 0.0)
      return;
    this.timeToNextRandomAnimationSet = this.timeToNextRandomAnimationSetMax;
    this.SetRandomNextAnimation();
  }

  private void SetRandomNextAnimation()
  {
    this.animator.SetInteger("Movable.POI.AnimationIndex2", Random.Range(0, this.animator.GetInteger("Movable.POI.MiddleAnimationsCount")));
  }

  public void OnLodStateChanged(bool enabled)
  {
  }
}

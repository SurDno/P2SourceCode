using Engine.Behaviours.Components;
using Engine.Behaviours.Unity.Mecanim;
using Engine.Common;
using Engine.Common.Services;
using Engine.Impl.Tasks;
using Engine.Source.Commons;
using Engine.Source.Components;
using Engine.Source.Services;
using Inspectors;

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
  private float timeToNextRandomAnimationSet;
  private float timeToNextRandomAnimationSetMax = 2f;

  [Inspected]
  public NpcStateStatusEnum Status
  {
    get
    {
      return !infinite && timeLeft <= 0.0 ? NpcStateStatusEnum.Success : NpcStateStatusEnum.Running;
    }
  }

  public GameObject GameObject { get; private set; }

  private bool TryInit()
  {
    if (inited)
      return true;
    behavior = pivot.GetBehavior();
    poiSetup = GameObject.GetComponent<POISetup>();
    agent = pivot.GetAgent();
    rigidbody = pivot.GetRigidbody();
    animator = pivot.GetAnimator();
    if ((Object) animator == (Object) null)
    {
      Debug.LogError((object) ("Null animator " + GameObject.name), (Object) GameObject);
      Debug.LogError((object) ("Null animator " + GameObject.GetFullName()));
      failed = true;
      return false;
    }
    animatorState = AnimatorState45.GetAnimatorState(animator);
    failed = false;
    inited = true;
    return true;
  }

  public NpcStateDialogNpc(NpcState npcState, Pivot pivot)
  {
    GameObject = npcState.gameObject;
    this.pivot = pivot;
    this.npcState = npcState;
  }

  public void Activate(GameObject targetCharacter, float time, bool speaking)
  {
    if (!TryInit())
      return;
    infinite = time == 0.0;
    timeLeft = time;
    this.speaking = speaking;
    IEntity setupPoint = npcState.Owner.GetComponent<NavigationComponent>().SetupPoint;
    POIAnimationSetupBase animationSetup = poiSetup.GetAnimationSetup(POIAnimationEnum.S_Dialog);
    animationIndex = Random.Range(0, animationSetup.Elements.Count);
    if (animationSetup.Elements.Count > animationIndex && animationSetup.Elements[animationIndex] is POIAnimationSetupElementSlow)
      animationsCount = (animationSetup.Elements[animationIndex] as POIAnimationSetupElementSlow).MiddleAnimationClips.Count;
    animatorState.ControlMovableState = AnimatorState45.MovableState45.POI;
    animatorState.ControlPOIAnimationIndex = animationIndex;
    animatorState.ControlPOIMiddleAnimationsCount = animationsCount;
    animatorState.ControlPOIStartFromMiddle = false;
    animatorState.ControlPOIState = MecanimKinds.MovablePOIStateKind.S_Dialog;
    animatorState.MovableStop = false;
    if (!speaking)
      return;
    ServiceLocator.GetService<POIService>().StartDialog(GameObject, targetCharacter);
  }

  public void OnAnimatorMove()
  {
    if (failed || InstanceByRequest<EngineApplication>.Instance.IsPaused)
      return;
    if ((Object) agent != (Object) null)
      agent.nextPosition = animator.rootPosition;
    GameObject.transform.position = animator.rootPosition;
    GameObject.transform.rotation = animator.rootRotation;
  }

  public void Shutdown()
  {
    if (failed)
      return;
    animatorState.MovableStop = true;
    ServiceLocator.GetService<POIService>().CharacterCanceledDialogActivity(GameObject);
  }

  public void OnAnimatorEventEvent(string obj)
  {
    if (!failed)
      ;
  }

  public void Update()
  {
    if (failed)
      return;
    if (!infinite)
      timeLeft -= Time.deltaTime;
    if (animatorState != null && !animatorState.IsPOI)
      animatorState.ControlMovableState = AnimatorState45.MovableState45.POI;
    timeToNextRandomAnimationSet -= Time.deltaTime;
    if (timeToNextRandomAnimationSet > 0.0)
      return;
    timeToNextRandomAnimationSet = timeToNextRandomAnimationSetMax;
    SetRandomNextAnimation();
  }

  private void SetRandomNextAnimation()
  {
    animator.SetInteger("Movable.POI.AnimationIndex2", Random.Range(0, animator.GetInteger("Movable.POI.MiddleAnimationsCount")));
  }

  public void OnLodStateChanged(bool enabled)
  {
  }
}

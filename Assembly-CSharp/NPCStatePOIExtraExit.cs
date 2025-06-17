using Engine.Behaviours.Components;
using Engine.Behaviours.Unity.Mecanim;
using UnityEngine;

public class NPCStatePOIExtraExit(NpcState npcState, Pivot pivot) : INpcState 
  {
  private Animator animator;
  private NpcState npcState = npcState;
  private Rigidbody rigidbody;
  private AnimatorState45 animatorState;
  private bool initiallyKinematic;
  private bool failed;
  private bool inited;
  private bool complete;
  private float exitTime;

  public GameObject GameObject { get; private set; } = npcState.gameObject;

  private bool TryInit()
  {
    if (inited)
      return true;
    animator = pivot.GetAnimator();
    if (animator == null)
    {
      Debug.LogError("Null animator " + GameObject.name, GameObject);
      Debug.LogError("Null animator " + GameObject.GetFullName());
      failed = true;
      return false;
    }
    animatorState = AnimatorState45.GetAnimatorState(animator);
    rigidbody = pivot.GetRigidbody();
    failed = false;
    inited = true;
    return true;
  }

  public void Activate(INpcState previousState, float time)
  {
    if (!TryInit())
      return;
    if (!(previousState is INpcStateNeedSyncBack))
    {
      complete = true;
    }
    else
    {
      exitTime = time;
      bool flag = pivot.HasFastPOIExit((previousState as INpcStateNeedSyncBack).GetPoiType());
      animatorState.ResetAllTriggers();
      animatorState.SetTrigger(flag ? "Triggers/POIExtraExit" : "Triggers/POIExtraExitNoAnimation");
      complete = false;
      if (!(bool) (Object) rigidbody)
        return;
      initiallyKinematic = rigidbody.isKinematic;
      rigidbody.isKinematic = true;
    }
  }

  public NpcStateStatusEnum Status => complete ? NpcStateStatusEnum.Success : NpcStateStatusEnum.Running;

  public void Shutdown()
  {
    if (!(bool) (Object) rigidbody)
      return;
    rigidbody.isKinematic = initiallyKinematic;
  }

  public void OnAnimatorEventEvent(string obj)
  {
    if (!failed)
      ;
  }

  public void Update()
  {
  }

  public void OnAnimatorMove()
  {
    if (failed || complete)
      return;
    float num = animator.updateMode == AnimatorUpdateMode.AnimatePhysics ? Time.fixedDeltaTime : Time.deltaTime;
    if (!animatorState.IsPOI && !animatorState.IsPOIExit)
      animatorState.ControlMovableState = AnimatorState45.MovableState45.Idle;
    exitTime -= num;
    if (exitTime <= 0.0 && !animatorState.IsPOI && !animatorState.IsPOIExit)
    {
      complete = true;
    }
    else
    {
      GameObject.transform.position += animator.deltaPosition;
      GameObject.transform.rotation *= animator.deltaRotation;
    }
  }

  public void OnLodStateChanged(bool enabled)
  {
  }
}

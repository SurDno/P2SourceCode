using System;
using UnityEngine;
using UnityEngine.Events;

namespace RootMotion.Dynamics
{
  public abstract class BehaviourBase : MonoBehaviour
  {
    [HideInInspector]
    public PuppetMaster puppetMaster;
    public BehaviourDelegate OnPreActivate;
    public BehaviourDelegate OnPreInitiate;
    public BehaviourDelegate OnPreFixedUpdate;
    public BehaviourDelegate OnPreUpdate;
    public BehaviourDelegate OnPreLateUpdate;
    public BehaviourDelegate OnPreDeactivate;
    public BehaviourDelegate OnPreFixTransforms;
    public BehaviourDelegate OnPreRead;
    public BehaviourDelegate OnPreWrite;
    public HitDelegate OnPreMuscleHit;
    public CollisionDelegate OnPreMuscleCollision;
    public CollisionDelegate OnPreMuscleCollisionExit;
    public BehaviourDelegate OnHierarchyChanged;
    public BehaviourDelegate OnPostActivate;
    public BehaviourDelegate OnPostInitiate;
    public BehaviourDelegate OnPostFixedUpdate;
    public BehaviourDelegate OnPostUpdate;
    public BehaviourDelegate OnPostLateUpdate;
    public BehaviourDelegate OnPostDeactivate;
    public BehaviourDelegate OnPostDrawGizmos;
    public BehaviourDelegate OnPostFixTransforms;
    public BehaviourDelegate OnPostRead;
    public BehaviourDelegate OnPostWrite;
    public HitDelegate OnPostMuscleHit;
    public CollisionDelegate OnPostMuscleCollision;
    public CollisionDelegate OnPostMuscleCollisionExit;
    [HideInInspector]
    public bool deactivated;
    private bool initiated;

    public abstract void OnReactivate();

    public virtual void Resurrect()
    {
    }

    public virtual void Freeze()
    {
    }

    public virtual void Unfreeze()
    {
    }

    public virtual void KillStart()
    {
    }

    public virtual void KillEnd()
    {
    }

    public virtual void OnTeleport(
      Quaternion deltaRotation,
      Vector3 deltaPosition,
      Vector3 pivot,
      bool moveToTarget)
    {
    }

    public virtual void OnMuscleAdded(Muscle m)
    {
      if (OnHierarchyChanged == null)
        return;
      OnHierarchyChanged();
    }

    public virtual void OnMuscleRemoved(Muscle m)
    {
      if (OnHierarchyChanged == null)
        return;
      OnHierarchyChanged();
    }

    protected virtual void OnActivate()
    {
    }

    protected virtual void OnDeactivate()
    {
    }

    protected virtual void OnInitiate()
    {
    }

    protected virtual void OnFixedUpdate()
    {
    }

    protected virtual void OnUpdate()
    {
    }

    protected virtual void OnLateUpdate()
    {
    }

    protected virtual void OnDrawGizmosBehaviour()
    {
    }

    protected virtual void OnFixTransformsBehaviour()
    {
    }

    protected virtual void OnReadBehaviour()
    {
    }

    protected virtual void OnWriteBehaviour()
    {
    }

    protected virtual void OnMuscleHitBehaviour(MuscleHit hit)
    {
    }

    protected virtual void OnMuscleCollisionBehaviour(MuscleCollision collision)
    {
    }

    protected virtual void OnMuscleCollisionExitBehaviour(MuscleCollision collision)
    {
    }

    public bool forceActive { get; protected set; }

    public void Initiate()
    {
      initiated = true;
      if (OnPreInitiate != null)
        OnPreInitiate();
      OnInitiate();
      if (OnPostInitiate == null)
        return;
      OnPostInitiate();
    }

    public void OnFixTransforms()
    {
      if (!initiated || !enabled)
        return;
      if (OnPreFixTransforms != null)
        OnPreFixTransforms();
      OnFixTransformsBehaviour();
      if (OnPostFixTransforms == null)
        return;
      OnPostFixTransforms();
    }

    public void OnRead()
    {
      if (!initiated || !enabled)
        return;
      if (OnPreRead != null)
        OnPreRead();
      OnReadBehaviour();
      if (OnPostRead == null)
        return;
      OnPostRead();
    }

    public void OnWrite()
    {
      if (!initiated || !enabled)
        return;
      if (OnPreWrite != null)
        OnPreWrite();
      OnWriteBehaviour();
      if (OnPostWrite == null)
        return;
      OnPostWrite();
    }

    public void OnMuscleHit(MuscleHit hit)
    {
      if (!initiated)
        return;
      if (OnPreMuscleHit != null)
        OnPreMuscleHit(hit);
      OnMuscleHitBehaviour(hit);
      if (OnPostMuscleHit == null)
        return;
      OnPostMuscleHit(hit);
    }

    public void OnMuscleCollision(MuscleCollision collision)
    {
      if (!initiated)
        return;
      if (OnPreMuscleCollision != null)
        OnPreMuscleCollision(collision);
      OnMuscleCollisionBehaviour(collision);
      if (OnPostMuscleCollision == null)
        return;
      OnPostMuscleCollision(collision);
    }

    public void OnMuscleCollisionExit(MuscleCollision collision)
    {
      if (!initiated)
        return;
      if (OnPreMuscleCollisionExit != null)
        OnPreMuscleCollisionExit(collision);
      OnMuscleCollisionExitBehaviour(collision);
      if (OnPostMuscleCollisionExit == null)
        return;
      OnPostMuscleCollisionExit(collision);
    }

    private void OnEnable()
    {
      if (!initiated)
        return;
      Activate();
    }

    public void Activate()
    {
      foreach (BehaviourBase behaviour in puppetMaster.behaviours)
        behaviour.enabled = behaviour == this;
      if (OnPreActivate != null)
        OnPreActivate();
      OnActivate();
      if (OnPostActivate == null)
        return;
      OnPostActivate();
    }

    private void OnDisable()
    {
      if (!initiated)
        return;
      if (OnPreDeactivate != null)
        OnPreDeactivate();
      OnDeactivate();
      if (OnPostDeactivate == null)
        return;
      OnPostDeactivate();
    }

    private void FixedUpdate()
    {
      if (!initiated || puppetMaster.muscles.Length == 0)
        return;
      if (OnPreFixedUpdate != null && enabled)
        OnPreFixedUpdate();
      OnFixedUpdate();
      if (OnPostFixedUpdate == null || !enabled)
        return;
      OnPostFixedUpdate();
    }

    private void Update()
    {
      if (!initiated || puppetMaster.muscles.Length == 0)
        return;
      if (OnPreUpdate != null && enabled)
        OnPreUpdate();
      OnUpdate();
      if (OnPostUpdate == null || !enabled)
        return;
      OnPostUpdate();
    }

    private void LateUpdate()
    {
      if (!initiated || puppetMaster.muscles.Length == 0)
        return;
      if (OnPreLateUpdate != null && enabled)
        OnPreLateUpdate();
      OnLateUpdate();
      if (OnPostLateUpdate == null || !enabled)
        return;
      OnPostLateUpdate();
    }

    protected virtual void OnDrawGizmos()
    {
      if (!initiated)
        return;
      OnDrawGizmosBehaviour();
      if (OnPostDrawGizmos == null)
        return;
      OnPostDrawGizmos();
    }

    protected void RotateTargetToRootMuscle()
    {
      puppetMaster.targetRoot.rotation = Quaternion.LookRotation((puppetMaster.muscles[0].rigidbody.rotation * (Quaternion.Inverse(puppetMaster.muscles[0].target.rotation) * puppetMaster.targetRoot.forward)) with
      {
        y = 0.0f
      });
    }

    protected void TranslateTargetToRootMuscle(float maintainY)
    {
      puppetMaster.muscles[0].target.position = new Vector3(puppetMaster.muscles[0].transform.position.x, Mathf.Lerp(puppetMaster.muscles[0].transform.position.y, puppetMaster.muscles[0].target.position.y, maintainY), puppetMaster.muscles[0].transform.position.z);
    }

    protected void RemoveMusclesOfGroup(Muscle.Group group)
    {
      while (MusclesContainsGroup(group))
      {
        for (int index = 0; index < puppetMaster.muscles.Length; ++index)
        {
          if (puppetMaster.muscles[index].props.group == group)
          {
            puppetMaster.RemoveMuscleRecursive(puppetMaster.muscles[index].joint, true);
            break;
          }
        }
      }
    }

    protected virtual void GroundTarget(LayerMask layers)
    {
      if (!Physics.Raycast(new Ray(puppetMaster.targetRoot.position + puppetMaster.targetRoot.up, -puppetMaster.targetRoot.up), out RaycastHit hitInfo, 4f, layers))
        return;
      puppetMaster.targetRoot.position = hitInfo.point;
    }

    protected bool MusclesContainsGroup(Muscle.Group group)
    {
      foreach (Muscle muscle in puppetMaster.muscles)
      {
        if (muscle.props.group == group)
          return true;
      }
      return false;
    }

    public delegate void BehaviourDelegate();

    public delegate void HitDelegate(MuscleHit hit);

    public delegate void CollisionDelegate(MuscleCollision collision);

    [Serializable]
    public struct PuppetEvent
    {
      [Tooltip("Another Puppet Behaviour to switch to on this event. This must be the exact Type of the the Behaviour, careful with spelling.")]
      public string switchToBehaviour;
      [Tooltip("Animations to cross-fade to on this event. This is separate from the UnityEvent below because UnityEvents can't handle calls with more than one parameter such as Animator.CrossFade.")]
      public AnimatorEvent[] animations;
      [Tooltip("The UnityEvent to invoke on this event.")]
      public UnityEvent unityEvent;
      private const string empty = "";

      public bool switchBehaviour => switchToBehaviour != string.Empty && switchToBehaviour != "";

      public void Trigger(PuppetMaster puppetMaster, bool switchBehaviourEnabled = true)
      {
        unityEvent.Invoke();
        foreach (AnimatorEvent animation in animations)
          animation.Activate(puppetMaster.targetAnimator, puppetMaster.targetAnimation);
        if (!switchBehaviour)
          return;
        bool flag = false;
        foreach (BehaviourBase behaviour in puppetMaster.behaviours)
        {
          if (behaviour != null && behaviour.GetType().ToString() == "RootMotion.Dynamics." + switchToBehaviour)
          {
            flag = true;
            behaviour.enabled = true;
            break;
          }
        }
        if (!flag)
          Debug.LogWarning("No Puppet Behaviour of type '" + switchToBehaviour + "' was found. Can not switch to the behaviour, please check the spelling (also for empty spaces).");
      }
    }

    [Serializable]
    public class AnimatorEvent
    {
      public string animationState;
      public float crossfadeTime = 0.3f;
      public int layer;
      public bool resetNormalizedTime;
      private const string empty = "";

      public void Activate(Animator animator, Animation animation)
      {
        if (animator != null)
          Activate(animator);
        if (!(animation != null))
          return;
        Activate(animation);
      }

      private void Activate(Animator animator)
      {
        if (animationState == "")
          return;
        if (resetNormalizedTime)
        {
          if (crossfadeTime > 0.0)
            animator.CrossFadeInFixedTime(animationState, crossfadeTime, layer, 0.0f);
          else
            animator.Play(animationState, layer, 0.0f);
        }
        else if (crossfadeTime > 0.0)
          animator.CrossFadeInFixedTime(animationState, crossfadeTime, layer);
        else
          animator.Play(animationState, layer);
      }

      private void Activate(Animation animation)
      {
        if (animationState == "")
          return;
        if (resetNormalizedTime)
          animation[animationState].normalizedTime = 0.0f;
        animation[animationState].layer = layer;
        animation.CrossFade(animationState, crossfadeTime);
      }
    }
  }
}

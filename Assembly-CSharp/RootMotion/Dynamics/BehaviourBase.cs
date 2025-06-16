using System;
using UnityEngine;
using UnityEngine.Events;

namespace RootMotion.Dynamics
{
  public abstract class BehaviourBase : MonoBehaviour
  {
    [HideInInspector]
    public PuppetMaster puppetMaster;
    public BehaviourBase.BehaviourDelegate OnPreActivate;
    public BehaviourBase.BehaviourDelegate OnPreInitiate;
    public BehaviourBase.BehaviourDelegate OnPreFixedUpdate;
    public BehaviourBase.BehaviourDelegate OnPreUpdate;
    public BehaviourBase.BehaviourDelegate OnPreLateUpdate;
    public BehaviourBase.BehaviourDelegate OnPreDeactivate;
    public BehaviourBase.BehaviourDelegate OnPreFixTransforms;
    public BehaviourBase.BehaviourDelegate OnPreRead;
    public BehaviourBase.BehaviourDelegate OnPreWrite;
    public BehaviourBase.HitDelegate OnPreMuscleHit;
    public BehaviourBase.CollisionDelegate OnPreMuscleCollision;
    public BehaviourBase.CollisionDelegate OnPreMuscleCollisionExit;
    public BehaviourBase.BehaviourDelegate OnHierarchyChanged;
    public BehaviourBase.BehaviourDelegate OnPostActivate;
    public BehaviourBase.BehaviourDelegate OnPostInitiate;
    public BehaviourBase.BehaviourDelegate OnPostFixedUpdate;
    public BehaviourBase.BehaviourDelegate OnPostUpdate;
    public BehaviourBase.BehaviourDelegate OnPostLateUpdate;
    public BehaviourBase.BehaviourDelegate OnPostDeactivate;
    public BehaviourBase.BehaviourDelegate OnPostDrawGizmos;
    public BehaviourBase.BehaviourDelegate OnPostFixTransforms;
    public BehaviourBase.BehaviourDelegate OnPostRead;
    public BehaviourBase.BehaviourDelegate OnPostWrite;
    public BehaviourBase.HitDelegate OnPostMuscleHit;
    public BehaviourBase.CollisionDelegate OnPostMuscleCollision;
    public BehaviourBase.CollisionDelegate OnPostMuscleCollisionExit;
    [HideInInspector]
    public bool deactivated;
    private bool initiated = false;

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
      if (this.OnHierarchyChanged == null)
        return;
      this.OnHierarchyChanged();
    }

    public virtual void OnMuscleRemoved(Muscle m)
    {
      if (this.OnHierarchyChanged == null)
        return;
      this.OnHierarchyChanged();
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
      this.initiated = true;
      if (this.OnPreInitiate != null)
        this.OnPreInitiate();
      this.OnInitiate();
      if (this.OnPostInitiate == null)
        return;
      this.OnPostInitiate();
    }

    public void OnFixTransforms()
    {
      if (!this.initiated || !this.enabled)
        return;
      if (this.OnPreFixTransforms != null)
        this.OnPreFixTransforms();
      this.OnFixTransformsBehaviour();
      if (this.OnPostFixTransforms == null)
        return;
      this.OnPostFixTransforms();
    }

    public void OnRead()
    {
      if (!this.initiated || !this.enabled)
        return;
      if (this.OnPreRead != null)
        this.OnPreRead();
      this.OnReadBehaviour();
      if (this.OnPostRead == null)
        return;
      this.OnPostRead();
    }

    public void OnWrite()
    {
      if (!this.initiated || !this.enabled)
        return;
      if (this.OnPreWrite != null)
        this.OnPreWrite();
      this.OnWriteBehaviour();
      if (this.OnPostWrite == null)
        return;
      this.OnPostWrite();
    }

    public void OnMuscleHit(MuscleHit hit)
    {
      if (!this.initiated)
        return;
      if (this.OnPreMuscleHit != null)
        this.OnPreMuscleHit(hit);
      this.OnMuscleHitBehaviour(hit);
      if (this.OnPostMuscleHit == null)
        return;
      this.OnPostMuscleHit(hit);
    }

    public void OnMuscleCollision(MuscleCollision collision)
    {
      if (!this.initiated)
        return;
      if (this.OnPreMuscleCollision != null)
        this.OnPreMuscleCollision(collision);
      this.OnMuscleCollisionBehaviour(collision);
      if (this.OnPostMuscleCollision == null)
        return;
      this.OnPostMuscleCollision(collision);
    }

    public void OnMuscleCollisionExit(MuscleCollision collision)
    {
      if (!this.initiated)
        return;
      if (this.OnPreMuscleCollisionExit != null)
        this.OnPreMuscleCollisionExit(collision);
      this.OnMuscleCollisionExitBehaviour(collision);
      if (this.OnPostMuscleCollisionExit == null)
        return;
      this.OnPostMuscleCollisionExit(collision);
    }

    private void OnEnable()
    {
      if (!this.initiated)
        return;
      this.Activate();
    }

    public void Activate()
    {
      foreach (BehaviourBase behaviour in this.puppetMaster.behaviours)
        behaviour.enabled = (UnityEngine.Object) behaviour == (UnityEngine.Object) this;
      if (this.OnPreActivate != null)
        this.OnPreActivate();
      this.OnActivate();
      if (this.OnPostActivate == null)
        return;
      this.OnPostActivate();
    }

    private void OnDisable()
    {
      if (!this.initiated)
        return;
      if (this.OnPreDeactivate != null)
        this.OnPreDeactivate();
      this.OnDeactivate();
      if (this.OnPostDeactivate == null)
        return;
      this.OnPostDeactivate();
    }

    private void FixedUpdate()
    {
      if (!this.initiated || this.puppetMaster.muscles.Length == 0)
        return;
      if (this.OnPreFixedUpdate != null && this.enabled)
        this.OnPreFixedUpdate();
      this.OnFixedUpdate();
      if (this.OnPostFixedUpdate == null || !this.enabled)
        return;
      this.OnPostFixedUpdate();
    }

    private void Update()
    {
      if (!this.initiated || this.puppetMaster.muscles.Length == 0)
        return;
      if (this.OnPreUpdate != null && this.enabled)
        this.OnPreUpdate();
      this.OnUpdate();
      if (this.OnPostUpdate == null || !this.enabled)
        return;
      this.OnPostUpdate();
    }

    private void LateUpdate()
    {
      if (!this.initiated || this.puppetMaster.muscles.Length == 0)
        return;
      if (this.OnPreLateUpdate != null && this.enabled)
        this.OnPreLateUpdate();
      this.OnLateUpdate();
      if (this.OnPostLateUpdate == null || !this.enabled)
        return;
      this.OnPostLateUpdate();
    }

    protected virtual void OnDrawGizmos()
    {
      if (!this.initiated)
        return;
      this.OnDrawGizmosBehaviour();
      if (this.OnPostDrawGizmos == null)
        return;
      this.OnPostDrawGizmos();
    }

    protected void RotateTargetToRootMuscle()
    {
      this.puppetMaster.targetRoot.rotation = Quaternion.LookRotation((this.puppetMaster.muscles[0].rigidbody.rotation * (Quaternion.Inverse(this.puppetMaster.muscles[0].target.rotation) * this.puppetMaster.targetRoot.forward)) with
      {
        y = 0.0f
      });
    }

    protected void TranslateTargetToRootMuscle(float maintainY)
    {
      this.puppetMaster.muscles[0].target.position = new Vector3(this.puppetMaster.muscles[0].transform.position.x, Mathf.Lerp(this.puppetMaster.muscles[0].transform.position.y, this.puppetMaster.muscles[0].target.position.y, maintainY), this.puppetMaster.muscles[0].transform.position.z);
    }

    protected void RemoveMusclesOfGroup(Muscle.Group group)
    {
      while (this.MusclesContainsGroup(group))
      {
        for (int index = 0; index < this.puppetMaster.muscles.Length; ++index)
        {
          if (this.puppetMaster.muscles[index].props.group == group)
          {
            this.puppetMaster.RemoveMuscleRecursive(this.puppetMaster.muscles[index].joint, true);
            break;
          }
        }
      }
    }

    protected virtual void GroundTarget(LayerMask layers)
    {
      RaycastHit hitInfo;
      if (!Physics.Raycast(new Ray(this.puppetMaster.targetRoot.position + this.puppetMaster.targetRoot.up, -this.puppetMaster.targetRoot.up), out hitInfo, 4f, (int) layers))
        return;
      this.puppetMaster.targetRoot.position = hitInfo.point;
    }

    protected bool MusclesContainsGroup(Muscle.Group group)
    {
      foreach (Muscle muscle in this.puppetMaster.muscles)
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
      public BehaviourBase.AnimatorEvent[] animations;
      [Tooltip("The UnityEvent to invoke on this event.")]
      public UnityEvent unityEvent;
      private const string empty = "";

      public bool switchBehaviour
      {
        get => this.switchToBehaviour != string.Empty && this.switchToBehaviour != "";
      }

      public void Trigger(PuppetMaster puppetMaster, bool switchBehaviourEnabled = true)
      {
        this.unityEvent.Invoke();
        foreach (BehaviourBase.AnimatorEvent animation in this.animations)
          animation.Activate(puppetMaster.targetAnimator, puppetMaster.targetAnimation);
        if (!this.switchBehaviour)
          return;
        bool flag = false;
        foreach (BehaviourBase behaviour in puppetMaster.behaviours)
        {
          if ((UnityEngine.Object) behaviour != (UnityEngine.Object) null && ((object) behaviour).GetType().ToString() == "RootMotion.Dynamics." + this.switchToBehaviour)
          {
            flag = true;
            behaviour.enabled = true;
            break;
          }
        }
        if (!flag)
          Debug.LogWarning((object) ("No Puppet Behaviour of type '" + this.switchToBehaviour + "' was found. Can not switch to the behaviour, please check the spelling (also for empty spaces)."));
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
        if ((UnityEngine.Object) animator != (UnityEngine.Object) null)
          this.Activate(animator);
        if (!((UnityEngine.Object) animation != (UnityEngine.Object) null))
          return;
        this.Activate(animation);
      }

      private void Activate(Animator animator)
      {
        if (this.animationState == "")
          return;
        if (this.resetNormalizedTime)
        {
          if ((double) this.crossfadeTime > 0.0)
            animator.CrossFadeInFixedTime(this.animationState, this.crossfadeTime, this.layer, 0.0f);
          else
            animator.Play(this.animationState, this.layer, 0.0f);
        }
        else if ((double) this.crossfadeTime > 0.0)
          animator.CrossFadeInFixedTime(this.animationState, this.crossfadeTime, this.layer);
        else
          animator.Play(this.animationState, this.layer);
      }

      private void Activate(Animation animation)
      {
        if (this.animationState == "")
          return;
        if (this.resetNormalizedTime)
          animation[this.animationState].normalizedTime = 0.0f;
        animation[this.animationState].layer = this.layer;
        animation.CrossFade(this.animationState, this.crossfadeTime);
      }
    }
  }
}

using System;
using System.Collections;
using UnityEngine;

namespace RootMotion.FinalIK
{
  [RequireComponent(typeof (Animator))]
  public class RagdollUtility : MonoBehaviour
  {
    [Tooltip("If you have multiple IK components, then this should be the one that solves last each frame.")]
    public IK ik;
    [Tooltip("How long does it take to blend from ragdoll to animation?")]
    public float ragdollToAnimationTime = 0.2f;
    [Tooltip("If true, IK can be used on top of physical ragdoll simulation.")]
    public bool applyIkOnRagdoll;
    [Tooltip("How much velocity transfer from animation to ragdoll?")]
    public float applyVelocity = 1f;
    [Tooltip("How much angular velocity to transfer from animation to ragdoll?")]
    public float applyAngularVelocity = 1f;
    private Animator animator;
    private RagdollUtility.Rigidbone[] rigidbones = new RagdollUtility.Rigidbone[0];
    private RagdollUtility.Child[] children = new RagdollUtility.Child[0];
    private bool enableRagdollFlag;
    private AnimatorUpdateMode animatorUpdateMode;
    private IK[] allIKComponents = new IK[0];
    private bool[] fixTransforms = new bool[0];
    private float ragdollWeight;
    private float ragdollWeightV;
    private bool fixedFrame;
    private bool[] disabledIKComponents = new bool[0];

    public void EnableRagdoll()
    {
      if (this.isRagdoll)
        return;
      this.StopAllCoroutines();
      this.enableRagdollFlag = true;
    }

    public void DisableRagdoll()
    {
      if (!this.isRagdoll)
        return;
      this.StoreLocalState();
      this.StopAllCoroutines();
      this.StartCoroutine(this.DisableRagdollSmooth());
    }

    public void Start()
    {
      this.animator = this.GetComponent<Animator>();
      this.allIKComponents = this.GetComponentsInChildren<IK>();
      this.disabledIKComponents = new bool[this.allIKComponents.Length];
      this.fixTransforms = new bool[this.allIKComponents.Length];
      if ((UnityEngine.Object) this.ik != (UnityEngine.Object) null)
        this.ik.GetIKSolver().OnPostUpdate += new IKSolver.UpdateDelegate(this.AfterLastIK);
      Rigidbody[] componentsInChildren1 = this.GetComponentsInChildren<Rigidbody>();
      int num = (UnityEngine.Object) componentsInChildren1[0].gameObject == (UnityEngine.Object) this.gameObject ? 1 : 0;
      this.rigidbones = new RagdollUtility.Rigidbone[num == 0 ? componentsInChildren1.Length : componentsInChildren1.Length - 1];
      for (int index = 0; index < this.rigidbones.Length; ++index)
        this.rigidbones[index] = new RagdollUtility.Rigidbone(componentsInChildren1[index + num]);
      Transform[] componentsInChildren2 = this.GetComponentsInChildren<Transform>();
      this.children = new RagdollUtility.Child[componentsInChildren2.Length - 1];
      for (int index = 0; index < this.children.Length; ++index)
        this.children[index] = new RagdollUtility.Child(componentsInChildren2[index + 1]);
    }

    private IEnumerator DisableRagdollSmooth()
    {
      for (int i = 0; i < this.rigidbones.Length; ++i)
        this.rigidbones[i].r.isKinematic = true;
      for (int i = 0; i < this.allIKComponents.Length; ++i)
      {
        this.allIKComponents[i].fixTransforms = this.fixTransforms[i];
        if (this.disabledIKComponents[i])
          this.allIKComponents[i].enabled = true;
      }
      this.animator.updateMode = this.animatorUpdateMode;
      this.animator.enabled = true;
      while ((double) this.ragdollWeight > 0.0)
      {
        this.ragdollWeight = Mathf.SmoothDamp(this.ragdollWeight, 0.0f, ref this.ragdollWeightV, this.ragdollToAnimationTime);
        if ((double) this.ragdollWeight < 1.0 / 1000.0)
          this.ragdollWeight = 0.0f;
        yield return (object) null;
      }
      yield return (object) null;
    }

    private void Update()
    {
      if (!this.isRagdoll)
        return;
      if (!this.applyIkOnRagdoll)
      {
        bool flag = false;
        for (int index = 0; index < this.allIKComponents.Length; ++index)
        {
          if (this.allIKComponents[index].enabled)
          {
            flag = true;
            break;
          }
        }
        if (flag)
        {
          for (int index = 0; index < this.allIKComponents.Length; ++index)
            this.disabledIKComponents[index] = false;
        }
        for (int index = 0; index < this.allIKComponents.Length; ++index)
        {
          if (this.allIKComponents[index].enabled)
          {
            this.allIKComponents[index].enabled = false;
            this.disabledIKComponents[index] = true;
          }
        }
      }
      else
      {
        bool flag = false;
        for (int index = 0; index < this.allIKComponents.Length; ++index)
        {
          if (this.disabledIKComponents[index])
          {
            flag = true;
            break;
          }
        }
        if (flag)
        {
          for (int index = 0; index < this.allIKComponents.Length; ++index)
          {
            if (this.disabledIKComponents[index])
              this.allIKComponents[index].enabled = true;
          }
          for (int index = 0; index < this.allIKComponents.Length; ++index)
            this.disabledIKComponents[index] = false;
        }
      }
    }

    private void FixedUpdate()
    {
      if (this.isRagdoll && this.applyIkOnRagdoll)
        this.FixTransforms(1f);
      this.fixedFrame = true;
    }

    private void LateUpdate()
    {
      if (this.animator.updateMode != AnimatorUpdateMode.AnimatePhysics || this.animator.updateMode == AnimatorUpdateMode.AnimatePhysics && this.fixedFrame)
        this.AfterAnimation();
      this.fixedFrame = false;
      if (this.ikUsed)
        return;
      this.OnFinalPose();
    }

    private void AfterLastIK()
    {
      if (!this.ikUsed)
        return;
      this.OnFinalPose();
    }

    private void AfterAnimation()
    {
      if (this.isRagdoll)
        this.StoreLocalState();
      else
        this.FixTransforms(this.ragdollWeight);
    }

    private void OnFinalPose()
    {
      if (!this.isRagdoll)
        this.RecordVelocities();
      if (!this.enableRagdollFlag)
        return;
      this.RagdollEnabler();
    }

    private void RagdollEnabler()
    {
      this.StoreLocalState();
      for (int index = 0; index < this.allIKComponents.Length; ++index)
        this.disabledIKComponents[index] = false;
      if (!this.applyIkOnRagdoll)
      {
        for (int index = 0; index < this.allIKComponents.Length; ++index)
        {
          if (this.allIKComponents[index].enabled)
          {
            this.allIKComponents[index].enabled = false;
            this.disabledIKComponents[index] = true;
          }
        }
      }
      this.animatorUpdateMode = this.animator.updateMode;
      this.animator.updateMode = AnimatorUpdateMode.AnimatePhysics;
      this.animator.enabled = false;
      for (int index = 0; index < this.rigidbones.Length; ++index)
        this.rigidbones[index].WakeUp(this.applyVelocity, this.applyAngularVelocity);
      for (int index = 0; index < this.fixTransforms.Length; ++index)
      {
        this.fixTransforms[index] = this.allIKComponents[index].fixTransforms;
        this.allIKComponents[index].fixTransforms = false;
      }
      this.ragdollWeight = 1f;
      this.ragdollWeightV = 0.0f;
      this.enableRagdollFlag = false;
    }

    private bool isRagdoll => !this.rigidbones[0].r.isKinematic && !this.animator.enabled;

    private void RecordVelocities()
    {
      foreach (RagdollUtility.Rigidbone rigidbone in this.rigidbones)
        rigidbone.RecordVelocity();
    }

    private bool ikUsed
    {
      get
      {
        if ((UnityEngine.Object) this.ik == (UnityEngine.Object) null)
          return false;
        if (this.ik.enabled && (double) this.ik.GetIKSolver().IKPositionWeight > 0.0)
          return true;
        foreach (IK allIkComponent in this.allIKComponents)
        {
          if (allIkComponent.enabled && (double) allIkComponent.GetIKSolver().IKPositionWeight > 0.0)
            return true;
        }
        return false;
      }
    }

    private void StoreLocalState()
    {
      foreach (RagdollUtility.Child child in this.children)
        child.StoreLocalState();
    }

    private void FixTransforms(float weight)
    {
      foreach (RagdollUtility.Child child in this.children)
        child.FixTransform(weight);
    }

    private void OnDestroy()
    {
      if (!((UnityEngine.Object) this.ik != (UnityEngine.Object) null))
        return;
      this.ik.GetIKSolver().OnPostUpdate -= new IKSolver.UpdateDelegate(this.AfterLastIK);
    }

    public class Rigidbone
    {
      public Rigidbody r;
      public Transform t;
      public Collider collider;
      public Joint joint;
      public Rigidbody c;
      public bool updateAnchor;
      public Vector3 deltaPosition;
      public Quaternion deltaRotation;
      public float deltaTime;
      public Vector3 lastPosition;
      public Quaternion lastRotation;

      public Rigidbone(Rigidbody r)
      {
        this.r = r;
        this.t = r.transform;
        this.joint = this.t.GetComponent<Joint>();
        this.collider = this.t.GetComponent<Collider>();
        if ((UnityEngine.Object) this.joint != (UnityEngine.Object) null)
        {
          this.c = this.joint.connectedBody;
          this.updateAnchor = (UnityEngine.Object) this.c != (UnityEngine.Object) null;
        }
        this.lastPosition = this.t.position;
        this.lastRotation = this.t.rotation;
      }

      public void RecordVelocity()
      {
        this.deltaPosition = this.t.position - this.lastPosition;
        this.lastPosition = this.t.position;
        this.deltaRotation = QuaTools.FromToRotation(this.lastRotation, this.t.rotation);
        this.lastRotation = this.t.rotation;
        this.deltaTime = Time.deltaTime;
      }

      public void WakeUp(float velocityWeight, float angularVelocityWeight)
      {
        if (this.updateAnchor)
          this.joint.connectedAnchor = this.t.InverseTransformPoint(this.c.position);
        this.r.isKinematic = false;
        if ((double) velocityWeight != 0.0)
          this.r.velocity = this.deltaPosition / this.deltaTime * velocityWeight;
        if ((double) angularVelocityWeight != 0.0)
        {
          float angle = 0.0f;
          Vector3 axis = Vector3.zero;
          this.deltaRotation.ToAngleAxis(out angle, out axis);
          angle *= (float) Math.PI / 180f;
          float num = angle / this.deltaTime;
          this.r.angularVelocity = Vector3.ClampMagnitude(axis * (num * angularVelocityWeight), this.r.maxAngularVelocity);
        }
        this.r.WakeUp();
      }
    }

    public class Child
    {
      public Transform t;
      public Vector3 localPosition;
      public Quaternion localRotation;

      public Child(Transform transform)
      {
        this.t = transform;
        this.localPosition = this.t.localPosition;
        this.localRotation = this.t.localRotation;
      }

      public void FixTransform(float weight)
      {
        if ((double) weight <= 0.0)
          return;
        if ((double) weight >= 1.0)
        {
          this.t.localPosition = this.localPosition;
          this.t.localRotation = this.localRotation;
        }
        else
        {
          this.t.localPosition = Vector3.Lerp(this.t.localPosition, this.localPosition, weight);
          this.t.localRotation = Quaternion.Lerp(this.t.localRotation, this.localRotation, weight);
        }
      }

      public void StoreLocalState()
      {
        this.localPosition = this.t.localPosition;
        this.localRotation = this.t.localRotation;
      }
    }
  }
}

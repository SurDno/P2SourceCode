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
    private Rigidbone[] rigidbones = [];
    private Child[] children = [];
    private bool enableRagdollFlag;
    private AnimatorUpdateMode animatorUpdateMode;
    private IK[] allIKComponents = [];
    private bool[] fixTransforms = [];
    private float ragdollWeight;
    private float ragdollWeightV;
    private bool fixedFrame;
    private bool[] disabledIKComponents = [];

    public void EnableRagdoll()
    {
      if (isRagdoll)
        return;
      StopAllCoroutines();
      enableRagdollFlag = true;
    }

    public void DisableRagdoll()
    {
      if (!isRagdoll)
        return;
      StoreLocalState();
      StopAllCoroutines();
      StartCoroutine(DisableRagdollSmooth());
    }

    public void Start()
    {
      animator = GetComponent<Animator>();
      allIKComponents = GetComponentsInChildren<IK>();
      disabledIKComponents = new bool[allIKComponents.Length];
      fixTransforms = new bool[allIKComponents.Length];
      if (ik != null)
        ik.GetIKSolver().OnPostUpdate += AfterLastIK;
      Rigidbody[] componentsInChildren1 = GetComponentsInChildren<Rigidbody>();
      int num = componentsInChildren1[0].gameObject == gameObject ? 1 : 0;
      rigidbones = new Rigidbone[num == 0 ? componentsInChildren1.Length : componentsInChildren1.Length - 1];
      for (int index = 0; index < rigidbones.Length; ++index)
        rigidbones[index] = new Rigidbone(componentsInChildren1[index + num]);
      Transform[] componentsInChildren2 = GetComponentsInChildren<Transform>();
      children = new Child[componentsInChildren2.Length - 1];
      for (int index = 0; index < children.Length; ++index)
        children[index] = new Child(componentsInChildren2[index + 1]);
    }

    private IEnumerator DisableRagdollSmooth()
    {
      for (int i = 0; i < rigidbones.Length; ++i)
        rigidbones[i].r.isKinematic = true;
      for (int i = 0; i < allIKComponents.Length; ++i)
      {
        allIKComponents[i].fixTransforms = fixTransforms[i];
        if (disabledIKComponents[i])
          allIKComponents[i].enabled = true;
      }
      animator.updateMode = animatorUpdateMode;
      animator.enabled = true;
      while (ragdollWeight > 0.0)
      {
        ragdollWeight = Mathf.SmoothDamp(ragdollWeight, 0.0f, ref ragdollWeightV, ragdollToAnimationTime);
        if (ragdollWeight < 1.0 / 1000.0)
          ragdollWeight = 0.0f;
        yield return null;
      }
      yield return null;
    }

    private void Update()
    {
      if (!isRagdoll)
        return;
      if (!applyIkOnRagdoll)
      {
        bool flag = false;
        for (int index = 0; index < allIKComponents.Length; ++index)
        {
          if (allIKComponents[index].enabled)
          {
            flag = true;
            break;
          }
        }
        if (flag)
        {
          for (int index = 0; index < allIKComponents.Length; ++index)
            disabledIKComponents[index] = false;
        }
        for (int index = 0; index < allIKComponents.Length; ++index)
        {
          if (allIKComponents[index].enabled)
          {
            allIKComponents[index].enabled = false;
            disabledIKComponents[index] = true;
          }
        }
      }
      else
      {
        bool flag = false;
        for (int index = 0; index < allIKComponents.Length; ++index)
        {
          if (disabledIKComponents[index])
          {
            flag = true;
            break;
          }
        }
        if (flag)
        {
          for (int index = 0; index < allIKComponents.Length; ++index)
          {
            if (disabledIKComponents[index])
              allIKComponents[index].enabled = true;
          }
          for (int index = 0; index < allIKComponents.Length; ++index)
            disabledIKComponents[index] = false;
        }
      }
    }

    private void FixedUpdate()
    {
      if (isRagdoll && applyIkOnRagdoll)
        FixTransforms(1f);
      fixedFrame = true;
    }

    private void LateUpdate()
    {
      if (animator.updateMode != AnimatorUpdateMode.AnimatePhysics || animator.updateMode == AnimatorUpdateMode.AnimatePhysics && fixedFrame)
        AfterAnimation();
      fixedFrame = false;
      if (ikUsed)
        return;
      OnFinalPose();
    }

    private void AfterLastIK()
    {
      if (!ikUsed)
        return;
      OnFinalPose();
    }

    private void AfterAnimation()
    {
      if (isRagdoll)
        StoreLocalState();
      else
        FixTransforms(ragdollWeight);
    }

    private void OnFinalPose()
    {
      if (!isRagdoll)
        RecordVelocities();
      if (!enableRagdollFlag)
        return;
      RagdollEnabler();
    }

    private void RagdollEnabler()
    {
      StoreLocalState();
      for (int index = 0; index < allIKComponents.Length; ++index)
        disabledIKComponents[index] = false;
      if (!applyIkOnRagdoll)
      {
        for (int index = 0; index < allIKComponents.Length; ++index)
        {
          if (allIKComponents[index].enabled)
          {
            allIKComponents[index].enabled = false;
            disabledIKComponents[index] = true;
          }
        }
      }
      animatorUpdateMode = animator.updateMode;
      animator.updateMode = AnimatorUpdateMode.AnimatePhysics;
      animator.enabled = false;
      for (int index = 0; index < rigidbones.Length; ++index)
        rigidbones[index].WakeUp(applyVelocity, applyAngularVelocity);
      for (int index = 0; index < fixTransforms.Length; ++index)
      {
        fixTransforms[index] = allIKComponents[index].fixTransforms;
        allIKComponents[index].fixTransforms = false;
      }
      ragdollWeight = 1f;
      ragdollWeightV = 0.0f;
      enableRagdollFlag = false;
    }

    private bool isRagdoll => !rigidbones[0].r.isKinematic && !animator.enabled;

    private void RecordVelocities()
    {
      foreach (Rigidbone rigidbone in rigidbones)
        rigidbone.RecordVelocity();
    }

    private bool ikUsed
    {
      get
      {
        if (ik == null)
          return false;
        if (ik.enabled && ik.GetIKSolver().IKPositionWeight > 0.0)
          return true;
        foreach (IK allIkComponent in allIKComponents)
        {
          if (allIkComponent.enabled && allIkComponent.GetIKSolver().IKPositionWeight > 0.0)
            return true;
        }
        return false;
      }
    }

    private void StoreLocalState()
    {
      foreach (Child child in children)
        child.StoreLocalState();
    }

    private void FixTransforms(float weight)
    {
      foreach (Child child in children)
        child.FixTransform(weight);
    }

    private void OnDestroy()
    {
      if (!(ik != null))
        return;
      ik.GetIKSolver().OnPostUpdate -= AfterLastIK;
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
        t = r.transform;
        joint = t.GetComponent<Joint>();
        collider = t.GetComponent<Collider>();
        if (joint != null)
        {
          c = joint.connectedBody;
          updateAnchor = c != null;
        }
        lastPosition = t.position;
        lastRotation = t.rotation;
      }

      public void RecordVelocity()
      {
        deltaPosition = t.position - lastPosition;
        lastPosition = t.position;
        deltaRotation = QuaTools.FromToRotation(lastRotation, t.rotation);
        lastRotation = t.rotation;
        deltaTime = Time.deltaTime;
      }

      public void WakeUp(float velocityWeight, float angularVelocityWeight)
      {
        if (updateAnchor)
          joint.connectedAnchor = t.InverseTransformPoint(c.position);
        r.isKinematic = false;
        if (velocityWeight != 0.0)
          r.velocity = deltaPosition / deltaTime * velocityWeight;
        if (angularVelocityWeight != 0.0)
        {
          deltaRotation.ToAngleAxis(out float angle, out Vector3 axis);
          angle *= (float) Math.PI / 180f;
          float num = angle / deltaTime;
          r.angularVelocity = Vector3.ClampMagnitude(axis * (num * angularVelocityWeight), r.maxAngularVelocity);
        }
        r.WakeUp();
      }
    }

    public class Child
    {
      public Transform t;
      public Vector3 localPosition;
      public Quaternion localRotation;

      public Child(Transform transform)
      {
        t = transform;
        localPosition = t.localPosition;
        localRotation = t.localRotation;
      }

      public void FixTransform(float weight)
      {
        if (weight <= 0.0)
          return;
        if (weight >= 1.0)
        {
          t.localPosition = localPosition;
          t.localRotation = localRotation;
        }
        else
        {
          t.localPosition = Vector3.Lerp(t.localPosition, localPosition, weight);
          t.localRotation = Quaternion.Lerp(t.localRotation, localRotation, weight);
        }
      }

      public void StoreLocalState()
      {
        localPosition = t.localPosition;
        localRotation = t.localRotation;
      }
    }
  }
}

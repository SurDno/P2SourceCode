// Decompiled with JetBrains decompiler
// Type: RootMotion.FinalIK.Finger
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
namespace RootMotion.FinalIK
{
  [Serializable]
  public class Finger
  {
    [Tooltip("Master Weight for the finger.")]
    [Range(0.0f, 1f)]
    public float weight = 1f;
    [Tooltip("The weight of rotating the finger tip and bending the finger to the target.")]
    [Range(0.0f, 1f)]
    public float rotationWeight = 1f;
    [Tooltip("Rotational degrees of freedom. When set to 'One' the fingers will be able to be rotated only around a single axis. When 3, all 3 axes are free to rotate around.")]
    public Finger.DOF rotationDOF;
    [Tooltip("The first bone of the finger.")]
    public Transform bone1;
    [Tooltip("The second bone of the finger.")]
    public Transform bone2;
    [Tooltip("The (optional) third bone of the finger. This can be ignored for thumbs.")]
    public Transform bone3;
    [Tooltip("The fingertip object. If your character doesn't have tip bones, you can create an empty GameObject and parent it to the last bone in the finger. Place it to the tip of the finger.")]
    public Transform tip;
    [Tooltip("The IK target (optional, can use IKPosition and IKRotation directly).")]
    public Transform target;
    private IKSolverLimb solver;
    private Quaternion bone3RelativeToTarget;
    private Vector3 bone3DefaultLocalPosition;
    private Quaternion bone3DefaultLocalRotation;
    private Vector3 bone1Axis;
    private Vector3 tipAxis;

    public bool initiated { get; private set; }

    public Vector3 IKPosition
    {
      get => this.solver.IKPosition;
      set => this.solver.IKPosition = value;
    }

    public Quaternion IKRotation
    {
      get => this.solver.IKRotation;
      set => this.solver.IKRotation = value;
    }

    public bool IsValid(ref string errorMessage)
    {
      if (!((UnityEngine.Object) this.bone1 == (UnityEngine.Object) null) && !((UnityEngine.Object) this.bone2 == (UnityEngine.Object) null) && !((UnityEngine.Object) this.tip == (UnityEngine.Object) null))
        return true;
      errorMessage = "One of the bones in the Finger Rig is null, can not initiate solvers.";
      return false;
    }

    public void Initiate(Transform hand, int index)
    {
      this.initiated = false;
      string empty = string.Empty;
      if (!this.IsValid(ref empty))
      {
        Warning.Log(empty, hand);
      }
      else
      {
        this.solver = new IKSolverLimb();
        this.solver.IKPositionWeight = this.weight;
        this.solver.bendModifier = IKSolverLimb.BendModifier.Target;
        this.solver.bendModifierWeight = 1f;
        Vector3 vector3 = Vector3.Cross(this.bone2.position - this.bone1.position, this.tip.position - this.bone1.position);
        this.bone1Axis = Quaternion.Inverse(this.bone1.rotation) * vector3;
        this.tipAxis = Quaternion.Inverse(this.tip.rotation) * vector3;
        this.IKPosition = this.tip.position;
        this.IKRotation = this.tip.rotation;
        if ((UnityEngine.Object) this.bone3 != (UnityEngine.Object) null)
        {
          this.bone3RelativeToTarget = Quaternion.Inverse(this.IKRotation) * this.bone3.rotation;
          this.bone3DefaultLocalPosition = this.bone3.localPosition;
          this.bone3DefaultLocalRotation = this.bone3.localRotation;
        }
        this.solver.SetChain(this.bone1, this.bone2, this.tip, hand);
        this.solver.Initiate(hand);
        this.initiated = true;
      }
    }

    public void FixTransforms()
    {
      if (!this.initiated)
        return;
      this.solver.FixTransforms();
      if (!((UnityEngine.Object) this.bone3 != (UnityEngine.Object) null))
        return;
      this.bone3.localPosition = this.bone3DefaultLocalPosition;
      this.bone3.localRotation = this.bone3DefaultLocalRotation;
    }

    public void Update(float masterWeight)
    {
      if (!this.initiated)
        return;
      float num = this.weight * masterWeight;
      if ((double) num <= 0.0)
        return;
      this.solver.target = this.target;
      if ((UnityEngine.Object) this.target != (UnityEngine.Object) null)
      {
        this.IKPosition = this.target.position;
        this.IKRotation = this.target.rotation;
      }
      if (this.rotationDOF == Finger.DOF.One)
        this.IKRotation = Quaternion.FromToRotation(this.IKRotation * this.tipAxis, this.bone1.rotation * this.bone1Axis) * this.IKRotation;
      if ((UnityEngine.Object) this.bone3 != (UnityEngine.Object) null)
        this.bone3.rotation = (double) num * (double) this.rotationWeight < 1.0 ? Quaternion.Lerp(this.bone3.rotation, this.IKRotation * this.bone3RelativeToTarget, num * this.rotationWeight) : this.IKRotation * this.bone3RelativeToTarget;
      this.solver.IKPositionWeight = num;
      this.solver.IKRotationWeight = this.rotationWeight;
      this.solver.bendModifierWeight = this.rotationWeight;
      this.solver.Update();
    }

    [Serializable]
    public enum DOF
    {
      One,
      Three,
    }
  }
}

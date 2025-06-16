// Decompiled with JetBrains decompiler
// Type: RootMotion.FinalIK.IKSolver
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
namespace RootMotion.FinalIK
{
  [Serializable]
  public abstract class IKSolver
  {
    [HideInInspector]
    public Vector3 IKPosition;
    [Tooltip("The positional or the master weight of the solver.")]
    [Range(0.0f, 1f)]
    public float IKPositionWeight = 1f;
    public IKSolver.UpdateDelegate OnPreInitiate;
    public IKSolver.UpdateDelegate OnPostInitiate;
    public IKSolver.UpdateDelegate OnPreUpdate;
    public IKSolver.UpdateDelegate OnPostUpdate;
    protected bool firstInitiation = true;
    [SerializeField]
    [HideInInspector]
    protected Transform root;

    public bool IsValid()
    {
      string empty = string.Empty;
      return this.IsValid(ref empty);
    }

    public abstract bool IsValid(ref string message);

    public void Initiate(Transform root)
    {
      if (this.OnPreInitiate != null)
        this.OnPreInitiate();
      if ((UnityEngine.Object) root == (UnityEngine.Object) null)
        Debug.LogError((object) "Initiating IKSolver with null root Transform.");
      this.root = root;
      this.initiated = false;
      string empty = string.Empty;
      if (!this.IsValid(ref empty))
      {
        Warning.Log(empty, root);
      }
      else
      {
        this.OnInitiate();
        this.StoreDefaultLocalState();
        this.initiated = true;
        this.firstInitiation = false;
        if (this.OnPostInitiate == null)
          return;
        this.OnPostInitiate();
      }
    }

    public void Update()
    {
      if (this.OnPreUpdate != null)
        this.OnPreUpdate();
      if (this.firstInitiation)
        this.Initiate(this.root);
      if (!this.initiated)
        return;
      this.OnUpdate();
      if (this.OnPostUpdate == null)
        return;
      this.OnPostUpdate();
    }

    public virtual Vector3 GetIKPosition() => this.IKPosition;

    public void SetIKPosition(Vector3 position) => this.IKPosition = position;

    public float GetIKPositionWeight() => this.IKPositionWeight;

    public void SetIKPositionWeight(float weight)
    {
      this.IKPositionWeight = Mathf.Clamp(weight, 0.0f, 1f);
    }

    public Transform GetRoot() => this.root;

    public bool initiated { get; private set; }

    public abstract IKSolver.Point[] GetPoints();

    public abstract IKSolver.Point GetPoint(Transform transform);

    public abstract void FixTransforms();

    public abstract void StoreDefaultLocalState();

    protected abstract void OnInitiate();

    protected abstract void OnUpdate();

    protected void LogWarning(string message) => Warning.Log(message, this.root, true);

    public static Transform ContainsDuplicateBone(IKSolver.Bone[] bones)
    {
      for (int index1 = 0; index1 < bones.Length; ++index1)
      {
        for (int index2 = 0; index2 < bones.Length; ++index2)
        {
          if (index1 != index2 && (UnityEngine.Object) bones[index1].transform == (UnityEngine.Object) bones[index2].transform)
            return bones[index1].transform;
        }
      }
      return (Transform) null;
    }

    public static bool HierarchyIsValid(IKSolver.Bone[] bones)
    {
      for (int index = 1; index < bones.Length; ++index)
      {
        if (!Hierarchy.IsAncestor(bones[index].transform, bones[index - 1].transform))
          return false;
      }
      return true;
    }

    protected static float PreSolveBones(ref IKSolver.Bone[] bones)
    {
      float num = 0.0f;
      for (int index = 0; index < bones.Length; ++index)
      {
        bones[index].solverPosition = bones[index].transform.position;
        bones[index].solverRotation = bones[index].transform.rotation;
      }
      for (int index = 0; index < bones.Length; ++index)
      {
        if (index < bones.Length - 1)
        {
          bones[index].sqrMag = (bones[index + 1].solverPosition - bones[index].solverPosition).sqrMagnitude;
          bones[index].length = Mathf.Sqrt(bones[index].sqrMag);
          num += bones[index].length;
          bones[index].axis = Quaternion.Inverse(bones[index].solverRotation) * (bones[index + 1].solverPosition - bones[index].solverPosition);
        }
        else
        {
          bones[index].sqrMag = 0.0f;
          bones[index].length = 0.0f;
        }
      }
      return num;
    }

    [Serializable]
    public class Point
    {
      public Transform transform;
      [Range(0.0f, 1f)]
      public float weight = 1f;
      public Vector3 solverPosition;
      public Quaternion solverRotation = Quaternion.identity;
      public Vector3 defaultLocalPosition;
      public Quaternion defaultLocalRotation;

      public void StoreDefaultLocalState()
      {
        this.defaultLocalPosition = this.transform.localPosition;
        this.defaultLocalRotation = this.transform.localRotation;
      }

      public void FixTransform()
      {
        if (this.transform.localPosition != this.defaultLocalPosition)
          this.transform.localPosition = this.defaultLocalPosition;
        if (!(this.transform.localRotation != this.defaultLocalRotation))
          return;
        this.transform.localRotation = this.defaultLocalRotation;
      }

      public void UpdateSolverPosition() => this.solverPosition = this.transform.position;

      public void UpdateSolverLocalPosition() => this.solverPosition = this.transform.localPosition;

      public void UpdateSolverState()
      {
        this.solverPosition = this.transform.position;
        this.solverRotation = this.transform.rotation;
      }

      public void UpdateSolverLocalState()
      {
        this.solverPosition = this.transform.localPosition;
        this.solverRotation = this.transform.localRotation;
      }
    }

    [Serializable]
    public class Bone : IKSolver.Point
    {
      public float length;
      public float sqrMag;
      public Vector3 axis = -Vector3.right;
      private RotationLimit _rotationLimit;
      private bool isLimited = true;

      public RotationLimit rotationLimit
      {
        get
        {
          if (!this.isLimited)
            return (RotationLimit) null;
          if ((UnityEngine.Object) this._rotationLimit == (UnityEngine.Object) null)
            this._rotationLimit = this.transform.GetComponent<RotationLimit>();
          this.isLimited = (UnityEngine.Object) this._rotationLimit != (UnityEngine.Object) null;
          return this._rotationLimit;
        }
        set
        {
          this._rotationLimit = value;
          this.isLimited = (UnityEngine.Object) value != (UnityEngine.Object) null;
        }
      }

      public void Swing(Vector3 swingTarget, float weight = 1f)
      {
        if ((double) weight <= 0.0)
          return;
        Quaternion rotation = Quaternion.FromToRotation(this.transform.rotation * this.axis, swingTarget - this.transform.position);
        if ((double) weight >= 1.0)
          this.transform.rotation = rotation * this.transform.rotation;
        else
          this.transform.rotation = Quaternion.Lerp(Quaternion.identity, rotation, weight) * this.transform.rotation;
      }

      public static void SolverSwing(
        IKSolver.Bone[] bones,
        int index,
        Vector3 swingTarget,
        float weight = 1f)
      {
        if ((double) weight <= 0.0)
          return;
        Quaternion rotation = Quaternion.FromToRotation(bones[index].solverRotation * bones[index].axis, swingTarget - bones[index].solverPosition);
        if ((double) weight >= 1.0)
        {
          for (int index1 = index; index1 < bones.Length; ++index1)
            bones[index1].solverRotation = rotation * bones[index1].solverRotation;
        }
        else
        {
          for (int index2 = index; index2 < bones.Length; ++index2)
            bones[index2].solverRotation = Quaternion.Lerp(Quaternion.identity, rotation, weight) * bones[index2].solverRotation;
        }
      }

      public void Swing2D(Vector3 swingTarget, float weight = 1f)
      {
        if ((double) weight <= 0.0)
          return;
        Vector3 vector3_1 = this.transform.rotation * this.axis;
        Vector3 vector3_2 = swingTarget - this.transform.position;
        this.transform.rotation = Quaternion.AngleAxis(Mathf.DeltaAngle(Mathf.Atan2(vector3_1.x, vector3_1.y) * 57.29578f, Mathf.Atan2(vector3_2.x, vector3_2.y) * 57.29578f) * weight, Vector3.back) * this.transform.rotation;
      }

      public void SetToSolverPosition() => this.transform.position = this.solverPosition;

      public Bone()
      {
      }

      public Bone(Transform transform) => this.transform = transform;

      public Bone(Transform transform, float weight)
      {
        this.transform = transform;
        this.weight = weight;
      }
    }

    [Serializable]
    public class Node : IKSolver.Point
    {
      public float length;
      public float effectorPositionWeight;
      public float effectorRotationWeight;
      public Vector3 offset;

      public Node()
      {
      }

      public Node(Transform transform) => this.transform = transform;

      public Node(Transform transform, float weight)
      {
        this.transform = transform;
        this.weight = weight;
      }
    }

    public delegate void UpdateDelegate();

    public delegate void IterationDelegate(int i);
  }
}

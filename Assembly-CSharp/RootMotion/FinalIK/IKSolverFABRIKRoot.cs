// Decompiled with JetBrains decompiler
// Type: RootMotion.FinalIK.IKSolverFABRIKRoot
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
namespace RootMotion.FinalIK
{
  [Serializable]
  public class IKSolverFABRIKRoot : IKSolver
  {
    public int iterations = 4;
    [Range(0.0f, 1f)]
    public float rootPin = 0.0f;
    public FABRIKChain[] chains = new FABRIKChain[0];
    private bool zeroWeightApplied;
    private bool[] isRoot;
    private Vector3 rootDefaultPosition;

    public override bool IsValid(ref string message)
    {
      if (this.chains.Length == 0)
      {
        message = "IKSolverFABRIKRoot contains no chains.";
        return false;
      }
      foreach (FABRIKChain chain in this.chains)
      {
        if (!chain.IsValid(ref message))
          return false;
      }
      for (int index1 = 0; index1 < this.chains.Length; ++index1)
      {
        for (int index2 = 0; index2 < this.chains.Length; ++index2)
        {
          if (index1 != index2 && (UnityEngine.Object) this.chains[index1].ik == (UnityEngine.Object) this.chains[index2].ik)
          {
            message = this.chains[index1].ik.name + " is represented more than once in IKSolverFABRIKRoot chain.";
            return false;
          }
        }
      }
      for (int index3 = 0; index3 < this.chains.Length; ++index3)
      {
        for (int index4 = 0; index4 < this.chains[index3].children.Length; ++index4)
        {
          int child = this.chains[index3].children[index4];
          if (child < 0)
          {
            message = this.chains[index3].ik.name + "IKSolverFABRIKRoot chain at index " + (object) index3 + " has invalid children array. Child index is < 0.";
            return false;
          }
          if (child == index3)
          {
            message = this.chains[index3].ik.name + "IKSolverFABRIKRoot chain at index " + (object) index3 + " has invalid children array. Child index is referencing to itself.";
            return false;
          }
          if (child >= this.chains.Length)
          {
            message = this.chains[index3].ik.name + "IKSolverFABRIKRoot chain at index " + (object) index3 + " has invalid children array. Child index > number of chains";
            return false;
          }
          for (int index5 = 0; index5 < this.chains.Length; ++index5)
          {
            if (child == index5)
            {
              for (int index6 = 0; index6 < this.chains[index5].children.Length; ++index6)
              {
                if (this.chains[index5].children[index6] == index3)
                {
                  message = "Circular parenting. " + this.chains[index5].ik.name + " already has " + this.chains[index3].ik.name + " listed as it's child.";
                  return false;
                }
              }
            }
          }
          for (int index7 = 0; index7 < this.chains[index3].children.Length; ++index7)
          {
            if (index4 != index7 && this.chains[index3].children[index7] == child)
            {
              message = "Chain number " + (object) child + " is represented more than once in the children of " + this.chains[index3].ik.name;
              return false;
            }
          }
        }
      }
      return true;
    }

    public override void StoreDefaultLocalState()
    {
      this.rootDefaultPosition = this.root.localPosition;
      for (int index = 0; index < this.chains.Length; ++index)
        this.chains[index].ik.solver.StoreDefaultLocalState();
    }

    public override void FixTransforms()
    {
      if (!this.initiated)
        return;
      this.root.localPosition = this.rootDefaultPosition;
      for (int index = 0; index < this.chains.Length; ++index)
        this.chains[index].ik.solver.FixTransforms();
    }

    protected override void OnInitiate()
    {
      for (int index = 0; index < this.chains.Length; ++index)
        this.chains[index].Initiate();
      this.isRoot = new bool[this.chains.Length];
      for (int index = 0; index < this.chains.Length; ++index)
        this.isRoot[index] = this.IsRoot(index);
    }

    private bool IsRoot(int index)
    {
      for (int index1 = 0; index1 < this.chains.Length; ++index1)
      {
        for (int index2 = 0; index2 < this.chains[index1].children.Length; ++index2)
        {
          if (this.chains[index1].children[index2] == index)
            return false;
        }
      }
      return true;
    }

    protected override void OnUpdate()
    {
      if ((double) this.IKPositionWeight <= 0.0 && this.zeroWeightApplied)
        return;
      this.IKPositionWeight = Mathf.Clamp(this.IKPositionWeight, 0.0f, 1f);
      for (int index = 0; index < this.chains.Length; ++index)
        this.chains[index].ik.solver.IKPositionWeight = this.IKPositionWeight;
      if ((double) this.IKPositionWeight <= 0.0)
      {
        this.zeroWeightApplied = true;
      }
      else
      {
        this.zeroWeightApplied = false;
        for (int index1 = 0; index1 < this.iterations; ++index1)
        {
          for (int index2 = 0; index2 < this.chains.Length; ++index2)
          {
            if (this.isRoot[index2])
              this.chains[index2].Stage1(this.chains);
          }
          Vector3 centroid = this.GetCentroid();
          this.root.position = centroid;
          for (int index3 = 0; index3 < this.chains.Length; ++index3)
          {
            if (this.isRoot[index3])
              this.chains[index3].Stage2(centroid, this.chains);
          }
        }
      }
    }

    public override IKSolver.Point[] GetPoints()
    {
      IKSolver.Point[] array = new IKSolver.Point[0];
      for (int index = 0; index < this.chains.Length; ++index)
        this.AddPointsToArray(ref array, this.chains[index]);
      return array;
    }

    public override IKSolver.Point GetPoint(Transform transform)
    {
      for (int index = 0; index < this.chains.Length; ++index)
      {
        IKSolver.Point point = this.chains[index].ik.solver.GetPoint(transform);
        if (point != null)
          return point;
      }
      return (IKSolver.Point) null;
    }

    private void AddPointsToArray(ref IKSolver.Point[] array, FABRIKChain chain)
    {
      IKSolver.Point[] points = chain.ik.solver.GetPoints();
      Array.Resize<IKSolver.Point>(ref array, array.Length + points.Length);
      int index1 = 0;
      for (int index2 = array.Length - points.Length; index2 < array.Length; ++index2)
      {
        array[index2] = points[index1];
        ++index1;
      }
    }

    private Vector3 GetCentroid()
    {
      Vector3 position = this.root.position;
      if ((double) this.rootPin >= 1.0)
        return position;
      float max = 0.0f;
      for (int index = 0; index < this.chains.Length; ++index)
      {
        if (this.isRoot[index])
          max += this.chains[index].pull;
      }
      for (int index = 0; index < this.chains.Length; ++index)
      {
        if (this.isRoot[index] && (double) max > 0.0)
          position += (this.chains[index].ik.solver.bones[0].solverPosition - this.root.position) * (this.chains[index].pull / Mathf.Clamp(max, 1f, max));
      }
      return Vector3.Lerp(position, this.root.position, this.rootPin);
    }
  }
}

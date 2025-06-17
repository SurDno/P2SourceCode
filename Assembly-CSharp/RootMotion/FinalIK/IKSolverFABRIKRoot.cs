using System;
using UnityEngine;

namespace RootMotion.FinalIK
{
  [Serializable]
  public class IKSolverFABRIKRoot : IKSolver
  {
    public int iterations = 4;
    [Range(0.0f, 1f)]
    public float rootPin;
    public FABRIKChain[] chains = [];
    private bool zeroWeightApplied;
    private bool[] isRoot;
    private Vector3 rootDefaultPosition;

    public override bool IsValid(ref string message)
    {
      if (chains.Length == 0)
      {
        message = "IKSolverFABRIKRoot contains no chains.";
        return false;
      }
      foreach (FABRIKChain chain in chains)
      {
        if (!chain.IsValid(ref message))
          return false;
      }
      for (int index1 = 0; index1 < chains.Length; ++index1)
      {
        for (int index2 = 0; index2 < chains.Length; ++index2)
        {
          if (index1 != index2 && chains[index1].ik == chains[index2].ik)
          {
            message = chains[index1].ik.name + " is represented more than once in IKSolverFABRIKRoot chain.";
            return false;
          }
        }
      }
      for (int index3 = 0; index3 < chains.Length; ++index3)
      {
        for (int index4 = 0; index4 < chains[index3].children.Length; ++index4)
        {
          int child = chains[index3].children[index4];
          if (child < 0)
          {
            message = chains[index3].ik.name + "IKSolverFABRIKRoot chain at index " + index3 + " has invalid children array. Child index is < 0.";
            return false;
          }
          if (child == index3)
          {
            message = chains[index3].ik.name + "IKSolverFABRIKRoot chain at index " + index3 + " has invalid children array. Child index is referencing to itself.";
            return false;
          }
          if (child >= chains.Length)
          {
            message = chains[index3].ik.name + "IKSolverFABRIKRoot chain at index " + index3 + " has invalid children array. Child index > number of chains";
            return false;
          }
          for (int index5 = 0; index5 < chains.Length; ++index5)
          {
            if (child == index5)
            {
              for (int index6 = 0; index6 < chains[index5].children.Length; ++index6)
              {
                if (chains[index5].children[index6] == index3)
                {
                  message = "Circular parenting. " + chains[index5].ik.name + " already has " + chains[index3].ik.name + " listed as it's child.";
                  return false;
                }
              }
            }
          }
          for (int index7 = 0; index7 < chains[index3].children.Length; ++index7)
          {
            if (index4 != index7 && chains[index3].children[index7] == child)
            {
              message = "Chain number " + child + " is represented more than once in the children of " + chains[index3].ik.name;
              return false;
            }
          }
        }
      }
      return true;
    }

    public override void StoreDefaultLocalState()
    {
      rootDefaultPosition = root.localPosition;
      for (int index = 0; index < chains.Length; ++index)
        chains[index].ik.solver.StoreDefaultLocalState();
    }

    public override void FixTransforms()
    {
      if (!initiated)
        return;
      root.localPosition = rootDefaultPosition;
      for (int index = 0; index < chains.Length; ++index)
        chains[index].ik.solver.FixTransforms();
    }

    protected override void OnInitiate()
    {
      for (int index = 0; index < chains.Length; ++index)
        chains[index].Initiate();
      isRoot = new bool[chains.Length];
      for (int index = 0; index < chains.Length; ++index)
        isRoot[index] = IsRoot(index);
    }

    private bool IsRoot(int index)
    {
      for (int index1 = 0; index1 < chains.Length; ++index1)
      {
        for (int index2 = 0; index2 < chains[index1].children.Length; ++index2)
        {
          if (chains[index1].children[index2] == index)
            return false;
        }
      }
      return true;
    }

    protected override void OnUpdate()
    {
      if (IKPositionWeight <= 0.0 && zeroWeightApplied)
        return;
      IKPositionWeight = Mathf.Clamp(IKPositionWeight, 0.0f, 1f);
      for (int index = 0; index < chains.Length; ++index)
        chains[index].ik.solver.IKPositionWeight = IKPositionWeight;
      if (IKPositionWeight <= 0.0)
      {
        zeroWeightApplied = true;
      }
      else
      {
        zeroWeightApplied = false;
        for (int index1 = 0; index1 < iterations; ++index1)
        {
          for (int index2 = 0; index2 < chains.Length; ++index2)
          {
            if (isRoot[index2])
              chains[index2].Stage1(chains);
          }
          Vector3 centroid = GetCentroid();
          root.position = centroid;
          for (int index3 = 0; index3 < chains.Length; ++index3)
          {
            if (isRoot[index3])
              chains[index3].Stage2(centroid, chains);
          }
        }
      }
    }

    public override Point[] GetPoints()
    {
      Point[] array = [];
      for (int index = 0; index < chains.Length; ++index)
        AddPointsToArray(ref array, chains[index]);
      return array;
    }

    public override Point GetPoint(Transform transform)
    {
      for (int index = 0; index < chains.Length; ++index)
      {
        Point point = chains[index].ik.solver.GetPoint(transform);
        if (point != null)
          return point;
      }
      return null;
    }

    private void AddPointsToArray(ref Point[] array, FABRIKChain chain)
    {
      Point[] points = chain.ik.solver.GetPoints();
      Array.Resize(ref array, array.Length + points.Length);
      int index1 = 0;
      for (int index2 = array.Length - points.Length; index2 < array.Length; ++index2)
      {
        array[index2] = points[index1];
        ++index1;
      }
    }

    private Vector3 GetCentroid()
    {
      Vector3 position = root.position;
      if (rootPin >= 1.0)
        return position;
      float max = 0.0f;
      for (int index = 0; index < chains.Length; ++index)
      {
        if (isRoot[index])
          max += chains[index].pull;
      }
      for (int index = 0; index < chains.Length; ++index)
      {
        if (isRoot[index] && max > 0.0)
          position += (chains[index].ik.solver.bones[0].solverPosition - root.position) * (chains[index].pull / Mathf.Clamp(max, 1f, max));
      }
      return Vector3.Lerp(position, root.position, rootPin);
    }
  }
}

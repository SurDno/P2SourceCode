using System;
using UnityEngine;

namespace RootMotion.FinalIK
{
  [Serializable]
  public class IKSolverFullBody : IKSolver
  {
    [Range(0.0f, 10f)]
    public int iterations = 4;
    public FBIKChain[] chain = new FBIKChain[0];
    public IKEffector[] effectors = new IKEffector[0];
    public IKMappingSpine spineMapping = new IKMappingSpine();
    public IKMappingBone[] boneMappings = new IKMappingBone[0];
    public IKMappingLimb[] limbMappings = new IKMappingLimb[0];
    public bool FABRIKPass = true;
    public UpdateDelegate OnPreRead;
    public UpdateDelegate OnPreSolve;
    public IterationDelegate OnPreIteration;
    public IterationDelegate OnPostIteration;
    public UpdateDelegate OnPreBend;
    public UpdateDelegate OnPostSolve;
    public UpdateDelegate OnStoreDefaultLocalState;
    public UpdateDelegate OnFixTransforms;

    public IKEffector GetEffector(Transform t)
    {
      for (int index = 0; index < effectors.Length; ++index)
      {
        if (effectors[index].bone == t)
          return effectors[index];
      }
      return null;
    }

    public FBIKChain GetChain(Transform transform)
    {
      int chainIndex = GetChainIndex(transform);
      return chainIndex == -1 ? null : chain[chainIndex];
    }

    public int GetChainIndex(Transform transform)
    {
      for (int chainIndex = 0; chainIndex < chain.Length; ++chainIndex)
      {
        for (int index = 0; index < chain[chainIndex].nodes.Length; ++index)
        {
          if (chain[chainIndex].nodes[index].transform == transform)
            return chainIndex;
        }
      }
      return -1;
    }

    public Node GetNode(int chainIndex, int nodeIndex)
    {
      return chain[chainIndex].nodes[nodeIndex];
    }

    public void GetChainAndNodeIndexes(Transform transform, out int chainIndex, out int nodeIndex)
    {
      chainIndex = GetChainIndex(transform);
      if (chainIndex == -1)
        nodeIndex = -1;
      else
        nodeIndex = chain[chainIndex].GetNodeIndex(transform);
    }

    public override Point[] GetPoints()
    {
      int length = 0;
      for (int index = 0; index < chain.Length; ++index)
        length += chain[index].nodes.Length;
      Point[] points = new Point[length];
      int index1 = 0;
      for (int index2 = 0; index2 < chain.Length; ++index2)
      {
        for (int index3 = 0; index3 < chain[index2].nodes.Length; ++index3)
          points[index1] = chain[index2].nodes[index3];
      }
      return points;
    }

    public override Point GetPoint(Transform transform)
    {
      for (int index1 = 0; index1 < chain.Length; ++index1)
      {
        for (int index2 = 0; index2 < chain[index1].nodes.Length; ++index2)
        {
          if (chain[index1].nodes[index2].transform == transform)
            return chain[index1].nodes[index2];
        }
      }
      return null;
    }

    public override bool IsValid(ref string message)
    {
      if (chain == null)
      {
        message = "FBIK chain is null, can't initiate solver.";
        return false;
      }
      if (chain.Length == 0)
      {
        message = "FBIK chain length is 0, can't initiate solver.";
        return false;
      }
      for (int index = 0; index < chain.Length; ++index)
      {
        if (!chain[index].IsValid(ref message))
          return false;
      }
      foreach (IKEffector effector in effectors)
      {
        if (!effector.IsValid(this, ref message))
          return false;
      }
      if (!spineMapping.IsValid(this, ref message))
        return false;
      foreach (IKMapping limbMapping in limbMappings)
      {
        if (!limbMapping.IsValid(this, ref message))
          return false;
      }
      foreach (IKMapping boneMapping in boneMappings)
      {
        if (!boneMapping.IsValid(this, ref message))
          return false;
      }
      return true;
    }

    public override void StoreDefaultLocalState()
    {
      spineMapping.StoreDefaultLocalState();
      for (int index = 0; index < limbMappings.Length; ++index)
        limbMappings[index].StoreDefaultLocalState();
      for (int index = 0; index < boneMappings.Length; ++index)
        boneMappings[index].StoreDefaultLocalState();
      if (OnStoreDefaultLocalState == null)
        return;
      OnStoreDefaultLocalState();
    }

    public override void FixTransforms()
    {
      if (!initiated || IKPositionWeight <= 0.0)
        return;
      spineMapping.FixTransforms();
      for (int index = 0; index < limbMappings.Length; ++index)
        limbMappings[index].FixTransforms();
      for (int index = 0; index < boneMappings.Length; ++index)
        boneMappings[index].FixTransforms();
      if (OnFixTransforms == null)
        return;
      OnFixTransforms();
    }

    protected override void OnInitiate()
    {
      for (int index = 0; index < chain.Length; ++index)
        chain[index].Initiate(this);
      foreach (IKEffector effector in effectors)
        effector.Initiate(this);
      spineMapping.Initiate(this);
      foreach (IKMapping boneMapping in boneMappings)
        boneMapping.Initiate(this);
      foreach (IKMapping limbMapping in limbMappings)
        limbMapping.Initiate(this);
    }

    protected override void OnUpdate()
    {
      if (IKPositionWeight <= 0.0)
      {
        for (int index = 0; index < effectors.Length; ++index)
          effectors[index].positionOffset = Vector3.zero;
      }
      else
      {
        if (chain.Length == 0)
          return;
        IKPositionWeight = Mathf.Clamp(IKPositionWeight, 0.0f, 1f);
        if (OnPreRead != null)
          OnPreRead();
        ReadPose();
        if (OnPreSolve != null)
          OnPreSolve();
        Solve();
        if (OnPostSolve != null)
          OnPostSolve();
        WritePose();
        for (int index = 0; index < effectors.Length; ++index)
          effectors[index].OnPostWrite();
      }
    }

    protected virtual void ReadPose()
    {
      for (int index = 0; index < chain.Length; ++index)
      {
        if (chain[index].bendConstraint.initiated)
          chain[index].bendConstraint.LimitBend(IKPositionWeight, GetEffector(chain[index].nodes[2].transform).positionWeight);
      }
      for (int index = 0; index < effectors.Length; ++index)
        effectors[index].ResetOffset(this);
      for (int index = 0; index < effectors.Length; ++index)
        effectors[index].OnPreSolve(this);
      for (int index = 0; index < chain.Length; ++index)
        chain[index].ReadPose(this, iterations > 0);
      if (iterations > 0)
      {
        spineMapping.ReadPose();
        for (int index = 0; index < boneMappings.Length; ++index)
          boneMappings[index].ReadPose();
      }
      for (int index = 0; index < limbMappings.Length; ++index)
        limbMappings[index].ReadPose();
    }

    protected virtual void Solve()
    {
      if (iterations > 0)
      {
        for (int i = 0; i < (FABRIKPass ? iterations : 1); ++i)
        {
          if (OnPreIteration != null)
            OnPreIteration(i);
          for (int index = 0; index < effectors.Length; ++index)
          {
            if (effectors[index].isEndEffector)
              effectors[index].Update(this);
          }
          if (FABRIKPass)
          {
            chain[0].Push(this);
            if (FABRIKPass)
              chain[0].Reach(this);
            for (int index = 0; index < effectors.Length; ++index)
            {
              if (!effectors[index].isEndEffector)
                effectors[index].Update(this);
            }
          }
          chain[0].SolveTrigonometric(this);
          if (FABRIKPass)
          {
            chain[0].Stage1(this);
            for (int index = 0; index < effectors.Length; ++index)
            {
              if (!effectors[index].isEndEffector)
                effectors[index].Update(this);
            }
            chain[0].Stage2(this, chain[0].nodes[0].solverPosition);
          }
          if (OnPostIteration != null)
            OnPostIteration(i);
        }
      }
      if (OnPreBend != null)
        OnPreBend();
      for (int index = 0; index < effectors.Length; ++index)
      {
        if (effectors[index].isEndEffector)
          effectors[index].Update(this);
      }
      ApplyBendConstraints();
    }

    protected virtual void ApplyBendConstraints() => chain[0].SolveTrigonometric(this, true);

    protected virtual void WritePose()
    {
      if (IKPositionWeight <= 0.0)
        return;
      if (iterations > 0)
      {
        spineMapping.WritePose(this);
        for (int index = 0; index < boneMappings.Length; ++index)
          boneMappings[index].WritePose(IKPositionWeight);
      }
      for (int index = 0; index < limbMappings.Length; ++index)
        limbMappings[index].WritePose(this, iterations > 0);
    }
  }
}

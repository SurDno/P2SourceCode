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
    public IKSolver.UpdateDelegate OnPreRead;
    public IKSolver.UpdateDelegate OnPreSolve;
    public IKSolver.IterationDelegate OnPreIteration;
    public IKSolver.IterationDelegate OnPostIteration;
    public IKSolver.UpdateDelegate OnPreBend;
    public IKSolver.UpdateDelegate OnPostSolve;
    public IKSolver.UpdateDelegate OnStoreDefaultLocalState;
    public IKSolver.UpdateDelegate OnFixTransforms;

    public IKEffector GetEffector(Transform t)
    {
      for (int index = 0; index < this.effectors.Length; ++index)
      {
        if ((UnityEngine.Object) this.effectors[index].bone == (UnityEngine.Object) t)
          return this.effectors[index];
      }
      return (IKEffector) null;
    }

    public FBIKChain GetChain(Transform transform)
    {
      int chainIndex = this.GetChainIndex(transform);
      return chainIndex == -1 ? (FBIKChain) null : this.chain[chainIndex];
    }

    public int GetChainIndex(Transform transform)
    {
      for (int chainIndex = 0; chainIndex < this.chain.Length; ++chainIndex)
      {
        for (int index = 0; index < this.chain[chainIndex].nodes.Length; ++index)
        {
          if ((UnityEngine.Object) this.chain[chainIndex].nodes[index].transform == (UnityEngine.Object) transform)
            return chainIndex;
        }
      }
      return -1;
    }

    public IKSolver.Node GetNode(int chainIndex, int nodeIndex)
    {
      return this.chain[chainIndex].nodes[nodeIndex];
    }

    public void GetChainAndNodeIndexes(Transform transform, out int chainIndex, out int nodeIndex)
    {
      chainIndex = this.GetChainIndex(transform);
      if (chainIndex == -1)
        nodeIndex = -1;
      else
        nodeIndex = this.chain[chainIndex].GetNodeIndex(transform);
    }

    public override IKSolver.Point[] GetPoints()
    {
      int length = 0;
      for (int index = 0; index < this.chain.Length; ++index)
        length += this.chain[index].nodes.Length;
      IKSolver.Point[] points = new IKSolver.Point[length];
      int index1 = 0;
      for (int index2 = 0; index2 < this.chain.Length; ++index2)
      {
        for (int index3 = 0; index3 < this.chain[index2].nodes.Length; ++index3)
          points[index1] = (IKSolver.Point) this.chain[index2].nodes[index3];
      }
      return points;
    }

    public override IKSolver.Point GetPoint(Transform transform)
    {
      for (int index1 = 0; index1 < this.chain.Length; ++index1)
      {
        for (int index2 = 0; index2 < this.chain[index1].nodes.Length; ++index2)
        {
          if ((UnityEngine.Object) this.chain[index1].nodes[index2].transform == (UnityEngine.Object) transform)
            return (IKSolver.Point) this.chain[index1].nodes[index2];
        }
      }
      return (IKSolver.Point) null;
    }

    public override bool IsValid(ref string message)
    {
      if (this.chain == null)
      {
        message = "FBIK chain is null, can't initiate solver.";
        return false;
      }
      if (this.chain.Length == 0)
      {
        message = "FBIK chain length is 0, can't initiate solver.";
        return false;
      }
      for (int index = 0; index < this.chain.Length; ++index)
      {
        if (!this.chain[index].IsValid(ref message))
          return false;
      }
      foreach (IKEffector effector in this.effectors)
      {
        if (!effector.IsValid((IKSolver) this, ref message))
          return false;
      }
      if (!this.spineMapping.IsValid((IKSolver) this, ref message))
        return false;
      foreach (IKMapping limbMapping in this.limbMappings)
      {
        if (!limbMapping.IsValid((IKSolver) this, ref message))
          return false;
      }
      foreach (IKMapping boneMapping in this.boneMappings)
      {
        if (!boneMapping.IsValid((IKSolver) this, ref message))
          return false;
      }
      return true;
    }

    public override void StoreDefaultLocalState()
    {
      this.spineMapping.StoreDefaultLocalState();
      for (int index = 0; index < this.limbMappings.Length; ++index)
        this.limbMappings[index].StoreDefaultLocalState();
      for (int index = 0; index < this.boneMappings.Length; ++index)
        this.boneMappings[index].StoreDefaultLocalState();
      if (this.OnStoreDefaultLocalState == null)
        return;
      this.OnStoreDefaultLocalState();
    }

    public override void FixTransforms()
    {
      if (!this.initiated || (double) this.IKPositionWeight <= 0.0)
        return;
      this.spineMapping.FixTransforms();
      for (int index = 0; index < this.limbMappings.Length; ++index)
        this.limbMappings[index].FixTransforms();
      for (int index = 0; index < this.boneMappings.Length; ++index)
        this.boneMappings[index].FixTransforms();
      if (this.OnFixTransforms == null)
        return;
      this.OnFixTransforms();
    }

    protected override void OnInitiate()
    {
      for (int index = 0; index < this.chain.Length; ++index)
        this.chain[index].Initiate(this);
      foreach (IKEffector effector in this.effectors)
        effector.Initiate(this);
      this.spineMapping.Initiate(this);
      foreach (IKMapping boneMapping in this.boneMappings)
        boneMapping.Initiate(this);
      foreach (IKMapping limbMapping in this.limbMappings)
        limbMapping.Initiate(this);
    }

    protected override void OnUpdate()
    {
      if ((double) this.IKPositionWeight <= 0.0)
      {
        for (int index = 0; index < this.effectors.Length; ++index)
          this.effectors[index].positionOffset = Vector3.zero;
      }
      else
      {
        if (this.chain.Length == 0)
          return;
        this.IKPositionWeight = Mathf.Clamp(this.IKPositionWeight, 0.0f, 1f);
        if (this.OnPreRead != null)
          this.OnPreRead();
        this.ReadPose();
        if (this.OnPreSolve != null)
          this.OnPreSolve();
        this.Solve();
        if (this.OnPostSolve != null)
          this.OnPostSolve();
        this.WritePose();
        for (int index = 0; index < this.effectors.Length; ++index)
          this.effectors[index].OnPostWrite();
      }
    }

    protected virtual void ReadPose()
    {
      for (int index = 0; index < this.chain.Length; ++index)
      {
        if (this.chain[index].bendConstraint.initiated)
          this.chain[index].bendConstraint.LimitBend(this.IKPositionWeight, this.GetEffector(this.chain[index].nodes[2].transform).positionWeight);
      }
      for (int index = 0; index < this.effectors.Length; ++index)
        this.effectors[index].ResetOffset(this);
      for (int index = 0; index < this.effectors.Length; ++index)
        this.effectors[index].OnPreSolve(this);
      for (int index = 0; index < this.chain.Length; ++index)
        this.chain[index].ReadPose(this, this.iterations > 0);
      if (this.iterations > 0)
      {
        this.spineMapping.ReadPose();
        for (int index = 0; index < this.boneMappings.Length; ++index)
          this.boneMappings[index].ReadPose();
      }
      for (int index = 0; index < this.limbMappings.Length; ++index)
        this.limbMappings[index].ReadPose();
    }

    protected virtual void Solve()
    {
      if (this.iterations > 0)
      {
        for (int i = 0; i < (this.FABRIKPass ? this.iterations : 1); ++i)
        {
          if (this.OnPreIteration != null)
            this.OnPreIteration(i);
          for (int index = 0; index < this.effectors.Length; ++index)
          {
            if (this.effectors[index].isEndEffector)
              this.effectors[index].Update(this);
          }
          if (this.FABRIKPass)
          {
            this.chain[0].Push(this);
            if (this.FABRIKPass)
              this.chain[0].Reach(this);
            for (int index = 0; index < this.effectors.Length; ++index)
            {
              if (!this.effectors[index].isEndEffector)
                this.effectors[index].Update(this);
            }
          }
          this.chain[0].SolveTrigonometric(this);
          if (this.FABRIKPass)
          {
            this.chain[0].Stage1(this);
            for (int index = 0; index < this.effectors.Length; ++index)
            {
              if (!this.effectors[index].isEndEffector)
                this.effectors[index].Update(this);
            }
            this.chain[0].Stage2(this, this.chain[0].nodes[0].solverPosition);
          }
          if (this.OnPostIteration != null)
            this.OnPostIteration(i);
        }
      }
      if (this.OnPreBend != null)
        this.OnPreBend();
      for (int index = 0; index < this.effectors.Length; ++index)
      {
        if (this.effectors[index].isEndEffector)
          this.effectors[index].Update(this);
      }
      this.ApplyBendConstraints();
    }

    protected virtual void ApplyBendConstraints() => this.chain[0].SolveTrigonometric(this, true);

    protected virtual void WritePose()
    {
      if ((double) this.IKPositionWeight <= 0.0)
        return;
      if (this.iterations > 0)
      {
        this.spineMapping.WritePose(this);
        for (int index = 0; index < this.boneMappings.Length; ++index)
          this.boneMappings[index].WritePose(this.IKPositionWeight);
      }
      for (int index = 0; index < this.limbMappings.Length; ++index)
        this.limbMappings[index].WritePose(this, this.iterations > 0);
    }
  }
}

using System;
using UnityEngine;

namespace RootMotion.FinalIK
{
  [Serializable]
  public class FABRIKChain
  {
    public FABRIK ik;
    [Range(0.0f, 1f)]
    public float pull = 1f;
    [Range(0.0f, 1f)]
    public float pin = 1f;
    public int[] children = new int[0];

    public bool IsValid(ref string message)
    {
      if ((UnityEngine.Object) this.ik == (UnityEngine.Object) null)
      {
        message = "IK unassigned in FABRIKChain.";
        return false;
      }
      return this.ik.solver.IsValid(ref message);
    }

    public void Initiate() => this.ik.enabled = false;

    public void Stage1(FABRIKChain[] chain)
    {
      for (int index = 0; index < this.children.Length; ++index)
        chain[this.children[index]].Stage1(chain);
      if (this.children.Length == 0)
        this.ik.solver.SolveForward(this.ik.solver.GetIKPosition());
      else
        this.ik.solver.SolveForward(this.GetCentroid(chain));
    }

    public void Stage2(Vector3 rootPosition, FABRIKChain[] chain)
    {
      this.ik.solver.SolveBackward(rootPosition);
      for (int index = 0; index < this.children.Length; ++index)
        chain[this.children[index]].Stage2(this.ik.solver.bones[this.ik.solver.bones.Length - 1].transform.position, chain);
    }

    private Vector3 GetCentroid(FABRIKChain[] chain)
    {
      Vector3 ikPosition = this.ik.solver.GetIKPosition();
      if ((double) this.pin >= 1.0)
        return ikPosition;
      float num1 = 0.0f;
      for (int index = 0; index < this.children.Length; ++index)
        num1 += chain[this.children[index]].pull;
      if ((double) num1 <= 0.0)
        return ikPosition;
      if ((double) num1 < 1.0)
        num1 = 1f;
      Vector3 vector3_1 = ikPosition;
      for (int index = 0; index < this.children.Length; ++index)
      {
        Vector3 vector3_2 = chain[this.children[index]].ik.solver.bones[0].solverPosition - ikPosition;
        float num2 = chain[this.children[index]].pull / num1;
        vector3_1 += vector3_2 * num2;
      }
      return (double) this.pin <= 0.0 ? vector3_1 : vector3_1 + (ikPosition - vector3_1) * this.pin;
    }
  }
}

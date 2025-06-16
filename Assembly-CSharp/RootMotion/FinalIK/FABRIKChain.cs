using System;

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
      if ((UnityEngine.Object) ik == (UnityEngine.Object) null)
      {
        message = "IK unassigned in FABRIKChain.";
        return false;
      }
      return ik.solver.IsValid(ref message);
    }

    public void Initiate() => ik.enabled = false;

    public void Stage1(FABRIKChain[] chain)
    {
      for (int index = 0; index < children.Length; ++index)
        chain[children[index]].Stage1(chain);
      if (children.Length == 0)
        ik.solver.SolveForward(ik.solver.GetIKPosition());
      else
        ik.solver.SolveForward(GetCentroid(chain));
    }

    public void Stage2(Vector3 rootPosition, FABRIKChain[] chain)
    {
      ik.solver.SolveBackward(rootPosition);
      for (int index = 0; index < children.Length; ++index)
        chain[children[index]].Stage2(ik.solver.bones[ik.solver.bones.Length - 1].transform.position, chain);
    }

    private Vector3 GetCentroid(FABRIKChain[] chain)
    {
      Vector3 ikPosition = ik.solver.GetIKPosition();
      if (pin >= 1.0)
        return ikPosition;
      float num1 = 0.0f;
      for (int index = 0; index < children.Length; ++index)
        num1 += chain[children[index]].pull;
      if (num1 <= 0.0)
        return ikPosition;
      if (num1 < 1.0)
        num1 = 1f;
      Vector3 vector3_1 = ikPosition;
      for (int index = 0; index < children.Length; ++index)
      {
        Vector3 vector3_2 = chain[children[index]].ik.solver.bones[0].solverPosition - ikPosition;
        float num2 = chain[children[index]].pull / num1;
        vector3_1 += vector3_2 * num2;
      }
      return pin <= 0.0 ? vector3_1 : vector3_1 + (ikPosition - vector3_1) * pin;
    }
  }
}

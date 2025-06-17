using UnityEngine;

namespace RootMotion.FinalIK
{
  public class HandPoser : Poser
  {
    private Transform _poseRoot;
    private Transform[] children;
    private Transform[] poseChildren;
    private Vector3[] defaultLocalPositions;
    private Quaternion[] defaultLocalRotations;

    public override void AutoMapping()
    {
      poseChildren = !(poseRoot == null) ? poseRoot.GetComponentsInChildren<Transform>() : [];
      _poseRoot = poseRoot;
    }

    protected override void InitiatePoser()
    {
      children = GetComponentsInChildren<Transform>();
      StoreDefaultState();
    }

    protected override void FixPoserTransforms()
    {
      for (int index = 0; index < children.Length; ++index)
      {
        children[index].localPosition = defaultLocalPositions[index];
        children[index].localRotation = defaultLocalRotations[index];
      }
    }

    protected override void UpdatePoser()
    {
      if (weight <= 0.0 || localPositionWeight <= 0.0 && localRotationWeight <= 0.0)
        return;
      if (_poseRoot != poseRoot)
        AutoMapping();
      if (poseRoot == null)
        return;
      if (children.Length != poseChildren.Length)
      {
        Warning.Log("Number of children does not match with the pose", transform);
      }
      else
      {
        float t1 = localRotationWeight * weight;
        float t2 = localPositionWeight * weight;
        for (int index = 0; index < children.Length; ++index)
        {
          if (children[index] != transform)
          {
            children[index].localRotation = Quaternion.Lerp(children[index].localRotation, poseChildren[index].localRotation, t1);
            children[index].localPosition = Vector3.Lerp(children[index].localPosition, poseChildren[index].localPosition, t2);
          }
        }
      }
    }

    private void StoreDefaultState()
    {
      defaultLocalPositions = new Vector3[children.Length];
      defaultLocalRotations = new Quaternion[children.Length];
      for (int index = 0; index < children.Length; ++index)
      {
        defaultLocalPositions[index] = children[index].localPosition;
        defaultLocalRotations[index] = children[index].localRotation;
      }
    }
  }
}

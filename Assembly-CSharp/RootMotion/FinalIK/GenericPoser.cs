using System;
using UnityEngine;

namespace RootMotion.FinalIK
{
  public class GenericPoser : Poser
  {
    public Map[] maps;

    [ContextMenu("Auto-Mapping")]
    public override void AutoMapping()
    {
      if (poseRoot == null)
      {
        maps = [];
      }
      else
      {
        maps = [];
        Transform[] componentsInChildren1 = transform.GetComponentsInChildren<Transform>();
        Transform[] componentsInChildren2 = poseRoot.GetComponentsInChildren<Transform>();
        for (int index = 1; index < componentsInChildren1.Length; ++index)
        {
          Transform targetNamed = GetTargetNamed(componentsInChildren1[index].name, componentsInChildren2);
          if (targetNamed != null)
          {
            Array.Resize(ref maps, maps.Length + 1);
            maps[maps.Length - 1] = new Map(componentsInChildren1[index], targetNamed);
          }
        }
        StoreDefaultState();
      }
    }

    protected override void InitiatePoser() => StoreDefaultState();

    protected override void UpdatePoser()
    {
      if (weight <= 0.0 || this.localPositionWeight <= 0.0 && this.localRotationWeight <= 0.0 || poseRoot == null)
        return;
      float localRotationWeight = this.localRotationWeight * weight;
      float localPositionWeight = this.localPositionWeight * weight;
      for (int index = 0; index < maps.Length; ++index)
        maps[index].Update(localRotationWeight, localPositionWeight);
    }

    protected override void FixPoserTransforms()
    {
      for (int index = 0; index < maps.Length; ++index)
        maps[index].FixTransform();
    }

    private void StoreDefaultState()
    {
      for (int index = 0; index < maps.Length; ++index)
        maps[index].StoreDefaultState();
    }

    private Transform GetTargetNamed(string tName, Transform[] array)
    {
      for (int index = 0; index < array.Length; ++index)
      {
        if (array[index].name == tName)
          return array[index];
      }
      return null;
    }

    [Serializable]
    public class Map
    {
      public Transform bone;
      public Transform target;
      private Vector3 defaultLocalPosition;
      private Quaternion defaultLocalRotation;

      public Map(Transform bone, Transform target)
      {
        this.bone = bone;
        this.target = target;
        StoreDefaultState();
      }

      public void StoreDefaultState()
      {
        defaultLocalPosition = bone.localPosition;
        defaultLocalRotation = bone.localRotation;
      }

      public void FixTransform()
      {
        bone.localPosition = defaultLocalPosition;
        bone.localRotation = defaultLocalRotation;
      }

      public void Update(float localRotationWeight, float localPositionWeight)
      {
        bone.localRotation = Quaternion.Lerp(bone.localRotation, target.localRotation, localRotationWeight);
        bone.localPosition = Vector3.Lerp(bone.localPosition, target.localPosition, localPositionWeight);
      }
    }
  }
}

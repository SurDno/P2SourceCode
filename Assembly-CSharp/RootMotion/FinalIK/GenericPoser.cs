// Decompiled with JetBrains decompiler
// Type: RootMotion.FinalIK.GenericPoser
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
namespace RootMotion.FinalIK
{
  public class GenericPoser : Poser
  {
    public GenericPoser.Map[] maps;

    [ContextMenu("Auto-Mapping")]
    public override void AutoMapping()
    {
      if ((UnityEngine.Object) this.poseRoot == (UnityEngine.Object) null)
      {
        this.maps = new GenericPoser.Map[0];
      }
      else
      {
        this.maps = new GenericPoser.Map[0];
        Transform[] componentsInChildren1 = this.transform.GetComponentsInChildren<Transform>();
        Transform[] componentsInChildren2 = this.poseRoot.GetComponentsInChildren<Transform>();
        for (int index = 1; index < componentsInChildren1.Length; ++index)
        {
          Transform targetNamed = this.GetTargetNamed(componentsInChildren1[index].name, componentsInChildren2);
          if ((UnityEngine.Object) targetNamed != (UnityEngine.Object) null)
          {
            Array.Resize<GenericPoser.Map>(ref this.maps, this.maps.Length + 1);
            this.maps[this.maps.Length - 1] = new GenericPoser.Map(componentsInChildren1[index], targetNamed);
          }
        }
        this.StoreDefaultState();
      }
    }

    protected override void InitiatePoser() => this.StoreDefaultState();

    protected override void UpdatePoser()
    {
      if ((double) this.weight <= 0.0 || (double) this.localPositionWeight <= 0.0 && (double) this.localRotationWeight <= 0.0 || (UnityEngine.Object) this.poseRoot == (UnityEngine.Object) null)
        return;
      float localRotationWeight = this.localRotationWeight * this.weight;
      float localPositionWeight = this.localPositionWeight * this.weight;
      for (int index = 0; index < this.maps.Length; ++index)
        this.maps[index].Update(localRotationWeight, localPositionWeight);
    }

    protected override void FixPoserTransforms()
    {
      for (int index = 0; index < this.maps.Length; ++index)
        this.maps[index].FixTransform();
    }

    private void StoreDefaultState()
    {
      for (int index = 0; index < this.maps.Length; ++index)
        this.maps[index].StoreDefaultState();
    }

    private Transform GetTargetNamed(string tName, Transform[] array)
    {
      for (int index = 0; index < array.Length; ++index)
      {
        if (array[index].name == tName)
          return array[index];
      }
      return (Transform) null;
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
        this.StoreDefaultState();
      }

      public void StoreDefaultState()
      {
        this.defaultLocalPosition = this.bone.localPosition;
        this.defaultLocalRotation = this.bone.localRotation;
      }

      public void FixTransform()
      {
        this.bone.localPosition = this.defaultLocalPosition;
        this.bone.localRotation = this.defaultLocalRotation;
      }

      public void Update(float localRotationWeight, float localPositionWeight)
      {
        this.bone.localRotation = Quaternion.Lerp(this.bone.localRotation, this.target.localRotation, localRotationWeight);
        this.bone.localPosition = Vector3.Lerp(this.bone.localPosition, this.target.localPosition, localPositionWeight);
      }
    }
  }
}

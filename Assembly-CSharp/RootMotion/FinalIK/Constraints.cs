using System;
using UnityEngine;

namespace RootMotion.FinalIK
{
  [Serializable]
  public class Constraints
  {
    public Transform transform;
    public Transform target;
    public Vector3 positionOffset;
    public Vector3 position;
    [Range(0.0f, 1f)]
    public float positionWeight;
    public Vector3 rotationOffset;
    public Vector3 rotation;
    [Range(0.0f, 1f)]
    public float rotationWeight;

    public bool IsValid() => (UnityEngine.Object) this.transform != (UnityEngine.Object) null;

    public void Initiate(Transform transform)
    {
      this.transform = transform;
      this.position = transform.position;
      this.rotation = transform.eulerAngles;
    }

    public void Update()
    {
      if (!this.IsValid())
        return;
      if ((UnityEngine.Object) this.target != (UnityEngine.Object) null)
        this.position = this.target.position;
      this.transform.position += this.positionOffset;
      if ((double) this.positionWeight > 0.0)
        this.transform.position = Vector3.Lerp(this.transform.position, this.position, this.positionWeight);
      if ((UnityEngine.Object) this.target != (UnityEngine.Object) null)
        this.rotation = this.target.eulerAngles;
      this.transform.rotation = Quaternion.Euler(this.rotationOffset) * this.transform.rotation;
      if ((double) this.rotationWeight <= 0.0)
        return;
      this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.Euler(this.rotation), this.rotationWeight);
    }
  }
}

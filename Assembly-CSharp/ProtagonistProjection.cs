using Engine.Source.Commons;
using Engine.Source.Settings;
using System;
using UnityEngine;

public class ProtagonistProjection : MonoBehaviour
{
  [SerializeField]
  private Transform cameraAnchor;
  [SerializeField]
  private Transform child;
  [SerializeField]
  private float scale = 1f;
  [SerializeField]
  private float fieldOfView = 60f;
  private Vector3 position;
  private Quaternion rotation;

  private void Start()
  {
    Transform transform = this.transform;
    this.position = transform.localPosition;
    this.rotation = transform.localRotation;
  }

  private void LateUpdate()
  {
    Transform transform = this.transform;
    float num = Mathf.Tan(InstanceByRequest<GraphicsGameSettings>.Instance.FieldOfView.Value * ((float) Math.PI / 360f)) / Mathf.Tan(this.fieldOfView * ((float) Math.PI / 360f)) * this.scale;
    transform.localScale = new Vector3(num, num, this.scale);
    transform.localPosition = this.cameraAnchor.localPosition;
    transform.localRotation = this.cameraAnchor.localRotation;
    this.child.localPosition = Matrix4x4.TRS(this.cameraAnchor.localPosition, this.cameraAnchor.localRotation, Vector3.one).inverse.MultiplyPoint(this.position);
    this.child.localRotation = Quaternion.Inverse(transform.localRotation) * this.rotation;
  }
}

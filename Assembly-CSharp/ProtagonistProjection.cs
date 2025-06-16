using System;
using Engine.Source.Commons;
using Engine.Source.Settings;
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
    position = transform.localPosition;
    rotation = transform.localRotation;
  }

  private void LateUpdate()
  {
    Transform transform = this.transform;
    float num = Mathf.Tan(InstanceByRequest<GraphicsGameSettings>.Instance.FieldOfView.Value * ((float) Math.PI / 360f)) / Mathf.Tan(fieldOfView * ((float) Math.PI / 360f)) * scale;
    transform.localScale = new Vector3(num, num, scale);
    transform.localPosition = cameraAnchor.localPosition;
    transform.localRotation = cameraAnchor.localRotation;
    child.localPosition = Matrix4x4.TRS(cameraAnchor.localPosition, cameraAnchor.localRotation, Vector3.one).inverse.MultiplyPoint(position);
    child.localRotation = Quaternion.Inverse(transform.localRotation) * rotation;
  }
}

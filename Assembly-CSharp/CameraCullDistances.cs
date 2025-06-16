using Inspectors;
using System;
using UnityEngine;

[RequireComponent(typeof (Camera))]
public class CameraCullDistances : MonoBehaviour
{
  [SerializeField]
  private CameraCullDistances.LayerFarClipping[] layerFarClippings = new CameraCullDistances.LayerFarClipping[0];
  [SerializeField]
  private float defaultFarClippingPlane = 150f;

  private void Awake() => this.ApplyImpl();

  [Inspected]
  private void Apply() => this.ApplyImpl();

  private void ApplyImpl()
  {
    Camera component = this.GetComponent<Camera>();
    float[] numArray = new float[32];
    for (int index = 0; index < 32; ++index)
      numArray[index] = this.defaultFarClippingPlane;
    foreach (CameraCullDistances.LayerFarClipping layerFarClipping in this.layerFarClippings)
    {
      int index = layerFarClipping.Layer.GetIndex();
      numArray[index] = layerFarClipping.FarClippingPlane;
    }
    component.layerCullDistances = numArray;
  }

  [Serializable]
  public class LayerFarClipping
  {
    public LayerMask Layer;
    public float FarClippingPlane;
  }
}

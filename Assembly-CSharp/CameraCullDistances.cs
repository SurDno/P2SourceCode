using System;
using Inspectors;
using UnityEngine;

[RequireComponent(typeof (Camera))]
public class CameraCullDistances : MonoBehaviour
{
  [SerializeField]
  private LayerFarClipping[] layerFarClippings = new LayerFarClipping[0];
  [SerializeField]
  private float defaultFarClippingPlane = 150f;

  private void Awake() => ApplyImpl();

  [Inspected]
  private void Apply() => ApplyImpl();

  private void ApplyImpl()
  {
    Camera component = GetComponent<Camera>();
    float[] numArray = new float[32];
    for (int index = 0; index < 32; ++index)
      numArray[index] = defaultFarClippingPlane;
    foreach (LayerFarClipping layerFarClipping in layerFarClippings)
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

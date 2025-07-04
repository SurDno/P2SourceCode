﻿using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class TOD_Animation : MonoBehaviour
{
  [Tooltip("Wind direction in degrees.")]
  public float WindDegrees;
  [Tooltip("Speed of the wind that is acting on the clouds.")]
  public float WindSpeed = 1f;
  private TOD_Sky sky;

  public Vector3 CloudUV { get; set; }

  public Vector3 OffsetUV
  {
    get
    {
      Vector3 vector3 = transform.position * 0.0001f;
      return Quaternion.Euler(0.0f, -transform.rotation.eulerAngles.y, 0.0f) * vector3;
    }
  }

  protected void Start()
  {
    sky = GetComponent<TOD_Sky>();
    CloudUV = new Vector3(Random.value, Random.value, Random.value);
  }

  protected void Update()
  {
    float num1 = 1f / 1000f * Time.deltaTime;
    float f1 = WindSpeed * num1;
    if (float.IsNaN(f1) || float.IsNaN(f1))
      return;
    float num2 = Mathf.Sin((float) Math.PI / 180f * WindDegrees);
    float num3 = Mathf.Cos((float) Math.PI / 180f * WindDegrees);
    float x1 = CloudUV.x;
    float y1 = CloudUV.y;
    float z1 = CloudUV.z;
    float f2 = y1 + num1 * 0.1f;
    float f3 = x1 - f1 * num2;
    float f4 = z1 - f1 * num3;
    float x2 = f3 - Mathf.Floor(f3);
    float y2 = f2 - Mathf.Floor(f2);
    float z2 = f4 - Mathf.Floor(f4);
    CloudUV = new Vector3(x2, y2, z2);
    Quaternion.Euler(0.0f, (float) (WindSpeed * (double) y2 * 360.0), 0.0f);
    sky.Components.BillboardTransform.localRotation = Quaternion.Euler(0.0f, (float) (WindSpeed * (double) y2 * 360.0), 0.0f);
  }
}

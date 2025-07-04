﻿using UnityEngine;
using UnityEngine.Profiling;

[RequireComponent(typeof (Light))]
public class LightCameraDistance : MonoBehaviour
{
  [SerializeField]
  private AnimationCurve rangeCurve = AnimationCurve.Constant(0.0f, 10f, 1f);
  private Transform cachedTransform;
  private Light cachedLight;
  private float baseRange;
  private LightShadows shadows;

  private void OnPreCullEvent(Camera camera)
  {
    if (Profiler.enabled)
      Profiler.BeginSample(nameof (LightCameraDistance));
    OnPreCullEvent2(camera);
    if (!Profiler.enabled)
      return;
    Profiler.EndSample();
  }

  private void OnPreCullEvent2(Camera camera)
  {
    float num = rangeCurve.Evaluate(Vector3.Distance(camera.transform.position, cachedTransform.position));
    cachedLight.range = num * baseRange;
    cachedLight.shadows = num < 0.3 ? LightShadows.None : shadows;
  }

  private void OnEnable()
  {
    cachedTransform = transform;
    cachedLight = GetComponent<Light>();
    baseRange = cachedLight.range;
    shadows = cachedLight.shadows;
    Camera.onPreCull += OnPreCullEvent;
  }

  private void OnDisable()
  {
    Camera.onPreCull -= OnPreCullEvent;
    cachedLight.range = baseRange;
    cachedLight.shadows = shadows;
    cachedLight = null;
    cachedTransform = null;
  }
}

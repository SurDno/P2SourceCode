using System;
using UnityEngine;
using UnityEngine.Rendering;

[Serializable]
public class TOD_ReflectionParameters
{
  [Tooltip("Reflection probe mode.")]
  public TOD_ReflectionType Mode = TOD_ReflectionType.None;
  [Tooltip("Clear flags to use for the reflection.")]
  public ReflectionProbeClearFlags ClearFlags = ReflectionProbeClearFlags.Skybox;
  [Tooltip("Layers to include in the reflection.")]
  public LayerMask CullingMask = 0;
  [Tooltip("Time slicing behaviour to spread out rendering cost over multiple frames.")]
  public ReflectionProbeTimeSlicingMode TimeSlicing = ReflectionProbeTimeSlicingMode.AllFacesAtOnce;
  [Tooltip("Refresh interval of the reflection cubemap in seconds.")]
  [TOD_Min(0.0f)]
  public float UpdateInterval = 1f;
  public float NearClipPlane = 0.3f;
  public float FarClipPlane = 15f;
}

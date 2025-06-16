// Decompiled with JetBrains decompiler
// Type: LightCameraShadowDistance
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.Profiling;

#nullable disable
[RequireComponent(typeof (Light))]
public class LightCameraShadowDistance : MonoBehaviour
{
  [SerializeField]
  private float shadowDistance = 15f;
  private Transform cachedTransform;
  private Light cachedLight;
  private LightShadows shadows;

  private void OnPreCullEvent(Camera camera)
  {
    if (Profiler.enabled)
      Profiler.BeginSample(nameof (LightCameraShadowDistance));
    this.OnPreCullEvent2(camera);
    if (!Profiler.enabled)
      return;
    Profiler.EndSample();
  }

  private void OnPreCullEvent2(Camera camera)
  {
    this.cachedLight.shadows = (double) Vector3.Distance(camera.transform.position, this.cachedTransform.position) > (double) this.shadowDistance ? LightShadows.None : this.shadows;
  }

  private void OnEnable()
  {
    this.cachedTransform = this.transform;
    this.cachedLight = this.GetComponent<Light>();
    this.shadows = this.cachedLight.shadows;
    Camera.onPreCull += new Camera.CameraCallback(this.OnPreCullEvent);
  }

  private void OnDisable()
  {
    Camera.onPreCull -= new Camera.CameraCallback(this.OnPreCullEvent);
    this.cachedLight.shadows = this.shadows;
    this.cachedLight = (Light) null;
    this.cachedTransform = (Transform) null;
  }
}

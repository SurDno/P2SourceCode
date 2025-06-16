// Decompiled with JetBrains decompiler
// Type: Cinemachine.LensSettings
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
namespace Cinemachine
{
  [DocumentationSorting(2f, DocumentationSortingAttribute.Level.UserRef)]
  [Serializable]
  public struct LensSettings
  {
    public static LensSettings Default = new LensSettings(40f, 10f, 0.1f, 5000f, 0.0f, false, 1f);
    [Range(1f, 179f)]
    [Tooltip("This is the camera view in vertical degrees. For cinematic people, a 50mm lens on a super-35mm sensor would equal a 19.6 degree FOV")]
    public float FieldOfView;
    [Tooltip("When using an orthographic camera, this defines the half-height, in world coordinates, of the camera view.")]
    public float OrthographicSize;
    [Tooltip("This defines the near region in the renderable range of the camera frustum. Raising this value will stop the game from drawing things near the camera, which can sometimes come in handy.  Larger values will also increase your shadow resolution.")]
    public float NearClipPlane;
    [Tooltip("This defines the far region of the renderable range of the camera frustum. Typically you want to set this value as low as possible without cutting off desired distant objects")]
    public float FarClipPlane;
    [Range(-180f, 180f)]
    [Tooltip("Camera Z roll, or tilt, in degrees.")]
    public float Dutch;

    internal bool Orthographic { get; set; }

    internal float Aspect { get; set; }

    public static LensSettings FromCamera(Camera fromCamera)
    {
      LensSettings lensSettings = LensSettings.Default;
      if ((UnityEngine.Object) fromCamera != (UnityEngine.Object) null)
      {
        lensSettings.FieldOfView = fromCamera.fieldOfView;
        lensSettings.OrthographicSize = fromCamera.orthographicSize;
        lensSettings.NearClipPlane = fromCamera.nearClipPlane;
        lensSettings.FarClipPlane = fromCamera.farClipPlane;
        lensSettings.Orthographic = fromCamera.orthographic;
        lensSettings.Aspect = fromCamera.aspect;
      }
      return lensSettings;
    }

    public LensSettings(
      float fov,
      float orthographicSize,
      float nearClip,
      float farClip,
      float dutch,
      bool ortho,
      float aspect)
      : this()
    {
      this.FieldOfView = fov;
      this.OrthographicSize = orthographicSize;
      this.NearClipPlane = nearClip;
      this.FarClipPlane = farClip;
      this.Dutch = dutch;
      this.Orthographic = ortho;
      this.Aspect = aspect;
    }

    public static LensSettings Lerp(LensSettings lensA, LensSettings lensB, float t)
    {
      t = Mathf.Clamp01(t);
      return new LensSettings()
      {
        FarClipPlane = Mathf.Lerp(lensA.FarClipPlane, lensB.FarClipPlane, t),
        NearClipPlane = Mathf.Lerp(lensA.NearClipPlane, lensB.NearClipPlane, t),
        FieldOfView = Mathf.Lerp(lensA.FieldOfView, lensB.FieldOfView, t),
        OrthographicSize = Mathf.Lerp(lensA.OrthographicSize, lensB.OrthographicSize, t),
        Dutch = Mathf.Lerp(lensA.Dutch, lensB.Dutch, t),
        Aspect = Mathf.Lerp(lensA.Aspect, lensB.Aspect, t),
        Orthographic = lensA.Orthographic && lensB.Orthographic
      };
    }

    public void Validate()
    {
      this.NearClipPlane = Mathf.Max(this.NearClipPlane, 0.01f);
      this.FarClipPlane = Mathf.Max(this.FarClipPlane, this.NearClipPlane + 0.01f);
    }
  }
}

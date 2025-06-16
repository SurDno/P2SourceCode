﻿using System;
using UnityEngine;

namespace UnityStandardAssets.ImageEffects
{
  [AddComponentMenu("")]
  public class ImageEffects
  {
    public static void RenderDistortion(
      Material material,
      RenderTexture source,
      RenderTexture destination,
      float angle,
      Vector2 center,
      Vector2 radius)
    {
      if ((double) source.texelSize.y < 0.0)
      {
        center.y = 1f - center.y;
        angle = -angle;
      }
      Matrix4x4 matrix4x4 = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0.0f, 0.0f, angle), Vector3.one);
      material.SetMatrix("_RotationMatrix", matrix4x4);
      material.SetVector("_CenterRadius", new Vector4(center.x, center.y, radius.x, radius.y));
      material.SetFloat("_Angle", angle * ((float) Math.PI / 180f));
      Graphics.Blit((Texture) source, destination, material);
    }

    [Obsolete("Use Graphics.Blit(source,dest) instead")]
    public static void Blit(RenderTexture source, RenderTexture dest)
    {
      Graphics.Blit((Texture) source, dest);
    }

    [Obsolete("Use Graphics.Blit(source, destination, material) instead")]
    public static void BlitWithMaterial(
      Material material,
      RenderTexture source,
      RenderTexture dest)
    {
      Graphics.Blit((Texture) source, dest, material);
    }
  }
}

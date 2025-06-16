// Decompiled with JetBrains decompiler
// Type: SoundPropagation.Math
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace SoundPropagation
{
  public static class Math
  {
    public static Vector3 DirectionalityToDirection(Vector3 directionality, Vector3 fallback)
    {
      float magnitude = directionality.magnitude;
      if ((double) magnitude == 0.0)
        return fallback;
      directionality /= magnitude;
      return Vector3.Lerp(fallback, directionality, magnitude);
    }

    public static float Normalize(ref Vector3 vector)
    {
      float magnitude = vector.magnitude;
      if ((double) magnitude > 0.0)
      {
        vector /= magnitude;
      }
      else
      {
        vector.x = 0.0f;
        vector.y = 0.0f;
        vector.z = 0.0f;
      }
      return magnitude;
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: lerper
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

#nullable disable
public class lerper
{
  public static float lerp_safe(float l, float p1, float p2)
  {
    if ((double) p1 <= (double) p2)
      return p1 + (p2 - p1) * l;
    l = 1f - l;
    return p2 + (p1 - p2) * l;
  }
}

// Decompiled with JetBrains decompiler
// Type: Cinemachine.Utility.UnityRectExtensions
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace Cinemachine.Utility
{
  public static class UnityRectExtensions
  {
    public static Rect Inflated(this Rect r, Vector2 delta)
    {
      return new Rect(r.xMin - delta.x, r.yMin - delta.y, r.width + delta.x * 2f, r.height + delta.y * 2f);
    }
  }
}

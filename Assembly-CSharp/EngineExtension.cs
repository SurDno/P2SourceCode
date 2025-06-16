// Decompiled with JetBrains decompiler
// Type: EngineExtension
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Types;
using UnityEngine;

#nullable disable
public static class EngineExtension
{
  public static Vector2 To(this Position vector) => new Vector2(vector.X, vector.Y);

  public static Position To(this Vector2 vector) => new Position(vector.x, vector.y);
}
